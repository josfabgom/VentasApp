using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class VendedoresViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Vendedor> Vendedores { get; } = new();

        [ObservableProperty]
        private Vendedor _selectedVendedor = new();

        public VendedoresViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Vendedores";
        }

        [RelayCommand]
        public async Task LoadVendedoresAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var list = await _dataService.GetVendedoresAsync();
                Vendedores.Clear();
                foreach (var item in list) Vendedores.Add(item);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SaveVendedorAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedVendedor.Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es requerido", "OK");
                return;
            }

            IsBusy = true;
            await _dataService.SaveVendedorAsync(SelectedVendedor);
            IsBusy = false;
            
            SelectedVendedor = new Vendedor();
            await LoadVendedoresAsync();
            await Shell.Current.DisplayAlert("Éxito", "Vendedor guardado", "OK");
        }

        [RelayCommand]
        public void EditVendedor(Vendedor vendedor)
        {
            SelectedVendedor = new Vendedor
            {
                Id = vendedor.Id,
                Nombre = vendedor.Nombre,
                Email = vendedor.Email,
                Telefono = vendedor.Telefono
            };
        }

        [RelayCommand]
        public async Task DeleteVendedorAsync(Vendedor vendedor)
        {
            if (vendedor == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Eliminar a {vendedor.Nombre}?", "Sí", "No");
            if (!confirm) return;

            IsBusy = true;
            bool success = await _dataService.DeleteVendedorAsync(vendedor.Id);
            IsBusy = false;

            if (success)
            {
                Vendedores.Remove(vendedor);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se puede eliminar (posiblemente tiene equipos asociados)", "OK");
            }
        }
    }
}
