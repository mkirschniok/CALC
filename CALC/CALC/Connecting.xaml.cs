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
        Device.BeginInvokeOnMainThread(() =>
        {
            Status_Label.Text = "Po��czono! \nID urz�dzenia: " + id;
        });
    }

    public void ConfigReceived(JObject config)
    {
        Device.BeginInvokeOnMainThread(() =>
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