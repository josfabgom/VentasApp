using VentasApp.ViewModels;

namespace VentasApp.Views
{
    public partial class TakeOrderPage : ContentPage
    {
        private readonly TakeOrderViewModel _viewModel;

        public TakeOrderPage(TakeOrderViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchProductCommand.Execute(null);
        }
    }
}
