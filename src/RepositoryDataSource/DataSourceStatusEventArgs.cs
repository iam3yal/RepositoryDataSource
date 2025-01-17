namespace Aspx.WebControls;

using System;
using System.Collections;

public class DataSourceStatusEventArgs(object returnValue, IDictionary outputParameters, Exception exception)
    : EventArgs
{
    public int AffectedRows { get; set; }

    public Exception Exception { get; } = exception;

    public bool ExceptionHandled { get; set; }

    public IDictionary OutputParameters { get; } = outputParameters;

    public object ReturnValue { get; } = returnValue;
}