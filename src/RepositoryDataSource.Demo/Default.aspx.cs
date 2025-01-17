using System;
using System.Web.UI;
using Aspx.WebControls;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void DataSource_DataBinding(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_DataBinding");
    }

    protected void DataSource_Deleted(object sender, DataSourceStatusEventArgs e)
    {
        Trace.Write("Events", "DataSource_Deleted");
    }

    protected void DataSource_Deleting(object sender, DataSourceMethodEventArgs e)
    {
        Trace.Write("Events", "DataSource_Deleting");
    }

    protected void DataSource_Disposed(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_Disposed");
    }

    protected void DataSource_Init(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_Init");
    }

    protected void DataSource_Inserted(object sender, DataSourceStatusEventArgs e)
    {
        Trace.Write("Events", "DataSource_Inserted");
    }

    protected void DataSource_Inserting(object sender, DataSourceMethodEventArgs e)
    {
        Trace.Write("Events", "DataSource_Inserting");
    }

    protected void DataSource_Load(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_Load");
    }

    protected void DataSource_ObjectCreated(object sender, DataSourceEventArgs e)
    {
        Trace.Write("Events", "DataSource_ObjectCreated");
    }

    protected void DataSource_ObjectCreating(object sender, DataSourceEventArgs e)
    {
        Trace.Write("Events", "DataSource_ObjectCreating");
    }

    protected void DataSource_ObjectDisposing(object sender, DataSourceDisposingEventArgs e)
    {
        Trace.Write("Events", "DataSource_ObjectDisposing");
    }

    protected void DataSource_PreRender(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_PreRender");
    }

    protected void DataSource_Selected(object sender, DataSourceStatusEventArgs e)
    {
        Trace.Write("Events", "DataSource_Selected");
    }

    protected void DataSource_Selecting(object sender, DataSourceSelectingEventArgs e)
    {
        Trace.Write("Events", "DataSource_Selecting");
    }

    protected void DataSource_Unload(object sender, EventArgs e)
    {
        Trace.Write("Events", "DataSource_Unload");
    }

    protected void DataSource_Updated(object sender, DataSourceStatusEventArgs e)
    {
        Trace.Write("Events", "DataSource_Updated");
    }

    protected void DataSource_Updating(object sender, DataSourceMethodEventArgs e)
    {
        Trace.Write("Events", "DataSource_Updating");
    }
}