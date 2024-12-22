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
        if (result is null) return;
        try
        {
            JObject wifiData = JObject.Parse(result.Value);
            var ssid = wifiData["ssid"].ToString();
            var password = wifiData["password"].ToString();
            var ip = wifiData["ip"].ToString();
            MainThread.BeginInvokeOnMainThread(async () => {
                await Navigation.PopAsync();
                GotData?.Invoke(ssid, password, ip);
            });
        }
        catch (Exception) { return; }
    }
}