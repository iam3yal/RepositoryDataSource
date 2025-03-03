namespace Aspx.WebControls;

using System;
using System.Collections;
using System.Reflection;
    
// #todo: The objects that are created by the InstanceManager should be cached per request based on the type
// #todo: Add missing documentation

public sealed class InstanceManager(IRepositoryDataSource owner, IDictionary cache)
{
    private const BindingFlags _publicMembersFlags = BindingFlags.Instance
                                                    | BindingFlags.Static
                                                    | BindingFlags.Public
                                                    | BindingFlags.FlattenHierarchy;

    private readonly string _contextString = $"__RepositoryDataSource_{owner.ID}";

    private object _targetObject;

    public event EventHandler<DataSourceEventArgs> ObjectCreated;

    public event EventHandler<DataSourceEventArgs> ObjectCreating;

    public event EventHandler<DataSourceDisposingEventArgs> ObjectDisposing;

    public object CreateInstance()
    {
        var instance = GetInstanceFromCache();

        if (instance != null)
        {
            return instance;
        }

        var target = owner.Target;

        var type = GetTypeFromTarget(target);

        // If the target's fullname is equal to the target we specified it means that there's no method to invoke.
        if (type.FullName != target)
        {
            var index = target.LastIndexOf('.') + 1;

            var methodName = target.Substring(index);

            var property = type.GetProperty(methodName, _publicMembersFlags);

            var method = property != null ? property.GetGetMethod() : type.GetMethod(methodName, _publicMembersFlags);

            if (method == null)
            {
                throw new InvalidOperationException(string.Format(Strings.TargetMethodNotFound, owner.ID));
            }

            _targetObject = null;

            if (!method.IsStatic)
            {
                _targetObject = CreateInstance(type);
            }

            instance = method.Invoke(_targetObject, null);
        }
        else
        {
            instance = CreateInstance(type);

            _targetObject = instance;
        }

        StoreInstanceToCache(instance);

        return instance;
    }

    public MethodInfo[] GetMethods()
        => CreateInstance().GetType().GetMethods(_publicMembersFlags);

    public object GetTargetObject()
    {
        if (_targetObject == null)
        {
            CreateInstance();
        }

        return _targetObject;
    }

    public void ReleaseInstance(bool disposeTargetObject = false)
    {
        var instance = GetInstanceFromCache();

        if (disposeTargetObject && _targetObject != null && instance != _targetObject)
        {
            if (!ReleaseInstance(_targetObject))
            {
                return;
            }

            _targetObject = null;
        }

        if (instance == null || !ReleaseInstance(instance))
        {
            return;
        }

        RemoveInstanceFromCache();
    }

    private Type GetTypeFromTarget(string target)
    {
        var length = target.Length;

        if (length == 0)
        {
            throw new InvalidOperationException(string.Format(Strings.TargetTypeNotSpecified, owner.ID));
        }

        var type = TypeHelper.GetType(target);

        if (type == null)
        {
            var index = target.LastIndexOf('.');

            if (index == -1)
            {
                throw new InvalidOperationException(string.Format(Strings.TargetTypeNotFound, owner.ID));
            }

            target = target.Substring(0, index);

            type = TypeHelper.GetType(target);

            if (type == null)
            {
                throw new InvalidOperationException(string.Format(Strings.TargetTypeNotFound, owner.ID));
            }
        }

        return type;
    }

    private void OnObjectCreated(DataSourceEventArgs e)
    {
        var handler = ObjectCreated;

        handler?.Invoke(this, e);
    }

    private void OnObjectCreating(DataSourceEventArgs e)
    {
        var handler = ObjectCreating;

        handler?.Invoke(this, e);
    }

    private void OnObjectDisposing(DataSourceDisposingEventArgs e)
    {
        var handler = ObjectDisposing;

        handler?.Invoke(this, e);
    }

    private object CreateInstance(Type type)
    {
        //Contract.Requires<ArgumentNullException>(type != null);

        object instance;

        var e = new DataSourceEventArgs(null);

        OnObjectCreating(e);

        if (e.ObjectInstance == null)
        {
            instance = Activator.CreateInstance(type);

            e.ObjectInstance = instance;

            OnObjectCreated(e);
        }
        else
        {
            instance = e.ObjectInstance;
        }

        return instance;
    }

    private object GetInstanceFromCache()
        => cache[_contextString];

    private bool ReleaseInstance(object objectToDispose)
    {
        var e = new DataSourceDisposingEventArgs(objectToDispose);

        OnObjectDisposing(e);

        if (e.Cancel)
        {
            return false;
        }

        if (objectToDispose is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return true;
    }

    private void RemoveInstanceFromCache()
        => cache.Remove(_contextString);

    private void StoreInstanceToCache(object instance)
        => cache.Add(_contextString, instance);
}