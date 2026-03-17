using VentasApp.ViewModels;

namespace VentasApp.Views;

public partial class EquiposPage : ContentPage
{
	public EquiposPage(EquiposViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as EquiposViewModel).InitializeAsync();
    }
}
