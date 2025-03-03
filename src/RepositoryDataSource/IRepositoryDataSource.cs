namespace Aspx.WebControls;

using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using System.Web.UI.WebControls;
    
public interface IRepositoryDataSource : IDataSource
{
    string CommitMethod { get; set; }

    string DeleteMethod { get; set; }

    ParameterCollection DeleteParameters { get; }

    bool EnablePaging { get; set; }

    string FilterExpression { get; set; }

    ParameterCollection FilterParameters { get; }

    string ID { get; set; }

    string InsertMethod { get; set; }

    ParameterCollection InsertParameters { get; }

    string SelectMethod { get; set; }

    ParameterCollection SelectParameters { get; }

    string SortExpression { get; set; }

    string Target { get; set; }

    string UpdateMethod { get; set; }

    ParameterCollection UpdateParameters { get; }
}