namespace RepositoryDataSource.DataObjects;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class CustomerRepository
{
    private readonly List<Customer> _customers = [
        new() { ID = 11, Name = "Alice Carter" },
        new() { ID = 22, Name = "Bob Johnson" },
        new() { ID = 33, Name = "Charlie Davis" },
        new() { ID = 44, Name = "Diana Evans" },
        new() { ID = 55, Name = "Ethan Moore" },
        new() { ID = 66, Name = "Fiona Adams" },
        new() { ID = 77, Name = "George Brooks" }
    ];

    public Customer GetByID(int id)
    {
        var customer = _customers.FirstOrDefault(customers => customers.ID == id);

        return customer;
    }

    public IList<Customer> GetAll()
    {
        return _customers;
    }
}