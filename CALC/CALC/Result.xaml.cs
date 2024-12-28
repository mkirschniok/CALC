using Newtonsoft.Json.Linq;

namespace CALC;

/// <summary>
/// Klasa ekranu wyniku
/// </summary>
public partial class Result : ContentPage
{
    /// <summary>
    /// Waro�� wyniku jako string
    /// </summary>
    private string result = "2";
    /// <summary>
    /// Identyfikator urz�dzenia
    /// </summary>
    private string id;
    /// <summary>
    /// Obiekt konfiguracji
    /// </summary>
    private JObject config;
    /// <summary>
    /// Token anulowania odtwarzania d�wi�ku
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Konstruktor klasy
    /// </summary>
    /// <param name="id">identyfikator urz�dzenia</param>
    /// <param name="config">obiekt konfiguracji</param>
    public Result(string id, JObject config)
    {
        this.id = id;
        this.config = config;
        TcpClientApp.ResultReceived += async (result) => await ResultReceived(result);
        InitializeComponent();
    }

    /// <summary>
    /// Metoda wywo�ywana po dotkni�ciu ekranu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnTapped(object sender, EventArgs e)
    {
        await Task.Run(PlaySound);
    }

    /// <summary>
    /// Metoda wywo�ywana po otrzymaniu wyniku
    /// </summary>
    /// <param name="result">wynik jako string</param>
    /// <returns></returns>
    private async Task ResultReceived(string result)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            this.result = result;
            ValueLabel.Text = result;
            ValueLabel.FontSize = 500 / ValueLabel.Text.Length;
        });
    }

    /// <summary>
    /// Metoda odtwarzaj�ca d�wi�k
    /// </summary>
    private async void PlaySound()
    {
        if (config["sound"].ToString() == "True")
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            string tts = result;
            if (result.Contains('r')) tts = result.Replace("r", "reszty");
            try
            {
                await TextToSpeech.SpeakAsync(tts, cancelToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

}
