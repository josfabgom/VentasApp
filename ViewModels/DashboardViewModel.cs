using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Models;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<SalesSummary> SalesPerVendedor { get; } = new();
        public ObservableCollection<Vendedor> AvailableVendedores { get; } = new();

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.Date;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now.Date;

        [ObservableProperty]
        private Vendedor? _selectedVendedor;

        [ObservableProperty]
        private decimal _totalSales;

        [ObservableProperty]
        private int _totalOrders;

        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Resumen de Ventas";
        }

        [RelayCommand]
        public async Task LoadDashboardAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Cargar vendedores para el filtro si no están cargados
                if (!AvailableVendedores.Any())
                {
                    var vendors = await _dataService.GetVendedoresAsync();
                    foreach (var v in vendors) AvailableVendedores.Add(v);
                }

                var allOrders = await _dataService.GetOrdersAsync();

                // Filtrar por fecha
                var filteredOrders = allOrders.Where(o => o.Date.Date >= StartDate.Date && o.Date.Date <= EndDate.Date);

                // Filtrar por vendedor si hay uno seleccionado
                if (SelectedVendedor != null)
                {
                    filteredOrders = filteredOrders.Where(o => o.VendedorId == SelectedVendedor.Id);
                }

                var list = filteredOrders.ToList();

                // Totales generales filtrados
                TotalSales = list.Sum(o => o.TotalAmount);
                TotalOrders = list.Count;

                // Agrupar por vendedor para el detalle
                var summary = list.GroupBy(o => o.VendedorId)
                                  .Select(g => new SalesSummary
                                  {
                                      VendedorNombre = g.FirstOrDefault()?.Vendedor?.Nombre ?? "Sin Asignar",
                                      TotalVendido = g.Sum(o => o.TotalAmount),
                                      CantidadPedidos = g.Count()
                                  })
                                  .OrderByDescending(s => s.TotalVendido)
                                  .ToList();

                SalesPerVendedor.Clear();
                foreach (var item in summary) SalesPerVendedor.Add(item);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ClearFilters()
        {
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            SelectedVendedor = null;
            _ = LoadDashboardAsync();
        }
    }

    public class SalesSummary
    {
        public string VendedorNombre { get; set; } = string.Empty;
        public decimal TotalVendido { get; set; }
        public int CantidadPedidos { get; set; }
    }
}
