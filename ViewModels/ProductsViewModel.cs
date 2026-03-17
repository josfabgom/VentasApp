using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class ProductsViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Product> Products { get; } = new();

        public ProductsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Productos";
        }

        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var products = await _dataService.GetProductsAsync();
                Products.Clear();
                foreach (var p in products)
                    Products.Add(p);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddProductAsync()
        {
            await Shell.Current.GoToAsync("ProductFormPage");
        }

        [RelayCommand]
        public async Task EditProductAsync(Product product)
        {
            if (product == null) return;
            var parameters = new Dictionary<string, object>
            {
                { "ProductParam", product }
            };
            await Shell.Current.GoToAsync("ProductFormPage", parameters);
        }

        [RelayCommand]
        public async Task DeleteProductAsync(Product product)
        {
            if (product == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Desea eliminar el producto '{product.Name}'?", "Sí", "No");
            if (!confirm) return;

            IsBusy = true;
            bool success = await _dataService.DeleteProductAsync(product.Id);
            IsBusy = false;

            if (success)
            {
                Products.Remove(product);
                await Shell.Current.DisplayAlert("Éxito", "Producto eliminado correctamente", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se puede eliminar porque tiene ventas asociadas", "OK");
            }
        }
    }
}
