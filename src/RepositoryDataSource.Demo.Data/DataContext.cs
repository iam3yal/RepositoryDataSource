namespace RepositoryDataSource.Demo.Data;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Models;
using Repositories;

public class DataContext : IDisposable
{
    private readonly List<Product> _products = [
        new() { ID = 1, Stock = 0, Name = "Laptop" },
        new() { ID = 2, Stock = 30, Name = "Smartphone" },
        new() { ID = 3, Stock = 0, Name = "Headphones" },
        new() { ID = 4, Stock = 10, Name = "Monitor" },
        new() { ID = 5, Stock = 75, Name = "Keyboard" },
        new() { ID = 6, Stock = 0, Name = "Mouse" },
        new() { ID = 7, Stock = 5, Name = "Tablet" },
        new() { ID = 8, Stock = 0, Name = "USB Drive" },
        new() { ID = 9, Stock = 15, Name = "External Hard Drive" },
        new() { ID = 10, Stock = 60, Name = "Printer" }
    ];

    private ProductRepository _productRepository;

    public virtual IProductRepository ProductRepository
        => _productRepository ??= new ProductRepository(_products);

    public virtual void Dispose()
    {
        Debug.WriteLine($"{nameof(DataContext)}.{nameof(Dispose)} is called.");
    }

    public virtual int Commit()
    {
        Debug.WriteLine($"{nameof(DataContext)}.{nameof(Commit)} is called.");

        return 0;
    }
}