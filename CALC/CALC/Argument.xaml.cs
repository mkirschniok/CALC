using Newtonsoft.Json.Linq;
using System.Threading;

namespace CALC;

/// <summary>
/// Klasa ekranu z liczb� (argumentem)
/// </summary>
public partial class Argument : ContentPage
{
    /// <summary>
    /// Aktualna warto�� argumentu
    /// </summary>
    private int value = 1;
    /// <summary>
    /// Obiekt konfiguracji
    /// </summary>
    JObject config;
    /// <summary>
    /// Identyfikator telefonu
    /// </summary>
    string id;
    /// <summary>
    /// Akcja wysy�ania aktualizacji
    /// </summary>
    public static Action<JObject, string, string> SendUpdate;
    /// <summary>
    /// Token anulowania odtwarzania d�wi�ku
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Konstruktor klasy
    /// </summary>
    /// <param name="id">identyfikator telefonu</param>
    /// <param name="config">obiekt konfiguracji</param>
    public Argument(string id, JObject config)
    {
        InitializeComponent();
        this.id = id;
        this.config = config;
        SendUpdate.Invoke(config, id, value.ToString());
        ValueLabel.Text = value.ToString();
        ValueLabel.FontSize = 500 / ValueLabel.Text.Length;
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
    /// Metoda wywo�ywana po przesuni�ciu palcem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Metoda aktualizuj�ca warto�� argumentu
    /// </summary>
    private async void UpdateValue()
    {
        SendUpdate.Invoke(config, id, value.ToString());
        ValueLabel.Text = value.ToString();
        ValueLabel.FontSize = 500 / ValueLabel.Text.Length;
        await Task.Run(PlaySound);
    }

    /// <summary>
    /// Odtwarzanie d�wi�ku
    /// </summary>
    private async void PlaySound()
    {
        if (config["sound"].ToString() == "True")
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await TextToSpeech.Default.SpeakAsync(value == -1 ? "minus jeden" : value.ToString(), cancelToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }
        }
    }

}
