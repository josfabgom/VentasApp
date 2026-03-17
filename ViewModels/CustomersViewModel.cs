using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class CustomersViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Customer> Customers { get; } = new();
        public List<string> AvailableTipos { get; } = new() { "Minorista", "Mayorista" };

        [ObservableProperty]
        private Customer? _selectedCustomer;

        [ObservableProperty]
        private int _customerId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        [ObservableProperty]
        private string _localidad = string.Empty;

        [ObservableProperty]
        private string _provincia = string.Empty;

        [ObservableProperty]
        private string _barrio = string.Empty;

        [ObservableProperty]
        private string _zona = string.Empty;

        [ObservableProperty]
        private string _tipo = "Minorista";

        public CustomersViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Clientes";
        }

        [RelayCommand]
        public async Task LoadCustomersAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var customers = await _dataService.GetCustomersAsync();
                Customers.Clear();
                foreach (var c in customers)
                    Customers.Add(c);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var customer = new Customer
                {
                    Id = CustomerId,
                    Name = Name,
                    Email = Email,
                    Phone = Phone,
                    Address = Address,
                    Localidad = Localidad,
                    Provincia = Provincia,
                    Barrio = Barrio,
                    Zona = Zona,
                    Tipo = Tipo
                };

                await _dataService.SaveCustomerAsync(customer);
                ClearFields();
                await LoadCustomersAsync();
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public void EditCustomer(Customer customer)
        {
            if (customer == null) return;

            CustomerId = customer.Id;
            Name = customer.Name;
            Email = customer.Email;
            Phone = customer.Phone;
            Address = customer.Address;
            Localidad = customer.Localidad;
            Provincia = customer.Provincia;
            Barrio = customer.Barrio;
            Zona = customer.Zona;
            Tipo = customer.Tipo;
        }

        [RelayCommand]
        public async Task DeleteCustomerAsync(Customer customer)
        {
            if (customer == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Desea eliminar este cliente?", "Sí", "No");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                await _dataService.DeleteCustomerAsync(customer.Id);
                await LoadCustomersAsync();
                if (CustomerId == customer.Id) ClearFields();
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public void ClearFields()
        {
            CustomerId = 0;
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            Localidad = string.Empty;
            Provincia = string.Empty;
            Barrio = string.Empty;
            Zona = string.Empty;
            Tipo = "Minorista";
        }
    }
}
