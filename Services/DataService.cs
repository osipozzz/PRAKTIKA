using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WpfApp.Data;
using WpfApp.Models;

namespace WpfApp.Services
{
    /// <summary>
    /// Сервис для работы с данными через Entity Framework
    /// </summary>
    public class DataService : IDataService
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public DataService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            using var context = new AppDbContext(_options);
            return context.Products.ToList();
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            using var context = new AppDbContext(_options);
            var term = searchTerm.ToLower();
            return context.Products
                .Where(p => p.Name.ToLower().Contains(term) ||
                           (p.Description != null && p.Description.ToLower().Contains(term)))
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            using var context = new AppDbContext(_options);
            return context.Products.Find(id);
        }

        public void AddProduct(Product product)
        {
            using var context = new AppDbContext(_options);
            context.Products.Add(product);
            context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            using var context = new AppDbContext(_options);
            context.Products.Update(product);
            context.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            using var context = new AppDbContext(_options);
            context.Products.Remove(product);
            context.SaveChanges();
        }

        public int GetTotalCount()
        {
            using var context = new AppDbContext(_options);
            return context.Products.Count();
        }
    }
}
