namespace Aspx.WebControls;

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
    
public sealed partial class RepositoryDataSourceView
{
    private ParameterCollection _deleteParameters;

    private ParameterCollection _filterParameters;

    private ParameterCollection _insertParameters;

    private ParameterCollection _selectParameters;

    private ParameterCollection _updateParameters;

    public ParameterCollection DeleteParameters
        => _deleteParameters ??= new ParameterCollection();

    public ParameterCollection FilterParameters
    {
        get {
            if (_filterParameters == null)
            {
                _filterParameters = new ParameterCollection();

                _filterParameters.ParametersChanged += SelectParameters_ParametersChanged;

                if (IsTrackingViewState)
                {
                    ((IStateManager)_filterParameters).TrackViewState();
                }
            }

            return _filterParameters;
        }
    }

    public ParameterCollection InsertParameters
        => _insertParameters ??= new ParameterCollection();

    public ParameterCollection SelectParameters
    {
        get {
            if (_selectParameters == null)
            {
                _selectParameters = new ParameterCollection();

                _selectParameters.ParametersChanged += SelectParameters_ParametersChanged;

                if (IsTrackingViewState)
                {
                    ((IStateManager)_selectParameters).TrackViewState();
                }
            }

            return _selectParameters;
        }
    }

    public ParameterCollection UpdateParameters
        => _updateParameters ??= new ParameterCollection();

    private void SelectParameters_ParametersChanged(object sender, EventArgs e)
    {
        OnDataSourceViewChanged(EventArgs.Empty);
    }
}