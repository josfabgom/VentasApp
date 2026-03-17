using VentasApp.ViewModels;

namespace VentasApp.Views
{
    public partial class DeliveryPage : ContentPage
    {
        private readonly DeliveryViewModel _viewModel;

        public DeliveryPage(DeliveryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDeliveriesCommand.ExecuteAsync(null);
        }
    }
}
