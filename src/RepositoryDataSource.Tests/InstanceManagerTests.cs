namespace RepositoryDataSource;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Aspx.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
[SuppressMessage("Usage", "MSTEST0002:Test classes should have valid layout")]
[SuppressMessage("Usage", "MSTEST0003:Test methods should have valid layout")]
public static class InstanceManagerTests
{
    [TestMethod]
    public static void CreateInstance_ResolveInstanceFromTypeWithInvalidTarget_ThrowInvalidOperationException()
    {
        var target = GetTarget("Unknown");

        var manager = CreateInstanceManager(target);
            
        Assert.ThrowsException<InvalidOperationException>(() => manager.CreateInstance());
    }

    [TestMethod]
    public static void CreateInstance_ResolveInstanceFromTypeWithValidTarget_ReturnObject()
    {
        var target = GetTarget("CustomerRepository");

        var manager = CreateInstanceManager(target);

        var instance = manager.CreateInstance();

        Assert.IsNotNull(instance);
    }

    [TestMethod]
    public static void CreateInstance_ResolveInstanceFromMethodWithValidTarget_ReturnObject()
    {
        var target = GetTarget("Context.GetCustomerRepository");

        var manager = CreateInstanceManager(target);

        var instance = manager.CreateInstance();

        Assert.IsNotNull(instance);
    }

    [TestMethod]
    public static void CreateInstance_ResolveInstanceFromPropertyWithValidTarget_ReturnObject()
    {
        var target = GetTarget("Context.CustomerRepository");

        var manager = CreateInstanceManager(target);

        var instance = manager.CreateInstance();

        Assert.IsNotNull(instance);
    }

    private static InstanceManager CreateInstanceManager(string target)
    {
        var cache = new Dictionary<object, object>();

        var mock = new Mock<IRepositoryDataSource>();

        mock.SetupProperty(dataSource => dataSource.ID, "DataSource1");
        mock.SetupProperty(dataSource => dataSource.Target, target);

        return new InstanceManager(mock.Object, cache);
    }

    private static string GetTarget(string name)
        => $"{typeof(InstanceManagerTests).Namespace}.{name}";
}