namespace Aspx.WebControls;

using System;
using System.ComponentModel;
using System.Web.UI;
    
internal static class DataSourceOperationExtensions
{
    public static string GetLocalizedName(this DataSourceOperation operation)
        => operation switch {
            DataSourceOperation.Insert => Strings.Insert, 
            DataSourceOperation.Update => Strings.Update, 
            DataSourceOperation.Delete => Strings.Delete, 
            DataSourceOperation.Select => Strings.Select, 
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

    public static DataObjectMethodType GetMethodType(this DataSourceOperation operation)
        => operation switch {
            DataSourceOperation.Delete => DataObjectMethodType.Delete, 
            DataSourceOperation.Insert => DataObjectMethodType.Insert, 
            DataSourceOperation.Select => DataObjectMethodType.Select, 
            DataSourceOperation.Update => DataObjectMethodType.Update, 
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

    public static string GetPropertyName(this DataSourceOperation operation)
    {
        return operation switch {
            DataSourceOperation.Insert => "InsertMethod", 
            DataSourceOperation.Update => "UpdateMethod", 
            DataSourceOperation.Delete => "DeleteMethod", 
            DataSourceOperation.Select => "SelectMethod", 
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
    }
}