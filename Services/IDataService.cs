using System.Collections.Generic;
using WpfApp.Models;

namespace WpfApp.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с данными
    /// </summary>
    public interface IDataService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> SearchProducts(string searchTerm);
        Product? GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
        int GetTotalCount();
    }
}
