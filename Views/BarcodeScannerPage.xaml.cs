using ZXing.Net.Maui;

namespace VentasApp.Views;

public partial class BarcodeScannerPage : ContentPage
{
    public event Action<string>? OnCodeScanned;
    private bool _isScanned = false;

	public BarcodeScannerPage()
	{
		InitializeComponent();
        
        barcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false
        };
	}

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isScanned) return;

        var first = e.Results.FirstOrDefault();
        if (first != null)
        {
            _isScanned = true;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                OnCodeScanned?.Invoke(first.Value);
                await Navigation.PopModalAsync();
            });
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
