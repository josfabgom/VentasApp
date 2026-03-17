using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class DeliveryViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Order> PendingDeliveries { get; } = new();

        public DeliveryViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Preparación y Entregas";
        }

        [RelayCommand]
        public async Task LoadDeliveriesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var orders = await _dataService.GetOrdersAsync();

                PendingDeliveries.Clear();
                // Mostramos las órdenes que NO han sido canceladas y que todavía no están entregadas formalmente
                foreach (var o in orders.Where(o => o.Status != "Cancelado" && o.DeliveryStatus != "Entregado"))
                {
                    PendingDeliveries.Add(o);
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task NextPhaseAsync(Order order)
        {
            if (order == null) return;

            string nextStatus = order.DeliveryStatus switch
            {
                "PorPreparar" => "EnPreparacion",
                "EnPreparacion" => "ListoParaEnvio",
                "ListoParaEnvio" => "Entregado",
                _ => order.DeliveryStatus
            };

            if (nextStatus != order.DeliveryStatus)
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Cambiar Estado",
                    $"¿Cambiar estado de embalaje/entrega a '{nextStatus}'?",
                    "Sí", "No");

                if (confirm)
                {
                    await _dataService.UpdateOrderDeliveryStatusAsync(order.Id, nextStatus);
                    await LoadDeliveriesAsync(); // refrescar
                }
            }
        }
    }
}
