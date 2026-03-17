using VentasApp.ViewModels;

namespace VentasApp.Views
{
    public partial class CustomersPage : ContentPage
    {
        private readonly CustomersViewModel _viewModel;

        public CustomersPage(CustomersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCustomersCommand.ExecuteAsync(null);
        }
    }
}
