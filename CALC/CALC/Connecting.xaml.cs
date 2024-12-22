using Newtonsoft.Json.Linq;

namespace CALC;

public partial class Connecting : ContentPage
{
    private string ssid, password, ip, id;
    public Connecting(string ssid, string password, string ip)
	{
		InitializeComponent();
        this.ssid = ssid;
        this.password = password;
        this.ip = ip;
        TcpClientApp.IDReceived += IDReceived;
        TcpClientApp.ConfigReceived += ConfigReceived;
    }

    public void IDReceived(string id)
    {
        this.id = id;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Status_Label.Text = "Połączono! \nID urządzenia: " + id;
        });
    }

    public void ConfigReceived(JObject config)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Status_Label.Text += "\nOtrzymano konfigurację";
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