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
    /// Identyfikator urz¹dzenia
    /// </summary>
    string id;
    /// <summary>
    /// Operatory (wyœwietlane na ekranie)
    /// </summary>
    private string[] operators = { "+", "-", "×", "÷" };
    /// <summary>
    /// Nazwy operatorów (do odczytu przez syntezator mowy)
    /// </summary>
    private string[] names = { "plus", "minus", "razy", "podzieliæ przez" };
    /// <summary>
    /// Akcja wysy³ania aktualizacji
    /// </summary>
    public static Action<JObject, string, string> SendUpdate;
    /// <summary>
    /// Token anulowania odtwarzania dŸwiêku
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Konstruktor klasy Operator
    /// </summary>
    /// <param name="id">identyfikator urz¹dzenia</param>
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
    /// Metoda wywo³ywana po dotkniêciu ekranu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnTapped(object sender, EventArgs e)
    {
        await Task.Run(PlaySound);
    }

    /// <summary>
    /// Metoda wywo³ywana po przesuniêciu palcem po ekranie
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
    /// Metoda aktualizuj¹ca wartoœæ operatora
    /// </summary>
    private async void UpdateValue()
    {
        SendUpdate.Invoke(config, id, operators[choice]);
        OperatorLabel.Text = operators[choice];
        await Task.Run(PlaySound);
    }

    /// <summary>
    /// Odtwarzanie dŸwiêku
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
