using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using WpfApp.Core;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    /// <summary>
    /// ViewModel для главного окна
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly IDataService _dataService;
        private readonly ILogger<MainViewModel> _logger;

        private ObservableCollection<Product> _products = new();
        private Product? _selectedProduct;
        private string _searchTerm = string.Empty;
        private string _statusMessage = "Готово к работе";
        private int _totalCount;
        private string _searchResultText = string.Empty;

        // Поля формы
        private string _name = string.Empty;
        private string _price = string.Empty;
        private string _quantity = string.Empty;
        private string _description = string.Empty;

        private readonly Dictionary<string, List<string>> _errors = new();

        public MainViewModel(IDataService dataService, ILogger<MainViewModel> logger)
        {
            _dataService = dataService;
            _logger = logger;

            LoadProductsCommand = new RelayCommand(LoadProducts);
            AddProductCommand = new RelayCommand(AddProduct, CanAddOrEdit);
            EditProductCommand = new RelayCommand(EditProduct, CanAddOrEdit);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanDelete);
            ClearFormCommand = new RelayCommand(ClearForm);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            SearchCommand = new RelayCommand<string>(Search);

            LoadProducts(null);
        }

        #region Properties

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (value != null)
                    {
                        Name = value.Name;
                        Price = value.Price.ToString();
                        Quantity = value.Quantity.ToString();
                        Description = value.Description ?? string.Empty;
                    }
                    OnPropertyChanged(nameof(CanDelete));
                }
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public string SearchResultText
        {
            get => _searchResultText;
            set => SetProperty(ref _searchResultText, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    ClearErrors(nameof(Name));
                    ValidateName();
                    OnPropertyChanged(nameof(CanAddOrEdit));
                }
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                {
                    ClearErrors(nameof(Price));
                    ValidatePrice();
                    OnPropertyChanged(nameof(CanAddOrEdit));
                }
            }
        }

        public string Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    ClearErrors(nameof(Quantity));
                    ValidateQuantity();
                    OnPropertyChanged(nameof(CanAddOrEdit));
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        #endregion

        #region Commands

        public RelayCommand LoadProductsCommand { get; }
        public RelayCommand AddProductCommand { get; }
        public RelayCommand EditProductCommand { get; }
        public RelayCommand DeleteProductCommand { get; }
        public RelayCommand ClearFormCommand { get; }
        public RelayCommand ClearSearchCommand { get; }
        public RelayCommand<string> SearchCommand { get; }

        #endregion

        #region Command Handlers

        private void LoadProducts(object? parameter)
        {
            _logger.LogInformation("Загрузка списка товаров");
            Products = new ObservableCollection<Product>(_dataService.GetAllProducts());
            TotalCount = _dataService.GetTotalCount();
            StatusMessage = "Готово к работе";
        }

        private void AddProduct(object? parameter)
        {
            if (!Validate()) return;

            try
            {
                var product = new Product
                {
                    Name = Name.Trim(),
                    Price = decimal.Parse(Price),
                    Quantity = int.Parse(Quantity),
                    Description = Description.Trim()
                };

                _dataService.AddProduct(product);
                _logger.LogInformation("Добавлен товар: {Name}", product.Name);

                LoadProducts(null);
                ClearForm(null);
                StatusMessage = "Товар успешно добавлен";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении товара");
                StatusMessage = "Ошибка при добавлении товара";
            }
        }

        private void EditProduct(object? parameter)
        {
            if (SelectedProduct == null || !Validate()) return;

            try
            {
                SelectedProduct.Name = Name.Trim();
                SelectedProduct.Price = decimal.Parse(Price);
                SelectedProduct.Quantity = int.Parse(Quantity);
                SelectedProduct.Description = Description.Trim();

                _dataService.UpdateProduct(SelectedProduct);
                _logger.LogInformation("Изменён товар: {Name}", SelectedProduct.Name);

                LoadProducts(null);
                ClearForm(null);
                StatusMessage = "Товар успешно изменён";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении товара");
                StatusMessage = "Ошибка при изменении товара";
            }
        }

        private void DeleteProduct(object? parameter)
        {
            if (SelectedProduct == null) return;

            try
            {
                _dataService.DeleteProduct(SelectedProduct);
                _logger.LogInformation("Удалён товар: {Name}", SelectedProduct.Name);

                LoadProducts(null);
                ClearForm(null);
                StatusMessage = "Товар успешно удалён";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении товара");
                StatusMessage = "Ошибка при удалении товара";
            }
        }

        private void ClearForm(object? parameter)
        {
            Name = string.Empty;
            Price = string.Empty;
            Quantity = string.Empty;
            Description = string.Empty;
            SelectedProduct = null;
            ClearAllErrors();
            StatusMessage = "Форма очищена";
        }

        private void ClearSearch(object? parameter)
        {
            SearchTerm = string.Empty;
            SearchResultText = string.Empty;
            LoadProducts(null);
        }

        private void Search(string? searchTerm)
        {
            SearchTerm = searchTerm ?? string.Empty;

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                LoadProducts(null);
                SearchResultText = string.Empty;
                return;
            }

            Products = new ObservableCollection<Product>(_dataService.SearchProducts(SearchTerm));
            SearchResultText = $"Найдено: {Products.Count}";
            StatusMessage = $"Поиск: {SearchTerm}";
            _logger.LogInformation("Поиск товаров по запросу: {Term}", SearchTerm);
        }

        #endregion

        #region CanExecute

        public bool CanAddOrEdit(object? parameter) => Validate() && !HasErrors;

        public bool CanDelete(object? parameter) => SelectedProduct != null;

        #endregion

        #region Validation

        private bool Validate()
        {
            ClearAllErrors();

            ValidateName();
            ValidatePrice();
            ValidateQuantity();

            return !HasErrors;
        }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Название обязательно для заполнения");
            }
            else if (Name.Length > 100)
            {
                AddError(nameof(Name), "Название не должно превышать 100 символов");
            }
        }

        private void ValidatePrice()
        {
            if (string.IsNullOrWhiteSpace(Price))
            {
                AddError(nameof(Price), "Цена обязательна для заполнения");
            }
            else if (!decimal.TryParse(Price, out var price) || price < 0)
            {
                AddError(nameof(Price), "Введите корректную цену (неотрицательное число)");
            }
        }

        private void ValidateQuantity()
        {
            if (string.IsNullOrWhiteSpace(Quantity))
            {
                AddError(nameof(Quantity), "Количество обязательно для заполнения");
            }
            else if (!int.TryParse(Quantity, out var quantity) || quantity < 0)
            {
                AddError(nameof(Quantity), "Введите корректное количество (неотрицательное целое число)");
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
                _errors[propertyName].Add(error);

            OnErrorsChanged(propertyName);
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearAllErrors()
        {
            _errors.Clear();
            OnErrorsChanged(null);
        }

        #endregion

        #region INotifyDataErrorInfo

        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _errors[propertyName];
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        #endregion
    }
}
