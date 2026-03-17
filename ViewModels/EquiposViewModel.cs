using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class EquiposViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Equipo> Equipos { get; } = new();
        public ObservableCollection<Vendedor> Vendedores { get; } = new();

        [ObservableProperty]
        private Equipo _selectedEquipo = new();

        [ObservableProperty]
        private Vendedor? _vendedorSeleccionado;

        public EquiposViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Equipos";
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadVendedoresAsync();
            await LoadEquiposAsync();
        }

        public async Task LoadVendedoresAsync()
        {
            var list = await _dataService.GetVendedoresAsync();
            Vendedores.Clear();
            foreach (var item in list) Vendedores.Add(item);
        }

        [RelayCommand]
        public async Task LoadEquiposAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var list = await _dataService.GetEquiposAsync();
                Equipos.Clear();
                foreach (var item in list) Equipos.Add(item);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SaveEquipoAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedEquipo.Nombre))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es requerido", "OK");
                return;
            }

            SelectedEquipo.VendedorId = VendedorSeleccionado?.Id;

            IsBusy = true;
            await _dataService.SaveEquipoAsync(SelectedEquipo);
            IsBusy = false;

            SelectedEquipo = new Equipo();
            VendedorSeleccionado = null;
            await LoadEquiposAsync();
            await Shell.Current.DisplayAlert("Éxito", "Equipo guardado", "OK");
        }

        [RelayCommand]
        public void EditEquipo(Equipo equipo)
        {
            SelectedEquipo = new Equipo
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                Tipo = equipo.Tipo,
                NumeroSerie = equipo.NumeroSerie,
                VendedorId = equipo.VendedorId
            };
            VendedorSeleccionado = Vendedores.FirstOrDefault(v => v.Id == equipo.VendedorId);
        }

        [RelayCommand]
        public async Task DeleteEquipoAsync(Equipo equipo)
        {
            if (equipo == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Eliminar equipo {equipo.Nombre}?", "Sí", "No");
            if (!confirm) return;

            IsBusy = true;
            await _dataService.DeleteEquipoAsync(equipo.Id);
            IsBusy = false;

            Equipos.Remove(equipo);
        }
    }
}
