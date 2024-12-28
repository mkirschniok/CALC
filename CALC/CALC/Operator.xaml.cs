using Newtonsoft.Json.Linq;

namespace CALC;

/// <summary>
/// Klasa ekranu z wyborem operatora (+, -, *, /).
/// </summary>
public partial class Operator : ContentPage
{
    /// <summary>
    /// Indeks wybranego operatora
    /// </summary>
    private int choice = 0;
    /// <summary>
    /// Obiekt konfiguracji
    /// </summary>
    JObject config;
    /// <summary>
    /// Identyfikator urz�dzenia
    /// </summary>
    string id;
    /// <summary>
    /// Operatory (wy�wietlane na ekranie)
    /// </summary>
    private string[] operators = { "+", "-", "�", "�" };
    /// <summary>
    /// Nazwy operator�w (do odczytu przez syntezator mowy)
    /// </summary>
    private string[] names = { "plus", "minus", "razy", "podzieli� przez" };
    /// <summary>
    /// Akcja wysy�ania aktualizacji
    /// </summary>
    public static Action<JObject, string, string> SendUpdate;
    /// <summary>
    /// Token anulowania odtwarzania d�wi�ku
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Konstruktor klasy Operator
    /// </summary>
    /// <param name="id">identyfikator urz�dzenia</param>
    /// <param name="config">obiekt konfiguracji</param>
    public Operator(string id, JObject config)
    {
        InitializeComponent();
        this.id = id;
        this.config = config;
        SendUpdate.Invoke(config, id, operators[choice]);
        OperatorLabel.Text = operators[choice];
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
    /// Metoda wywo�ywana po przesuni�ciu palcem po ekranie
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Metoda aktualizuj�ca warto�� operatora
    /// </summary>
    private async void UpdateValue()
    {
        SendUpdate.Invoke(config, id, operators[choice]);
        OperatorLabel.Text = operators[choice];
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
                await TextToSpeech.Default.SpeakAsync(names[choice], cancelToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }
        }
    }
}
