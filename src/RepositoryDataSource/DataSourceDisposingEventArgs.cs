namespace Aspx.WebControls;

using System.ComponentModel;

public class DataSourceDisposingEventArgs(object objectInstance) : CancelEventArgs
{
    public object ObjectInstance { get; } = objectInstance;
}