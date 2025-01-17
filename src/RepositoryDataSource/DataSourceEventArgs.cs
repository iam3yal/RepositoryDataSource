namespace Aspx.WebControls;

using System;

public class DataSourceEventArgs(object objectInstance) : EventArgs
{
    public object ObjectInstance { get; set; } = objectInstance;
}