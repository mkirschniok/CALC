namespace CALC;

public partial class Manual : ContentPage
{
    public Manual()
    {
        InitializeComponent();
    }

	public Manual(string ssid, string password, string ip)
    {
        InitializeComponent();
        SSID_Entry.Text = ssid;
        Password_Entry.Text = password;
        IP_Entry.Text = ip;
        Submit_Button.IsEnabled = true;
    }

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

    public void OnSubmitClicked(object sender, EventArgs e)
    {
#if ANDROID
        WiFiAndroid wiFiAndroid = new WiFiAndroid();
        wiFiAndroid.ConnectToWifi(SSID_Entry.Text, Password_Entry.Text, IP_Entry.Text);
#endif
        // Forward to "Connecting" page
        Navigation.PushAsync(new Connecting(SSID_Entry.Text, Password_Entry.Text, IP_Entry.Text));
    }
}