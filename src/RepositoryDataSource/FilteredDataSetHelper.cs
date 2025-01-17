namespace Aspx.WebControls;

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.UI;
    
internal static class FilteredDataSetHelper
{
    public static DataView CreateFilteredDataView(
        DataTable table,
        string sortExpression,
        string filterExpression,
        IDictionary filterParameters)
    {
        //Contract.Requires<ArgumentNullException>(table != null);

        var dataView = new DataView(table);

        if (!string.IsNullOrEmpty(sortExpression))
        {
            dataView.Sort = sortExpression;
        }

        if (!string.IsNullOrEmpty(filterExpression))
        {
            var invalidParameter = false;
            var args = new object[filterParameters.Count];
            var index = 0;

            foreach (DictionaryEntry parameter in filterParameters)
            {
                if (parameter.Value == null)
                {
                    invalidParameter = true;

                    break;
                }

                args[index] = parameter.Value;
                
                ++index;
            }

            //Contract.Assume(filterExpression != null, "'filterExpression' already been tested for a null value so it should never happen");

            filterExpression = string.Format(CultureInfo.InvariantCulture, filterExpression, args);

            if (!invalidParameter)
            {
                dataView.RowFilter = filterExpression;
            }
        }

        return dataView;
    }

    public static DataTable GetDataTable(Control owner, object dataObject)
    {
        //Contract.Requires<ArgumentNullException>(owner != null);

        var dataTable = dataObject as DataTable;

        if (dataTable != null)
        {
            return dataTable;
        }

        switch (dataObject)
        {
            case DataSet { Tables.Count: 0 }:
                throw new InvalidOperationException(string.Format(Strings.DataSetHasNoTables, owner.ID));
            case DataSet dataSet:
                dataTable = dataSet.Tables[0];

                break;
        }

        return dataTable;
    }
}