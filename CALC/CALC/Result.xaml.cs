using System.Globalization;
using MauiShakeDetector;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CALC;

public partial class Result : ContentPage
{
    private string result = "2";
    private string id;
    public static Action<string> RequestUpdate;
    private JObject config;
    public Result(string id, JObject config)
    {
        this.id = id;
        this.config = config;
        TcpClientApp.ResultReceived += async (result) => await ResultReceived(result);
        InitializeComponent();
    }

    private void OnLabelTapped(object sender, EventArgs e)
    {
        if (config["sound"].ToString() == "True") PlaySound();
    }

    private async Task ResultReceived(string result)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            this.result = result;
            ValueLabel.Text = result;
            ValueLabel.FontSize = 500 / ValueLabel.Text.Length;
            if (config["sound"].ToString() == "True") PlaySound();
        });
    }

    private async void PlaySound()
    {
        string tts = result;
        if (result.Contains("r"))
        {
            tts = result.Replace("r", "reszty");
        }
        if (result.Contains("-1"))
        {
            tts.Replace("-1", "minus jeden");
        }
        await TextToSpeech.Default.SpeakAsync(tts);
    }

}
