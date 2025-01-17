namespace Aspx.WebControls;

using System.Collections.Specialized;
using System.ComponentModel;
    
public class DataSourceFilteringEventArgs(IOrderedDictionary parameterValues) : CancelEventArgs
{
    public IOrderedDictionary ParameterValues { get; } = parameterValues;
}