using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class OrdersViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Order> Orders { get; } = new();
        public ObservableCollection<Order> InvoicedOrders { get; } = new();

        public OrdersViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Control de Facturación";
        }

        [RelayCommand]
        public async Task LoadOrdersAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var orders = await _dataService.GetOrdersAsync();

                Orders.Clear();
                InvoicedOrders.Clear();

                foreach (var o in orders)
                {
                    Orders.Add(o);
                    if (o.Status == "Facturado")
                    {
                        InvoicedOrders.Add(o);
                    }
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task InvoiceOrderAsync(Order order)
        {
            if (order == null || order.Status == "Facturado") return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Desea facturar el pedido {order.Id}?", "Sí", "No");
            if (confirm)
            {
                await _dataService.UpdateOrderStatusAsync(order.Id, "Facturado");
                await LoadOrdersAsync();
            }
        }

        [RelayCommand]
        public async Task CancelOrderAsync(Order order)
        {
            if (order == null || order.Status == "Cancelado") return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Desea cancelar el pedido {order.Id}?", "Sí", "No");
            if (confirm)
            {
                await _dataService.UpdateOrderStatusAsync(order.Id, "Cancelado");
                await LoadOrdersAsync();
            }
        }
    }
}
