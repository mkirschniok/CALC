#if ANDROID
using Android.Net;
using Java.Interop;
using Java.Nio.Channels;
#endif
using Newtonsoft.Json.Linq;
using ZXing.Net.Maui;

namespace CALC;

public partial class Scanner : ContentPage
{
    private string ssid, password, ip;
    private BarcodeResult? previousResult; 
    public Scanner()
	{
		InitializeComponent();
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true
        };
    }

    public static Action<string, string, string> GotData;
    protected async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results?.FirstOrDefault();
        if (result is null || result == previousResult) return;
        try
        {
            // Get data from JSON
            previousResult = result;
            JObject wifiData = JObject.Parse(result.Value);
            ssid = wifiData["ssid"].ToString();
            password = wifiData["password"].ToString();
            ip = wifiData["ip"].ToString();
            Device.BeginInvokeOnMainThread(async () => {
                await Navigation.PopAsync();
            });
            GotData?.Invoke(ssid, password, ip);
        }
        catch (Exception) { return; }
    }
}