namespace Aspx.WebControls;

internal static class Strings
{
    public const string InvalidViewName = "The data source '{0}' only supports a single view named '{1}'. You may also leave the view name (also called a data member) empty for the default view to be chosen.";

    public const string DataSetHasNoTables = "The DataSet in data source '{0}' does not contain any tables.";

    public const string CannotConvertType = "Cannot convert value of parameter '{0}' from '{1}' to '{2}'.";

    public const string CannotPerformPaging = "The data source '{0}' cannot perform paging either because the SortExpression property is not specified or invalid.";

    public const string DataObjectMethodNoParams = "The {0} operation on data source '{1}' requires method '{2}' to have at least one parameter available at the start of the method, any additional parameters should come next to it.";

    public const string DataObjectMethodNotFound = "The data source '{0}' could not find a method named '{1}'.";

    public const string DataObjectMethodOverloadingNotSupported = "The data source '{0}' found more than one method with the name '{1}'. Method overloading is currently not supported.";

    public const string DataObjectMethodRequiresAdditionalParam = "The data source '{0}' requires the method that was specified in the {1} property to have two parameters of type '{2}' available at the start of the method, any additional parameters should come next to it.";

    public const string DataObjectPropertyNotFound = "Could not find a property named '{0}' on type '{1}' in data source '{2}'.";

    public const string DataObjectPropertyReadOnly = "The property '{0}' on type '{1}' in data source '{2}' is readonly and its value cannot be set.";

    public const string DeleteNotSupported = "Deleting is not supported by data source '{0}' unless the DeleteMethod is specified.";

    public const string FilterNotSupported = "The data source '{0}' only supports filtering when the SelectMethod returns a DataSet or a DataTable.";

    public const string InsertNotSupported = "Inserting is not supported by data source '{0}' unless the InsertMethod is specified.";

    public const string InsertRequiresValues = "The data source '{0}' has no values to insert. Check that the 'values' dictionary contains values.";

    public const string MethodNotFoundNoParams = "The data source '{0}' could not find a non-generic method '{1}' that has no parameters.";

    public const string MethodNotFoundWithParams = "The data source '{0}' could not find a non-generic method '{1}' that has parameters: '{2}'.";

    public const string MissingCommitMethod = "The data source '{0}' could not infer a method with no parameters that is one of the following names Commit, Store or Save on type '{1}'. Use the CommitMethod property to provide it manually.";

    public const string MultipleOverloads = "More than one method with the specified name and parameters was found for data source '{0}'. Adding the DataObjectMethodAttribute to one of these methods and/or making it the default method can help resolve overload conflicts.";

    public const string OptimisticUpdateMethodRequiresAdditionalParam = "ConflictOptions is set to CompareAllValues on data source '{0}', the method specified in the UpdateMethod property must have two parameters of type '{1}' available at the start of the method, any additional parameters should come next to it.";

    public const string PagingNotSupportedOnIEnumerable  = "The data source '{0}' does not support paging with IEnumerable data. Automatic paging is only supported by IQueryable.";

    public const string Pessimistic = "You have specified that your {0} method compares all values on data source '{1}', but the dictionary passed in for '{2}' is empty.  Pass in a valid dictionary for {0} or change your mode to OverwriteChanges.";

    public const string SelectNotSupported = "The Select operation is not supported by data source '{0}' unless the SelectMethod is specified.";

    public const string SortNotSupportedOnIEnumerable = "The data source '{0}' does not support sorting with IEnumerable data. Automatic sorting is only supported with DataView, DataTable, DataSet and IQueryable.";

    public const string TargetMethodNotFound = "The method or property specified in the Target property of the data source '{0}' could not be found.";

    public const string TargetTypeNotFound = "The type specified in the Target property of data source '{0}' could not be found.";

    public const string TargetTypeNotSpecified = "A type must be specified in the Target property of data source '{0}'.";

    public const string TypeAmbiguity = "The type '{0}' is ambiguous. Multiple types are found within assemblies '{1} and {2}'.";

    public const string UpdateNotSupported = "Updating is not supported by data source '{0}' unless the UpdateMethod is specified.";
    
    public const string Insert = "insert";
    
    public const string Select = "select";
    
    public const string Update = "update";
    
    public const string Delete = "delete";
}