namespace Aspx.WebControls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// #todo: Make sure that all resources are disposed correctly when required
// #todo: Make sure that we don't have useless calls or that they are called multiple times for no reason
// #todo: Refactor all the methods that are responsible to get the data to another class
// #todo: Create smart caching capabilities and let developers the ability to adjust it
// #todo: Add missing documentation

public sealed partial class RepositoryDataSourceView(IRepositoryDataSource owner, string viewName, HttpContext context)
    : DataSourceView(owner, viewName)
{
    private static readonly Regex SortExpressionRegex = new(@"^(?<SortExpr>(?<PrimarySelector>[A-Za-z_]\w*)(?:\s+(?<PrimaryDir>(?i:Desc)))?(?:\s*,\s*(?:[A-Za-z_]\w*)(?:\s+(?i:Desc))?)*)\s*(?<DirFlag>(?i:DESC))?$");

    private const string OldValues = "oldValues";

    private string _commitMethod;

    private ConflictOptions _conflictDetection;

    private string _deleteMethod;

    private string _filterExpression;

    private string _insertMethod;

    private InstanceManager _instanceManager;

    private string _selectMethod;

    private string _sortExpression;

    private string _target;

    private string _updateMethod;
    
    //Contract.Requires<ArgumentNullException>(owner != null);

    public override bool CanDelete
        => DeleteMethod.Length > 0;

    public override bool CanInsert
        => InsertMethod.Length > 0;

    public override bool CanPage
        => EnablePaging;

    public override bool CanRetrieveTotalRowCount
        => EnablePaging;

    public override bool CanSort
        => true;

    public override bool CanUpdate
        => UpdateMethod.Length > 0;

    public string CommitMethod
    {
        get => _commitMethod ?? string.Empty;

        set {
            if (_commitMethod != null && _commitMethod == value)
            {
                return;
            }

            _commitMethod = value;
        }
    }

    public ConflictOptions ConflictDetection
    {
        get => _conflictDetection;

        set {
            if (value < ConflictOptions.OverwriteChanges || value > ConflictOptions.CompareAllValues)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _conflictDetection = value;

            OnDataSourceViewChanged(EventArgs.Empty);
        }
    }

    public bool ConvertNullToDBNull { get; set; }

    public string DeleteMethod
    {
        get => _deleteMethod ?? string.Empty;
        set => _deleteMethod = value;
    }

    public bool EnablePaging { get; set; }

    public string FilterExpression
    {
        get => _filterExpression ?? string.Empty;

        set {
            if (_filterExpression == value)
            {
                return;
            }

            _filterExpression = value;

            OnDataSourceViewChanged(EventArgs.Empty);
        }
    }

    public string InsertMethod
    {
        get => _insertMethod ?? string.Empty;
        set => _insertMethod = value;
    }

    public string SelectMethod
    {
        get => _selectMethod ?? string.Empty;

        set {
            if (_selectMethod == value)
            {
                return;
            }

            _selectMethod = value;

            OnDataSourceViewChanged(EventArgs.Empty);
        }
    }

    public string SortExpression
    {
        get => _sortExpression ?? string.Empty;
        set => _sortExpression = value;
    }

    public string Target
    {
        get => _target ?? string.Empty;
        set => _target = value;
    }

    public string UpdateMethod
    {
        get => _updateMethod ?? string.Empty;
        set => _updateMethod = value;
    }

    private InstanceManager InstanceManager
        => _instanceManager ??= new InstanceManager(owner, context.Items);

    private IEnumerable GetDataCore(DataSourceOperationResult result, DataSourceSelectArguments arguments)
    {
        IEnumerable enumerable = TryGetDataFromQueryableObject(result.ReturnValue, arguments);

        // Throw an exception because paging is not supported by other forms of data other than IQueryable
        if (EnablePaging && enumerable == null)
        {
            throw new NotSupportedException(string.Format(Strings.PagingNotSupportedOnIEnumerable, owner.ID));
        }

        // Convert the data to a non-lazy enumerable collection when paging is not enabled and therefor counting is not possible
        if (enumerable != null && !arguments.RetrieveTotalRowCount)
        {
            enumerable = enumerable.Cast<object>().ToArray();
        }

        return enumerable
               ?? TryGetDataFromDataView(result.ReturnValue, arguments)
               ?? TryGetDataFromDataTable(result.ReturnValue, arguments)
               ?? TryGetEnumerableData(result.ReturnValue, arguments);
    }

    protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
    {
        if (!CanDelete)
        {
            throw new InvalidOperationException(string.Format(Strings.DeleteNotSupported, owner.ID));
        }

        var dataObjectInfo = CreateDataObjectInfo(DeleteMethod, DataSourceOperation.Delete);

        IOrderedDictionary caseInsensitiveValues = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        MergeDictionaries(DeleteParameters, keys, caseInsensitiveValues);

        if (ConflictDetection == ConflictOptions.CompareAllValues)
        {
            if (oldValues == null)
            {
                throw new InvalidOperationException(string.Format(Strings.Pessimistic, dataObjectInfo.OperationType.GetLocalizedName(), owner.ID, OldValues));
            }

            MergeDictionaries(DeleteParameters, oldValues, caseInsensitiveValues);
        }

        var oldDataObject = BuildDataObject(dataObjectInfo.DataObjectType, caseInsensitiveValues);

        var source = DeleteParameters.GetValues(context, (Control)owner);

        var operationMethod = CreateOperationMethod(dataObjectInfo, oldDataObject, null, source);

        var e = new DataSourceMethodEventArgs(operationMethod.Parameters);

        OnDeleting(e);

        if (e.Cancel)
        {
            return 0;
        }

        var result = InvokeMethod(operationMethod, true);

        OnDataSourceViewChanged(EventArgs.Empty);

        return result.AffectedRows;
    }

    protected override int ExecuteInsert(IDictionary values)
    {
        if (!CanInsert)
        {
            throw new InvalidOperationException(string.Format(Strings.InsertNotSupported, owner.ID));
        }

        var dataObjectInfo = CreateDataObjectInfo(InsertMethod, DataSourceOperation.Insert);

        IOrderedDictionary caseInsensitiveValues = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        if (values == null || values.Count == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.InsertRequiresValues, owner.ID));
        }

        MergeDictionaries(InsertParameters, values, caseInsensitiveValues);

        var newDataObject = BuildDataObject(dataObjectInfo.DataObjectType, caseInsensitiveValues);

        var source = InsertParameters.GetValues(context, (Control)owner);

        var operationMethod = CreateOperationMethod(dataObjectInfo, null, newDataObject, source);

        var e = new DataSourceMethodEventArgs(operationMethod.Parameters);

        OnInserting(e);

        if (e.Cancel)
        {
            return 0;
        }

        var result = InvokeMethod(operationMethod, true);

        OnDataSourceViewChanged(EventArgs.Empty);

        return result.AffectedRows;
    }

    protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
    {
        if (SelectMethod.Length == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.SelectNotSupported, owner.ID));
        }

        if (CanSort)
        {
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);
        }

        if (CanPage)
        {
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
        }

        if (CanRetrieveTotalRowCount)
        {
            arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
        }

        arguments.RaiseUnsupportedCapabilitiesError(this);

        // Copy the parameters into a case-insensitive dictionary
        IOrderedDictionary mergedParameters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
        IDictionary selectParameters = SelectParameters.GetValues(context, (Control)owner);

        foreach (DictionaryEntry entry in selectParameters)
        {
            //Contract.Assume(entry.Key != null, "The key should never be null");

            mergedParameters[entry.Key] = entry.Value;
        }

        var e = new DataSourceSelectingEventArgs(
            mergedParameters,
            arguments,
            false);

        OnSelecting(e);

        if (e.Cancel)
        {
            return null;
        }

        arguments.SortExpression = string.IsNullOrEmpty(arguments.SortExpression)
                                       ? SortExpression
                                       : arguments.SortExpression;

        var operationMethod = CreateOperationMethod( SelectMethod, mergedParameters,  DataSourceOperation.Select);

        var result = InvokeMethod(operationMethod);

        if (result.ReturnValue == null)
        {
            return null;
        }

        return GetDataCore(result, arguments);
    }

    protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
    {
        if (!CanUpdate)
        {
            throw new InvalidOperationException(string.Format(Strings.UpdateNotSupported, owner.ID));
        }

        var dataObjectInfo = CreateDataObjectInfo(UpdateMethod, DataSourceOperation.Update);

        if (ConflictDetection == ConflictOptions.CompareAllValues && dataObjectInfo.MethodSecondParameter == null)
        {
            throw new InvalidOperationException(string.Format(Strings.OptimisticUpdateMethodRequiresAdditionalParam, owner.ID, dataObjectInfo.DataObjectType.FullName));
        }

        IOrderedDictionary caseInsensitiveValues = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        MergeDictionaries(UpdateParameters, oldValues, caseInsensitiveValues);

        MergeDictionaries(UpdateParameters, keys, caseInsensitiveValues);

        MergeDictionaries(UpdateParameters, values, caseInsensitiveValues);

        var newDataObject = BuildDataObject(dataObjectInfo.DataObjectType, caseInsensitiveValues);

        object oldDataObject = null;

        if (ConflictDetection == ConflictOptions.CompareAllValues)
        {
            if (oldValues == null)
            {
                throw new InvalidOperationException(string.Format(Strings.Pessimistic, dataObjectInfo.OperationType.GetLocalizedName(), owner.ID, OldValues));
            }

            IDictionary caseInsensitiveOldValues = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

            MergeDictionaries(UpdateParameters, oldValues, caseInsensitiveOldValues);

            MergeDictionaries(UpdateParameters, keys, caseInsensitiveOldValues);

            oldDataObject = BuildDataObject(dataObjectInfo.DataObjectType, caseInsensitiveOldValues);
        }

        IDictionary source = UpdateParameters.GetValues(context, (Control)owner);

        var operationMethod = CreateOperationMethod(
            dataObjectInfo,
            oldDataObject,
            newDataObject,
            source);

        var e = new DataSourceMethodEventArgs(operationMethod.Parameters);

        OnUpdating(e);

        if (e.Cancel)
        {
            return 0;
        }

        var result = InvokeMethod(operationMethod, true);

        OnDataSourceViewChanged(EventArgs.Empty);

        return result.AffectedRows;
    }

    private DataObjectInfo CreateDataObjectInfo(string methodName, DataSourceOperation operation)
    {
        var methods = GetMethods(methodName).ToArray();

        if (methods.Length == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.DataObjectMethodNotFound, owner.ID, methodName));
        }

        if (methods.Length > 1)
        {
            throw new InvalidOperationException(string.Format(Strings.DataObjectMethodOverloadingNotSupported, owner.ID, methodName));
        }

        var method = methods.First();

        var parameters = method.GetParameters();

        var length = parameters.Length;

        if (length == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.DataObjectMethodNoParams, operation.GetLocalizedName(), owner.ID, methodName));
        }

        var firstParameter = parameters.ElementAt(0);

        ParameterInfo secondParameter = null;

        if (length > 1)
        {
            secondParameter = parameters.ElementAt(1);

            if (ConflictDetection == ConflictOptions.OverwriteChanges
                || firstParameter.ParameterType != secondParameter.ParameterType
                || operation == DataSourceOperation.Select
                || operation == DataSourceOperation.SelectCount)
            {
                secondParameter = null;
            }
        }

        // Infer the type directly from the first parameter
        var type = TypeHelper.GetType(firstParameter.ParameterType.FullName);

        return new DataObjectInfo(operation, type, method, firstParameter, secondParameter);
    }

    private DataSourceOperationMethod CreateOperationMethod(
        string methodName,
        IDictionary allParameters,
        DataSourceOperation operation)
    {
        var isCountOperation = operation == DataSourceOperation.SelectCount;

        var methodType = DataObjectMethodType.Select;

        if (!isCountOperation)
        {
            methodType = operation.GetMethodType();
        }

        // Holds the method that was retrieved from the type and should be executed for the operation.
        MethodInfo operationMethod = null;

        // Holds the parameters for the method that was retrieved from the type.
        ParameterInfo[] methodParameters = null;

        // Indicates how confident we are that a method overload is a good match
        // -1 - indicates no confidence - no appropriate methods have been found at all
        // 0 - indicates low confidence - only parameter names match 
        // 1 - indicates medium confidence - parameter names match, method is DataObjectMethod
        // 2 - indicates high confidence - parameter names match, method is DataObjectMethod, is default method 
        var highestConfidence = -1;
        var confidenceConflict = false;

        foreach (var method in GetMethods(methodName, allParameters))
        {
            var confidence = 0;

            if (!isCountOperation)
            {
                if (Attribute.GetCustomAttribute(method, typeof(DataObjectMethodAttribute), true) is DataObjectMethodAttribute attribute && attribute.MethodType == methodType)
                {
                    confidence = !attribute.IsDefault ? 1 : 2;
                }
            }

            if (confidence == highestConfidence)
            {
                confidenceConflict = true;
            }
            else if (confidence > highestConfidence)
            {
                operationMethod = method;
                methodParameters = method.GetParameters();
                highestConfidence = confidence;
                confidenceConflict = false;
            }
        }

        if (confidenceConflict)
        {
            throw new InvalidOperationException(string.Format(Strings.MultipleOverloads, owner.ID));
        }

        if (operationMethod == null)
        {
            if (allParameters.Count == 0)
            {
                throw new InvalidOperationException(string.Format(Strings.MethodNotFoundNoParams, owner.ID, methodName));
            }

            var parametersName = JoinMethodParameters(allParameters);

            throw new InvalidOperationException(string.Format(Strings.MethodNotFoundWithParams, owner.ID, methodName, parametersName));
        }

        // Builds the parameters that should be used for the operation.
        var operationParameters = BuildOperationParameters(ConvertNullToDBNull, methodParameters, allParameters);

        return new DataSourceOperationMethod(operation, operationMethod, operationParameters);
    }

    private DataSourceOperationMethod CreateOperationMethod(
        DataObjectInfo dataObjectInfo,
        object oldDataObject,
        object newDataObject,
        IDictionary additionalParameters)
    {
        var newDataKey = dataObjectInfo.MethodFirstParameter.Name;

        var oldDataKey = oldDataObject != null && newDataObject != null && dataObjectInfo.MethodSecondParameter != null ? dataObjectInfo.MethodSecondParameter.Name : null;

        var parameters = new OrderedDictionary(2, StringComparer.OrdinalIgnoreCase);

        if (oldDataObject == null)
        {
            parameters.Add(newDataKey, newDataObject);
        }
        else if (newDataObject == null)
        {
            parameters.Add(newDataKey, oldDataObject);
        }
        else
        {
            if (oldDataKey != null)
            {
                parameters.Add(newDataKey, newDataObject);
                parameters.Add(oldDataKey, oldDataObject);
            }
            else
            {
                throw new InvalidOperationException(string.Format(Strings.DataObjectMethodRequiresAdditionalParam, owner.ID, dataObjectInfo.OperationType.GetPropertyName(), dataObjectInfo.DataObjectType.FullName));
            }
        }

        foreach (DictionaryEntry parameter in additionalParameters)
        {
            //Contract.Assume(parameter.Key != null, "Key should never be null");

            parameters.Add(parameter.Key, parameter.Value);
        }

        // Builds the parameters that should be used for the operation.
        var operationParameters = BuildOperationParameters(
            ConvertNullToDBNull,
            dataObjectInfo.DataObjectMethod.GetParameters(),
            parameters);

        return new DataSourceOperationMethod(
            dataObjectInfo.OperationType,
            dataObjectInfo.DataObjectMethod,
            operationParameters.AsReadOnly());
    }

    private DataSourceOperationResult InvokeMethod(DataSourceOperationMethod operationMethod, bool commit = false)
    {
        object instance = null;

        if (operationMethod.Method.IsStatic)
        {
            InstanceManager.ReleaseInstance();
        }
        else
        {
            instance = InstanceManager.CreateInstance();
        }

        object returnValue = null;
        var affectedRows = -1;

        var eventFired = false;
        object[] values = null;

        if (operationMethod.Parameters is { Count: > 0 })
        {
            values = new object[operationMethod.Parameters.Count];

            operationMethod.Parameters.Values.CopyTo(values, 0);
        }

        try
        {
            returnValue = operationMethod.Method.Invoke(instance, values);

            if (commit)
            {
                Commit();
            }
        }
        catch (Exception ex)
        {
            eventFired = true;

            var e = UpdateDataSourceStatus(operationMethod, values, returnValue, ex);

            affectedRows = e.AffectedRows;

            if (!e.ExceptionHandled)
            {
                throw;
            }
        }
        finally
        {
            if (!eventFired)
            {
                var e = UpdateDataSourceStatus(operationMethod, values, returnValue);

                affectedRows = e.AffectedRows;
            }
        }

        return new DataSourceOperationResult(returnValue, affectedRows);
    }

    private static object BuildObjectValue(object value, Type destType, string name)
    {
        if (value != null && !destType.IsInstanceOfType(value))
        {
            var isNullable = false;

            if (destType.IsGenericType && destType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                destType = destType.GetGenericArguments()[0];

                isNullable = true;
            }
            else if (destType.IsByRef)
            {
                destType = destType.GetElementType();
            }

            value = ConvertType(value, destType, name);

            if (isNullable)
            {
                var sourceType = value.GetType();

                if (destType != sourceType)
                {
                    const string nullable = "Nullable<{0}>";

                    var fullName = string.Format(CultureInfo.InvariantCulture, nullable, destType.FullName);

                    throw new InvalidOperationException(string.Format(Strings.CannotConvertType, name, sourceType.FullName, fullName));
                }
            }
        }

        return value;
    }

    private static OrderedDictionary BuildOperationParameters(
        bool convertNullToDbNull,
        ParameterInfo[] methodParameters,
        IDictionary inputParameters)
    {
        OrderedDictionary collection = null;

        var length = methodParameters.Length;

        if (length > 0)
        {
            collection = new OrderedDictionary(length, StringComparer.OrdinalIgnoreCase);

            foreach (var parameter in methodParameters)
            {
                var parameterName = parameter.Name;

                var inputParameter = inputParameters[parameterName];

                inputParameter = !convertNullToDbNull || inputParameter != null ? BuildObjectValue(inputParameter, parameter.ParameterType, parameterName) : DBNull.Value;

                collection.Add(parameterName, inputParameter);
            }
        }

        return collection;
    }

    private static object ConvertType(object value, Type type, string name)
    {
        if (value is string text && type != null)
        {
            var converter = TypeDescriptor.GetConverter(type);

            try
            {
                value = converter.ConvertFromInvariantString(text);
            }
            catch (NotSupportedException)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotConvertType, name, typeof(string).FullName, type.FullName));
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotConvertType, name, typeof(string).FullName, type.FullName));
            }
        }

        return value;
    }

    private static IDictionary GetOutputParameters(ParameterInfo[] parameters, object[] values)
    {
        IDictionary dictionary = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < parameters.Length; ++index)
        {
            var parameterInfo = parameters[index];

            if (parameterInfo.ParameterType.IsByRef)
            {
                dictionary[parameterInfo.Name] = values[index];
            }
        }

        return dictionary;
    }

    private static string JoinMethodParameters(IDictionary parameters)
    {
        var paramNames = new string[parameters.Count];

        parameters.Keys.CopyTo(paramNames, 0);

        return string.Join(", ", paramNames);
    }

    private static void MergeDictionaries(
        ParameterCollection reference,
        IDictionary source,
        IDictionary destination)
    {
        //Contract.Requires<ArgumentNullException>(destination != null);
        //Contract.Requires<ArgumentNullException>(reference != null);

        if (source == null)
        {
            return;
        }

        foreach (DictionaryEntry entry in source)
        {
            var destValue = entry.Value;

            var destKey = (string)entry.Key;

            var referenceParameter = reference.Cast<Parameter>()
                                              .FirstOrDefault(
                                                  p => string.Equals(
                                                      p.Name,
                                                      destKey,
                                                      StringComparison.OrdinalIgnoreCase));

            if (referenceParameter != null)
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                var method = referenceParameter.GetType()
                                               .GetMethod(
                                                   "GetValue",
                                                   flags,
                                                   null,
                                                   [typeof(object), typeof(bool)],
                                                   null);

                destValue = method?.Invoke(referenceParameter, [destValue, true]);
            }

            //Contract.Assume(destKey != null, "Key should never be null");

            destination[destKey] = destValue;
        }
    }

    private object BuildDataObject(Type dataObjectType, IDictionary inputParameters)
    {
        //Contract.Requires<ArgumentNullException>(dataObjectType != null);

        var instance = Activator.CreateInstance(dataObjectType);

        var properties = TypeDescriptor.GetProperties(instance);

        foreach (DictionaryEntry dictionaryEntry in inputParameters)
        {
            var propertyName = dictionaryEntry.Key == null ? string.Empty : dictionaryEntry.Key.ToString();

            var propertyDescriptor = properties.Find(propertyName, true);

            if (propertyDescriptor == null)
            {
                throw new InvalidOperationException(string.Format(Strings.DataObjectPropertyNotFound, propertyName, dataObjectType.FullName, owner.ID));
            }

            if (propertyDescriptor.IsReadOnly)
            {
                throw new InvalidOperationException(string.Format(Strings.DataObjectPropertyReadOnly, propertyName, dataObjectType.FullName, owner.ID));
            }

            var value = BuildObjectValue(dictionaryEntry.Value, propertyDescriptor.PropertyType, propertyName);

            propertyDescriptor.SetValue(instance, value);
        }

        return instance;
    }

    private void Commit()
    {
        var targetObject = InstanceManager.GetTargetObject();

        var targetType = targetObject.GetType();

        MethodInfo method;

        if (CommitMethod.Length == 0)
        {
            method = targetType.GetMethod("Commit")
                     ?? targetType.GetMethod("Store") ?? targetType.GetMethod("Save");

            if (method != null)
            {
                CommitMethod = method.Name;
            }
        }
        else
        {
            method = targetType.GetMethod(CommitMethod);
        }

        if (method != null && method.GetParameters().Length == 0)
        {
            // #todo: Optimise the amount of calls to commit.
            // #todo: Add Committing and Committed events.
            // #todo: Add CommitMode to allow developers to choose between automatic and manual commit operation.

            method.Invoke(targetObject, null);

            // #todo: This is only temporary and in future builds it will be removed.
            InstanceManager.ReleaseInstance(true);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.MissingCommitMethod, owner.ID, targetType.FullName));
        }
    }

    private DataView CreateFilteredDataView(DataTable table, string sortExpression, string filterExpression)
    {
        if (table == null)
        {
            return null;
        }

        var values = FilterParameters.GetValues(context, (Control)owner);

        if (filterExpression.Length > 0)
        {
            var e = new DataSourceFilteringEventArgs(values);

            OnFiltering(e);

            if (e.Cancel)
            {
                return null;
            }
        }

        return FilteredDataSetHelper.CreateFilteredDataView(table, sortExpression, filterExpression, values);
    }

    private IEnumerable<MethodInfo> GetMethods(string inputMethodName)
        => from method in InstanceManager.GetMethods()
           where string.Equals(inputMethodName, method.Name, StringComparison.OrdinalIgnoreCase) && !method.IsGenericMethodDefinition
           select method;

    private IEnumerable<MethodInfo> GetMethods(string inputMethodName, IDictionary inputParameters)
    {
        var methods = from method in GetMethods(inputMethodName)
                      let parameters = method.GetParameters()
                      where parameters.Length == inputParameters.Count
                      select method;
        
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();

            var parameterMismatch = parameters.Any(p => !inputParameters.Contains(p.Name));

            if (!parameterMismatch)
            {
                yield return method;
            }
        }
    }

    private IEnumerable TryGetDataFromDataTable(object dataObject, DataSourceSelectArguments arguments)
    {
        DataView dataView = null;

        var dataTable = FilteredDataSetHelper.GetDataTable((Control)owner, dataObject);

        if (dataTable != null)
        {
            if (arguments.RetrieveTotalRowCount)
            {
                arguments.TotalRowCount = dataTable.Rows.Count;
            }

            dataView = CreateFilteredDataView(dataTable, arguments.SortExpression, FilterExpression);
        }

        return dataView;
    }

    private IEnumerable TryGetDataFromDataView(object dataObject, DataSourceSelectArguments arguments)
    {
        var dataView = dataObject as DataView;

        if (dataView != null)
        {
            if (arguments.RetrieveTotalRowCount)
            {
                arguments.TotalRowCount = dataView.Count;
            }

            if (FilterExpression.Length > 0)
            {
                throw new NotSupportedException(string.Format(Strings.FilterNotSupported, owner.ID));
            }

            if (!string.IsNullOrEmpty(arguments.SortExpression))
            {
                dataView.Sort = arguments.SortExpression;
            }
        }

        return dataView;
    }

    private IQueryable TryGetDataFromQueryableObject(object dataObject, DataSourceSelectArguments arguments)
    {
        // #todo: Add filter capabilities for methods that return IQueryable 

        if (FilterExpression.Length > 0)
        {
            throw new NotSupportedException(string.Format(Strings.FilterNotSupported, owner.ID));
        }
        
        if (dataObject is IQueryable queryable)
        {
            queryable = ProcessPagingAndSortingData();
        }
        else
        {
            queryable = null;
        }

        return queryable;
        
        IQueryable ProcessPagingAndSortingData()
        {
            var expression = CreateSortExpression();

            if (EnablePaging && arguments.MaximumRows > 0)
            {
                Expression countMethodResultExpression = Expression.Call(
                    typeof(Queryable),
                    "Count",
                    [queryable.ElementType],
                    expression);

                arguments.TotalRowCount = (int)Expression.Lambda(countMethodResultExpression).Compile().DynamicInvoke();

                expression = Expression.Call(
                    typeof(QueryableExtensions),
                    "Page",
                    [queryable.ElementType],
                    expression,
                    Expression.Constant(arguments.StartRowIndex / arguments.MaximumRows),
                    Expression.Constant(arguments.MaximumRows));
            }

            if (expression != null)
            {
                queryable = Expression.Lambda(expression).Compile().DynamicInvoke() as IQueryable;
            }

            return queryable;
        }
        
        Expression CreateSortExpression()
        {
            var sortExpression = arguments.SortExpression;

            if (string.IsNullOrEmpty(sortExpression))
            {
                return null;
            }

            var selectors = GetSelectors(sortExpression);
            var selector = selectors.First().Split(' ');

            Expression resultExpression = Expression.Call(
                typeof(QueryableExtensions),
                "OrderBy",
                [queryable.ElementType],
                Expression.Constant(queryable),
                Expression.Constant(selector.First(), typeof(string)),
                Expression.Constant(selector.ElementAtOrDefault(1), typeof(string)));

            for (var i = 1; i < selectors.Length; i++)
            {
                selector = selectors[i].Split(' ');

                resultExpression = Expression.Call(
                    typeof(QueryableExtensions),
                    "ThenBy",
                    [queryable.ElementType],
                    resultExpression,
                    Expression.Constant(selector.First(), typeof(string)),
                    Expression.Constant(selector.ElementAtOrDefault(1), typeof(string)));
            }

            return resultExpression;
        }
        
        static string[] GetSelectors(string sortExpression)
        {
            var match = SortExpressionRegex.Match(sortExpression);

            if (!match.Success)
            {
                throw new ArgumentException($"The '{nameof(SortExpression)}' syntax is invalid.", nameof(sortExpression));
            }

            var hasFlag = !string.IsNullOrEmpty(match.Groups["DirFlag"].Value);
    
            if (hasFlag) 
            {
                sortExpression = match.Groups["SortExpr"].Value;
            }

            var selectors = sortExpression.Split(',')
                                          .Select(x => x.Trim())
                                          .ToArray();

            var primarySelector = match.Groups["PrimarySelector"].Value;
            var hasDir = !string.IsNullOrEmpty(match.Groups["PrimaryDir"].Value);
    
            if (hasFlag)
            {
                selectors[0] = hasDir ? primarySelector : $"{primarySelector} Desc";
            }

            return selectors;
        }
    }

    private IEnumerable TryGetEnumerableData(object dataObject, DataSourceSelectArguments arguments)
    {
        if (FilterExpression.Length > 0)
        {
            throw new NotSupportedException(string.Format(Strings.FilterNotSupported, owner.ID));
        }

        if (SortExpression.Length > 0)
        {
            throw new NotSupportedException(string.Format(Strings.SortNotSupportedOnIEnumerable, owner.ID));
        }

        if (dataObject is IEnumerable enumerable)
        {
            if (arguments.RetrieveTotalRowCount)
            {
                if (enumerable is ICollection collection)
                {
                    arguments.TotalRowCount = collection.Count;
                }
            }

            return enumerable;
        }

        // If the object is not an enumerable object it sends the object back as an enumerable one.

        if (arguments.RetrieveTotalRowCount)
        {
            arguments.TotalRowCount = 1;
        }

        return new[] { dataObject };
    }

    private DataSourceStatusEventArgs UpdateDataSourceStatus(
        DataSourceOperationMethod operationMethod,
        object[] values,
        object returnValue,
        Exception ex = null)
    {
        var outputParameters = GetOutputParameters(operationMethod.Method.GetParameters(), values);

        var e = new DataSourceStatusEventArgs(returnValue, outputParameters, ex);

        switch (operationMethod.OperationType)
        {
            case DataSourceOperation.Delete:
                OnDeleted(e);

                break;
            case DataSourceOperation.Insert:
                OnInserted(e);

                break;
            case DataSourceOperation.Select:
                OnSelected(e);

                break;
            case DataSourceOperation.Update:
                OnUpdated(e);

                break;
            case DataSourceOperation.SelectCount:
                OnSelected(e);

                break;
        }

        return e;
    }

    private readonly struct DataSourceOperationMethod
    {
        public readonly MethodInfo Method;

        public readonly DataSourceOperation OperationType;

        public readonly OrderedDictionary Parameters;

        internal DataSourceOperationMethod(
            DataSourceOperation operationType,
            MethodInfo method,
            OrderedDictionary parameters)
        {
            OperationType = operationType;
            Method = method;
            Parameters = parameters;
        }
    }

    private sealed class DataObjectInfo
    {
        public readonly MethodInfo DataObjectMethod;
        public readonly Type DataObjectType;
        public readonly ParameterInfo MethodFirstParameter;
        public readonly ParameterInfo MethodSecondParameter;
        public readonly DataSourceOperation OperationType;

        internal DataObjectInfo(
            DataSourceOperation operationType,
            Type dataObjectType,
            MethodInfo dataObjectMethod,
            ParameterInfo methodFirstParameter,
            ParameterInfo methodSecondParameter)
        {
            OperationType = operationType;
            DataObjectType = dataObjectType;
            DataObjectMethod = dataObjectMethod;
            MethodFirstParameter = methodFirstParameter;
            MethodSecondParameter = methodSecondParameter;
        }
    }

    private sealed class DataSourceOperationResult
    {
        public readonly int AffectedRows;
        public readonly object ReturnValue;

        internal DataSourceOperationResult(object returnValue, int affectedRows)
        {
            ReturnValue = returnValue;
            AffectedRows = affectedRows;
        }
    }
}