using VentasApp.ViewModels;

namespace VentasApp.Views
{
    public partial class OrdersPage : ContentPage
    {
        private readonly OrdersViewModel _viewModel;

        public OrdersPage(OrdersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadOrdersCommand.ExecuteAsync(null);
        }
    }
}
