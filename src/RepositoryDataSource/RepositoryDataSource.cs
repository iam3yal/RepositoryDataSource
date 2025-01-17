namespace Aspx.WebControls;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
    
// #todo: Add missing documentation

/// <summary>
///  Represents an object that provides data to data-bound controls in ASP.NET application.
/// </summary>
[DefaultProperty("Target")]
[DefaultEvent("Selecting")]
[ParseChildren(true)]
[PersistChildren(false)]
[ToolboxData("<{0}:RepositoryDataSource runat=server></{0}:RepositoryDataSource>")]
public class RepositoryDataSource : DataSourceControl, IRepositoryDataSource
{
    private const string DefaultViewName = "DefaultPlainView";

    private RepositoryDataSourceView _view;

    private ICollection _viewNames;

    [DefaultValue(ConflictOptions.OverwriteChanges)]
    public ConflictOptions ConflictDetection
    {
        get => GetView().ConflictDetection;
        set => GetView().ConflictDetection = value;
    }

    [DefaultValue(false)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public bool ConvertNullToDBNull
    {
        get => GetView().ConvertNullToDBNull;
        set => GetView().ConvertNullToDBNull = value;
    }

    public string CommitMethod
    {
        get => GetView().CommitMethod;
        set => GetView().CommitMethod = value;
    }

    [DefaultValue("")]
    public string DeleteMethod
    {
        get => GetView().DeleteMethod;
        set => GetView().DeleteMethod = value;
    }

    [MergableProperty(false)]
    [DefaultValue(null)]
    [Editor(
        "System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(UITypeEditor))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ParameterCollection DeleteParameters
        => GetView().DeleteParameters;

    public bool EnablePaging
    {
        get => GetView().EnablePaging;
        set => GetView().EnablePaging = value;
    }

    [DefaultValue("")]
    public string FilterExpression
    {
        get => GetView().FilterExpression;
        set => GetView().FilterExpression = value;
    }

    [DefaultValue(null)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Editor(
        "System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(UITypeEditor))]
    [MergableProperty(false)]
    public ParameterCollection FilterParameters
        => GetView().FilterParameters;

    [DefaultValue("")]
    public string InsertMethod
    {
        get => GetView().InsertMethod;
        set => GetView().InsertMethod = value;
    }

    [MergableProperty(false)]
    [DefaultValue(null)]
    [Editor(
        "System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(UITypeEditor))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ParameterCollection InsertParameters
        => GetView().InsertParameters;

    [DefaultValue("")]
    public string SelectMethod
    {
        get => GetView().SelectMethod;
        set => GetView().SelectMethod = value;
    }

    [DefaultValue(null)]
    [Editor(
        "System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(UITypeEditor))]
    [MergableProperty(false)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ParameterCollection SelectParameters
        => GetView().SelectParameters;

    public string SortExpression
    {
        get => GetView().SortExpression;
        set => GetView().SortExpression = value;
    }

    [DefaultValue("")]
    public string Target
    {
        get => GetView().Target;
        set => GetView().Target = value;
    }

    [DefaultValue("")]
    public string UpdateMethod
    {
        get => GetView().UpdateMethod;
        set => GetView().UpdateMethod = value;
    }

    [DefaultValue(null)]
    [Editor(
        "System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(UITypeEditor))]
    [MergableProperty(false)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ParameterCollection UpdateParameters
        => GetView().UpdateParameters;

    public event EventHandler<DataSourceStatusEventArgs> Deleted
    {
        add => GetView().Deleted += value;
        remove => GetView().Deleted -= value;
    }

    public event EventHandler<DataSourceMethodEventArgs> Deleting
    {
        add => GetView().Deleting += value;
        remove => GetView().Deleting -= value;
    }

    public event EventHandler<DataSourceFilteringEventArgs> Filtering
    {
        add => GetView().Filtering += value;
        remove => GetView().Filtering -= value;
    }

    public event EventHandler<DataSourceStatusEventArgs> Inserted
    {
        add => GetView().Inserted += value;
        remove => GetView().Inserted -= value;
    }

    public event EventHandler<DataSourceMethodEventArgs> Inserting
    {
        add => GetView().Inserting += value;
        remove => GetView().Inserting -= value;
    }

    public event EventHandler<DataSourceEventArgs> ObjectCreated
    {
        add => GetView().ObjectCreated += value;
        remove => GetView().ObjectCreated -= value;
    }

    public event EventHandler<DataSourceEventArgs> ObjectCreating
    {
        add => GetView().ObjectCreating += value;
        remove => GetView().ObjectCreating -= value;
    }

    public event EventHandler<DataSourceDisposingEventArgs> ObjectDisposing
    {
        add => GetView().ObjectDisposing += value;
        remove => GetView().ObjectDisposing -= value;
    }

    public event EventHandler<DataSourceStatusEventArgs> Selected
    {
        add => GetView().Selected += value;
        remove => GetView().Selected -= value;
    }

    public event EventHandler<DataSourceSelectingEventArgs> Selecting
    {
        add => GetView().Selecting += value;
        remove => GetView().Selecting -= value;
    }

    public event EventHandler<DataSourceStatusEventArgs> Updated
    {
        add => GetView().Updated += value;
        remove => GetView().Updated -= value;
    }

    public event EventHandler<DataSourceMethodEventArgs> Updating
    {
        add => GetView().Updating += value;
        remove => GetView().Updating -= value;
    }

    protected virtual RepositoryDataSourceView GetView()
    {
        if (_view == null)
        {
            _view = new RepositoryDataSourceView(this, DefaultViewName, Context);

            if (IsTrackingViewState)
            {
                ((IStateManager)_view).TrackViewState();
            }
        }

        return _view;
    }

    /// <summary>
    ///  Gets the named data source view associated with the data source control.
    /// </summary>
    /// <returns>
    ///  Returns the named <see cref="T:System.Web.UI.DataSourceView"/> associated with the <see cref="T:System.Web.UI.DataSourceControl"/>.
    /// </returns>
    /// <param name="viewName">The name of the <see cref="T:System.Web.UI.DataSourceView"/> to retrieve. In data source controls that support only one view, such as <see cref="T:System.Web.UI.WebControls.SqlDataSource"/>, this parameter is ignored. </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected sealed override DataSourceView GetView(string viewName)
    {
        var isNameEqual = string.Equals(viewName, DefaultViewName, StringComparison.OrdinalIgnoreCase);

        if (viewName is not { Length: 0 } && !isNameEqual)
        {
            throw new ArgumentException(string.Format(Strings.InvalidViewName, ID, DefaultViewName), nameof(viewName));
        }

        return GetView();
    }

    protected override ICollection GetViewNames()
        => _viewNames ??= new[] { DefaultViewName };
}