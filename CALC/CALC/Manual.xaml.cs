namespace CALC;

/// <summary>
/// Klasa ekranu rêcznego wprowadzania danych do po³¹czenia z tabletem
/// </summary>
public partial class Manual : ContentPage
{
    /// <summary>
    /// Konstruktor bezargumentowy
    /// </summary>
    public Manual()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Konstruktor z argumentami
    /// </summary>
    /// <param name="ssid">nazwa sieci</param>
    /// <param name="password">has³o</param>
    /// <param name="ip">adres IP jako string</param>
	public Manual(string ssid, string password, string ip)
    {
        InitializeComponent();
        SSID_Entry.Text = ssid;
        Password_Entry.Text = password;
        IP_Entry.Text = ip;
        Submit_Button.IsEnabled = true;

    }

    /// <summary>
    /// Metoda wywo³ywana po zmianie tekstu w polach tekstowych
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TextUpdate(object sender, EventArgs e)
    {
        if (SSID_Entry.Text != "" && Password_Entry.Text != "" && IP_Entry.Text != "")
        {
            Submit_Button.IsEnabled = true;
        }
        else
        {
            Submit_Button.IsEnabled = false;
        }
    }

    /// <summary>
    /// Metoda wywo³ywana po naciœniêciu przycisku "ZatwierdŸ"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnSubmitClicked(object sender, EventArgs e)
    {
#if ANDROID
        WiFiAndroid wiFiAndroid = new WiFiAndroid();
        wiFiAndroid.ConnectToWifi(SSID_Entry.Text, Password_Entry.Text, IP_Entry.Text);
#endif
        Navigation.PushAsync(new Connecting(SSID_Entry.Text, Password_Entry.Text, IP_Entry.Text));
    }
}