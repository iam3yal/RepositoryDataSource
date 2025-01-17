namespace RepositoryDataSource.DataObjects;

public class Context
{
    public CustomerRepository CustomerRepository
        => new CustomerRepository();

    public CustomerRepository GetCustomerRepository()
    {
        return new CustomerRepository();
    }
}