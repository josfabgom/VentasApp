using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VentasApp.Services;

namespace VentasApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Iniciar Sesión";
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Ingrese usuario y contraseña";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var user = await _dataService.LoginAsync(Username, Password);

            IsBusy = false;

            if (user != null)
            {
                // Navegar al shell principal
                App.CurrentUser = user;
                await Shell.Current.GoToAsync("//ProductsPage");
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos";
            }
        }
    }
}
