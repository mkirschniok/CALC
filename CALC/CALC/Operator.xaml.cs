using System.Globalization;
using MauiShakeDetector;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CALC;

public partial class Operator : ContentPage
{
    private int choice = 0;
    JObject config;
    string id;
    private string[] operators = { "+", "-", "×", "÷" };
    private string[] names = { "plus", "minus", "razy", "podzieliæ przez" };
    public static Action<JObject, string, string> SendUpdate;

    public Operator(string id, JObject config)
    {
        InitializeComponent();
        this.id = id;
        this.config = config;
        UpdateValue();
        if (ShakeDetector.Default.IsSupported && !ShakeDetector.Default.IsMonitoring)
        {
            ShakeDetector.Default.StartListening();
            ShakeDetector.Default.IsHapticsEnabled = false;
            ShakeDetector.Default.ShakeDetected += Detector_ShakeDetected;
        }
    }

    private void Detector_ShakeDetected(object sender, ShakeDetectedEventArgs e)
    {
        if (config["sound"].ToString() == "True") TextToSpeech.SpeakAsync(names[choice]);
    }

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left)
        {
            choice = (choice + 1) % operators.Length;
            UpdateValue();
        }
        else if (e.Direction == SwipeDirection.Right)
        {
            choice = (choice - 1 + operators.Length) % operators.Length;
            UpdateValue();
        }
    }

    private async void UpdateValue()
    {
        SendUpdate.Invoke(config, id, operators[choice]);
        OperatorLabel.Text = operators[choice];
        if (config["sound"].ToString() == "True") await TextToSpeech.Default.SpeakAsync(names[choice]);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (ShakeDetector.Default.IsMonitoring)
        {
            ShakeDetector.Default.StopListening();
            ShakeDetector.Default.ShakeDetected -= Detector_ShakeDetected;
        }
    }
}
