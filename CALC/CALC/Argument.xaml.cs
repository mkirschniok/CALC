using System.Globalization;
using MauiShakeDetector;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CALC;

public partial class Argument : ContentPage
{
    private int value = 1;
    JObject config;
    string id;
    public static Action<JObject, string, string> SendUpdate;
    public Argument(string id, JObject config)
    {
        InitializeComponent();
        this.id = id;
        this.config = config;
        UpdateValue();
    }

    private void OnTapped(object sender, EventArgs e)
    {
        if (config["sound"].ToString() == "True")  TextToSpeech.SpeakAsync(value == -1 ? "minus jeden" : value.ToString());
    }

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left)
        {
            value++;
            UpdateValue();
        }
        else if (e.Direction == SwipeDirection.Right)
        {
            value--;
            UpdateValue();
        }
    }

    private async void UpdateValue()
    {
        SendUpdate.Invoke(config, id, value.ToString());
        ValueLabel.Text = value.ToString();
        ValueLabel.FontSize = 500 / ValueLabel.Text.Length;
        if (config["sound"].ToString() == "True")
        {
#if ANDROID
            Android.Speech.Tts.TextToSpeech tts = new Android.Speech.Tts.TextToSpeech(Android.App.Application.Context, null);
            if (tts.IsSpeaking) tts.Stop();
#endif
            await TextToSpeech.Default.SpeakAsync(value == -1 ? "minus jeden" : value.ToString());
        }
    }

}
