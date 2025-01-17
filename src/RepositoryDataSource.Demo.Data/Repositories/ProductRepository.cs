using System.Collections.Generic;
using System.Linq;
using RepositoryDataSource.Demo.Data.Models;

namespace RepositoryDataSource.Demo.Data.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAll();

        void Update(Product product1, Product product2);

        void Insert(Product product);

        void Delete(Product product);
    }

    internal class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public ProductRepository(List<Product> products) 
            => _products = products;

        public IQueryable<Product> GetAll() 
            => _products.AsQueryable();

        public void Update(Product product1, Product product2) 
            => _products[_products.IndexOf(product1)] = product2;

        public void Insert(Product product) 
            => _products.Add(product);

        public void Delete(Product product) 
            => _products.Remove(product);
    }
}