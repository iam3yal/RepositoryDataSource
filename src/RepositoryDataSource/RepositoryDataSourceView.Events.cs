namespace Aspx.WebControls;

using System;

public sealed partial class RepositoryDataSourceView
{
    private static readonly object EventDeleted = new();

    private static readonly object EventDeleting = new();

    private static readonly object EventFiltering = new();

    private static readonly object EventInserted = new();

    private static readonly object EventInserting = new();

    private static readonly object EventSelected = new();

    private static readonly object EventSelecting = new();

    private static readonly object EventUpdated = new();

    private static readonly object EventUpdating = new();

    public event EventHandler<DataSourceStatusEventArgs> Deleted
    {
        add => Events.AddHandler(EventDeleted, value);
        remove => Events.RemoveHandler(EventDeleted, value);
    }

    public event EventHandler<DataSourceMethodEventArgs> Deleting
    {
        add => Events.AddHandler(EventDeleting, value);
        remove => Events.RemoveHandler(EventDeleting, value);
    }

    public event EventHandler<DataSourceFilteringEventArgs> Filtering
    {
        add => Events.AddHandler(EventFiltering, value);
        remove => Events.RemoveHandler(EventFiltering, value);
    }

    public event EventHandler<DataSourceStatusEventArgs> Inserted
    {
        add => Events.AddHandler(EventInserted, value);
        remove => Events.RemoveHandler(EventInserted, value);
    }

    public event EventHandler<DataSourceMethodEventArgs> Inserting
    {
        add => Events.AddHandler(EventInserting, value);
        remove => Events.RemoveHandler(EventInserting, value);
    }

    public event EventHandler<DataSourceEventArgs> ObjectCreated
    {
        add => InstanceManager.ObjectCreated += value;
        remove => InstanceManager.ObjectCreated -= value;
    }

    public event EventHandler<DataSourceEventArgs> ObjectCreating
    {
        add => InstanceManager.ObjectCreating += value;
        remove => InstanceManager.ObjectCreating -= value;
    }

    public event EventHandler<DataSourceDisposingEventArgs> ObjectDisposing
    {
        add => InstanceManager.ObjectDisposing += value;
        remove => InstanceManager.ObjectDisposing -= value;
    }

    public event EventHandler<DataSourceStatusEventArgs> Selected
    {
        add => Events.AddHandler(EventSelected, value);
        remove => Events.RemoveHandler(EventSelected, value);
    }

    public event EventHandler<DataSourceSelectingEventArgs> Selecting
    {
        add => Events.AddHandler(EventSelecting, value);
        remove => Events.RemoveHandler(EventSelecting, value);
    }

    public event EventHandler<DataSourceStatusEventArgs> Updated
    {
        add => Events.AddHandler(EventUpdated, value);
        remove => Events.RemoveHandler(EventUpdated, value);
    }

    public event EventHandler<DataSourceMethodEventArgs> Updating
    {
        add => Events.AddHandler(EventUpdating, value);
        remove => Events.RemoveHandler(EventUpdating, value);
    }

    private void OnDeleted(DataSourceStatusEventArgs e)
    {
        if (Events[EventDeleted] is EventHandler<DataSourceStatusEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnDeleting(DataSourceMethodEventArgs e)
    {
        if (Events[EventDeleting] is EventHandler<DataSourceMethodEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnFiltering(DataSourceFilteringEventArgs e)
    {
        if (Events[EventFiltering] is EventHandler<DataSourceFilteringEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnInserted(DataSourceStatusEventArgs e)
    {
        if (Events[EventInserted] is EventHandler<DataSourceStatusEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnInserting(DataSourceMethodEventArgs e)
    {
        if (Events[EventInserting] is EventHandler<DataSourceMethodEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnSelected(DataSourceStatusEventArgs e)
    {
        if (Events[EventSelected] is EventHandler<DataSourceStatusEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnSelecting(DataSourceSelectingEventArgs e)
    {
        if (Events[EventSelecting] is EventHandler<DataSourceSelectingEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnUpdated(DataSourceStatusEventArgs e)
    {
        if (Events[EventUpdated] is EventHandler<DataSourceStatusEventArgs> handler)
        {
            handler(this, e);
        }
    }

    private void OnUpdating(DataSourceMethodEventArgs e)
    {
        if (Events[EventUpdating] is EventHandler<DataSourceMethodEventArgs> handler)
        {
            handler(this, e);
        }
    }
}