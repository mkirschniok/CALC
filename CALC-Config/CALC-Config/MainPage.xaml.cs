using System.Reactive.Linq;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Net.Sockets;
using QRCoder;
using System.Net;
using Newtonsoft.Json.Linq;

namespace CALC_Config
{
    public partial class MainPage : ContentPage
    {
        private TcpServer _server;
        public static Action<JObject>? SendConfig;
        public MainPage()
        {
            InitializeComponent();
            // Poproś o uprawnienia do lokalizacji (wymagane do uzyskania nazwy sieci WiFi)
#if ANDROID
            WiFiAndroid.CheckAndRequestPermissions();
#endif
            _server = new TcpServer();
            _server.StartServer();
            TcpServer.DeviceAdded += UpdateDeviceList;

        }

        private void Update_Clicked(object sender, EventArgs e)
        {
#if ANDROID
            UpdateWiFiData(WiFiAndroid.GetSSID(), Password_Entry.Text, WiFiAndroid.GetLocalIPAddress());
#endif
        }

        public void UpdateWiFiData(string ssid, string password, string ip)
        {
            SSID_Label.Text = "SSID: " + ssid;
            IP_Label.Text = "IP: " + ip;
            JObject wifiData = new JObject
            {
                { "ssid", ssid },
                { "password", password },
                { "ip", ip }
            };
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(wifiData.ToString(), QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            QRCode_Image.Source = ImageSource.FromStream(() => new MemoryStream(qrCode.GetGraphic(20)));
        }

        public void UpdateDeviceList()
        {
            string text = "Lista urządzeń: \n";
            foreach (string device in _server._clients.Keys)
            {
                text += "ID: " + device + '\n';
            }
            DeviceList_Label.Text = text;
        }

        public void Start_Clicked(object sender, EventArgs e)
        {
            JObject config = new JObject
            {
                { "type", "config" },
                { "sound", Sound_Switch.IsToggled}
            };
            SendConfig?.Invoke(config);
            Navigation.PushAsync(new Preview());
        }

    }


}