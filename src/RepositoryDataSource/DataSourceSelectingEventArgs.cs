namespace Aspx.WebControls;

using System.Collections.Specialized;
using System.Web.UI;
    
public class DataSourceSelectingEventArgs(
    IOrderedDictionary inputParameters,
    DataSourceSelectArguments arguments,
    bool executingSelectCount) 
    : DataSourceMethodEventArgs(inputParameters)
{
    public DataSourceSelectArguments Arguments { get; } = arguments;

    public bool ExecutingSelectCount { get; } = executingSelectCount;
}