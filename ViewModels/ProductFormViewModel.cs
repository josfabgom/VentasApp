using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    [QueryProperty(nameof(ProductParam), "ProductParam")]
    public partial class ProductFormViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ProductFormViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Nuevo Producto";
        }

        private Product? _productParam;
        public Product? ProductParam
        {
            get => _productParam;
            set
            {
                SetProperty(ref _productParam, value);
                if (value != null)
                {
                    Title = "Editar Producto";
                    Name = value.Name;
                    Description = value.Description;
                    Price = value.Price;
                    Stock = value.Stock;
                    Barcode = value.Barcode;
                    QrCode = value.QrCode;
                    ImageUrl = value.ImageUrl;
                }
            }
        }

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private int _stock;

        [ObservableProperty]
        private string _barcode = string.Empty;

        [ObservableProperty]
        private string _qrCode = string.Empty;

        [ObservableProperty]
        private string _imageUrl = string.Empty;

        [RelayCommand]
        public async Task PickImageAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleccione una imagen",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Copiamos la imagen al directorio local de la app para asegurar su acceso en todas las plataformas
                    var newFile = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                    {
                        await stream.CopyToAsync(newStream);
                    }
                    ImageUrl = newFile;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar la imagen: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SaveContactAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Aviso", "El nombre del producto es obligatorio", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var targetProduct = ProductParam ?? new Product();
                targetProduct.Name = Name;
                targetProduct.Description = Description;
                targetProduct.Price = Price;
                targetProduct.Stock = Stock;
                targetProduct.Barcode = Barcode;
                targetProduct.QrCode = QrCode;
                targetProduct.ImageUrl = ImageUrl;

                await _dataService.SaveProductAsync(targetProduct);

                await Shell.Current.DisplayAlert("Éxito", "Producto guardado correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
