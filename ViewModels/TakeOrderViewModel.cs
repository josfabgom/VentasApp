using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;
using VentasApp.Views;

namespace VentasApp.ViewModels
{
    public partial class TakeOrderViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Product> AvailableProducts { get; } = new();
        public ObservableCollection<Product> SearchResults { get; } = new();
        public ObservableCollection<Customer> AvailableCustomers { get; } = new();
        public ObservableCollection<Vendedor> AvailableVendedores { get; } = new();
        public ObservableCollection<OrderItem> OrderItems { get; } = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private Customer? _selectedCustomer;

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private Vendedor? _selectedVendedor;

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private decimal _totalAmount;

        public TakeOrderViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Toma de Pedido";
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                var prods = await _dataService.GetProductsAsync();
                AvailableProducts.Clear();
                foreach (var p in prods.Where(p => p.Stock > 0)) AvailableProducts.Add(p);

                var custs = await _dataService.GetCustomersAsync();
                AvailableCustomers.Clear();
                foreach (var c in custs) AvailableCustomers.Add(c);

                var vendors = await _dataService.GetVendedoresAsync();
                AvailableVendedores.Clear();
                foreach (var v in vendors) AvailableVendedores.Add(v);

                CalculateTotal();
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SearchProductAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults.Clear();
                return;
            }

            var query = SearchText.ToLower();
            var results = AvailableProducts.Where(p => 
                p.Name.ToLower().Contains(query) || 
                p.Barcode.Contains(SearchText)
            ).ToList();

            SearchResults.Clear();
            foreach (var r in results) SearchResults.Add(r);

            // Si hay exactamente un resultado por código de barras exacto, lo seleccionamos
            var exactMatch = results.FirstOrDefault(p => p.Barcode == SearchText);
            if (exactMatch != null)
            {
                SelectedProduct = exactMatch;
                SearchText = string.Empty;
                SearchResults.Clear();
                // Opcionalmente agregar directo si el usuario lo prefiere
            }
        }

        [RelayCommand]
        public void SelectProduct(Product product)
        {
            SelectedProduct = product;
            SearchResults.Clear();
            SearchText = string.Empty;
        }

        [RelayCommand]
        public void AddProduct()
        {
            if (SelectedProduct == null || Quantity <= 0) return;

            if (Quantity > SelectedProduct.Stock)
            {
                Shell.Current.DisplayAlert("Error", "No hay suficiente stock", "OK");
                return;
            }

            var existing = OrderItems.FirstOrDefault(i => i.ProductId == SelectedProduct.Id);
            if (existing != null)
            {
                existing.Quantity += Quantity;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    ProductId = SelectedProduct.Id,
                    Product = SelectedProduct,
                    Quantity = Quantity,
                    UnitPrice = SelectedProduct.Price
                });
            }

            // Notifica cambios forzando refresh del total
            OnPropertyChanged(nameof(OrderItems));
            CalculateTotal();
            SelectedProduct = null;
            Quantity = 1;
        }

        [RelayCommand]
        public async Task ScanAsync()
        {
            var scannerPage = new BarcodeScannerPage();
            scannerPage.OnCodeScanned += (code) =>
            {
                SearchText = code;
                _ = SearchProductAsync();
            };
            await Shell.Current.Navigation.PushModalAsync(scannerPage);
        }

        [RelayCommand]
        public void RemoveProduct(OrderItem item)
        {
            if (item != null && OrderItems.Contains(item))
            {
                OrderItems.Remove(item);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            TotalAmount = OrderItems.Sum(i => i.SubTotal);
        }

        [RelayCommand]
        public async Task SaveOrderAsync()
        {
            if (SelectedCustomer == null)
            {
                await Shell.Current.DisplayAlert("Aviso", "Seleccione un cliente", "OK");
                return;
            }

            if (!OrderItems.Any())
            {
                await Shell.Current.DisplayAlert("Aviso", "Agregue al menos un producto", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var newOrder = new Order
                {
                    CustomerId = SelectedCustomer.Id,
                    Customer = SelectedCustomer,
                    VendedorId = SelectedVendedor?.Id,
                    Vendedor = SelectedVendedor,
                    Date = DateTime.Now,
                    TotalAmount = TotalAmount,
                    Items = OrderItems.ToList(),
                    Status = "Pendiente"
                };

                await _dataService.SaveOrderAsync(newOrder);

                await Shell.Current.DisplayAlert("Éxito", "Pedido guardado correctamente", "OK");

                // Clear form
                SelectedCustomer = null;
                OrderItems.Clear();
                CalculateTotal();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
