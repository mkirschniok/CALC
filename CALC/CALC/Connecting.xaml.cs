using Newtonsoft.Json.Linq;

namespace CALC;

/// <summary>
/// Klasa ekranu ��czenia z urz�dzeniem
/// </summary>
public partial class Connecting : ContentPage
{
    /// <summary>
    /// Parametry po��czenia, id urz�dzenia
    /// </summary>
    private string ssid, password, ip, id;

    /// <summary>
    /// Konstruktor klasy
    /// </summary>
    /// <param name="ssid">nazwa sieci</param>
    /// <param name="password">has�o</param>
    /// <param name="ip">adres ip jako string</param>
    public Connecting(string ssid, string password, string ip)
	{
		InitializeComponent();
        this.ssid = ssid;
        this.password = password;
        this.ip = ip;
        TcpClientApp.IDReceived += IDReceived;
        TcpClientApp.ConfigReceived += ConfigReceived;
    }

    /// <summary>
    /// Metoda wywo�ywana po uzyskaniu id urz�dzenia
    /// </summary>
    /// <param name="id">identyfikator urz�dzenia</param>
    public void IDReceived(string id)
    {
        this.id = id;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Status_Label.Text = "Po��czono! \nID urz�dzenia: " + id;
        });
    }

    /// <summary>
    /// Metoda wywo�ywana po otrzymaniu konfiguracji
    /// </summary>
    /// <param name="config">obiekt konfiguracji</param>
    public void ConfigReceived(JObject config)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Status_Label.Text += "\nOtrzymano konfiguracj�";
        if (config["role"].ToString().Contains("arg"))
        {
            Navigation.PushAsync(new Argument(id, config));
        }
        else if (config["role"].ToString() == "oper")
        {
            Navigation.PushAsync(new Operator(id, config));
        }
        else if (config["role"].ToString() == "res")
        {
            Navigation.PushAsync(new Result(id, config));
        }
        });
    }

}