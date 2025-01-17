namespace Aspx.WebControls;

using System.Web.UI;
    
public sealed partial class RepositoryDataSourceView : IStateManager
{
    private bool IsTrackingViewState { get; set; }

    private IStateManager FilterParametersStateManager
        => FilterParameters;

    private IStateManager SelectParametersStateManager
        => SelectParameters;

    bool IStateManager.IsTrackingViewState
        => IsTrackingViewState;

    void IStateManager.LoadViewState(object state)
        => LoadViewState(state);

    object IStateManager.SaveViewState()
        => SaveViewState();

    void IStateManager.TrackViewState()
        => TrackViewState();

    private void LoadViewState(object savedState)
    {
        if (savedState == null)
        {
            return;
        }

        var pair = (Pair)savedState;

        if (pair.First != null)
        {
            SelectParametersStateManager.LoadViewState(pair.First);
        }

        if (pair.Second != null)
        {
            FilterParametersStateManager.LoadViewState(pair.Second);
        }
    }

    private object SaveViewState()
    {
        var pair = new Pair {
            First = SelectParameters != null ? SelectParametersStateManager.SaveViewState() : null,
            Second = FilterParameters != null ? FilterParametersStateManager.SaveViewState() : null
        };

        return pair.First == null && pair.Second == null ? null : pair;
    }

    private void TrackViewState()
    {
        IsTrackingViewState = true;

        if (SelectParameters != null)
        {
            SelectParametersStateManager.TrackViewState();
        }

        if (FilterParameters != null)
        {
            FilterParametersStateManager.TrackViewState();
        }
    }
}