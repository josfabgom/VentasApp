using VentasApp.ViewModels;

namespace VentasApp.Views
{
    public partial class ProductFormPage : ContentPage
    {
        public ProductFormPage(ProductFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
