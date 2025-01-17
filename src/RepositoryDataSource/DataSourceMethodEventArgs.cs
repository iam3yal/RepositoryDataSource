namespace Aspx.WebControls;

using System.Collections.Specialized;
using System.ComponentModel;
    
public class DataSourceMethodEventArgs(IOrderedDictionary inputParameters) : CancelEventArgs
{
    public IOrderedDictionary InputParameters { get; } = inputParameters;
}