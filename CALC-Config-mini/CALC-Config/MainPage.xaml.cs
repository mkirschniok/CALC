using QRCoder;
using Newtonsoft.Json.Linq;

namespace CALC_Config
{
    /// <summary>
    /// Klasa strony głównej aplikacji
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Obiekt serwera WebSocket
        /// </summary>
        private readonly TcpServer _server;
        /// <summary>
        /// Dane sieci WiFi i adres IP
        /// </summary>
        private string? ssid, password, ip;
        /// <summary>
        /// Akcja wysyłająca konfigurację do urządzeń
        /// </summary>
        public static Action<JObject>? SendConfig;

        /// <summary>
        /// Konsktruktor klasy MainPage
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Dark;
#if ANDROID
            WiFiAndroid.CheckAndRequestPermissions();
#endif
            _server = new TcpServer();
            TcpServer.DeviceAdded += UpdateDeviceList;
        }

        /// <summary>
        /// Aktualiacja danych sieci WiFi i adresu IP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Clicked(object sender, EventArgs e)
        {
#if ANDROID
            ssid = WiFiAndroid.GetSSID();
            ip = WiFiAndroid.GetLocalIPAddress();
#endif
            password = Password_Entry.Text;
            SSID_Label.Text = ssid;
            IP_Label.Text = ip;
        }

        /// <summary>
        /// Generowanie kodu QR z danymi sieci WiFi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Generate_Clicked(object sender, EventArgs e)
        {
            JObject wifiData = new()
            {
                { "ssid", ssid },
                { "password", password },
                { "ip", ip }
            };
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(wifiData.ToString(), QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new(qrCodeData);
            QRCode_Image.Source = ImageSource.FromStream(() => new MemoryStream(qrCode.GetGraphic(20)));
        }

        /// <summary>
        /// Uruchomienie kalulatora
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Start_Clicked(object sender, EventArgs e)
        {
            JObject config = new()
            {
                { "type", "config" },
                { "sound", Sound_Switch.IsToggled}
            };
            SendConfig?.Invoke(config);
            Navigation.PushAsync(new Preview());
        }

        /// <summary>
        /// Włączenie serwera WebSocket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Server_Clicked(object sender, EventArgs e)
        {
            _server.StartServer();
            Server_Button.IsEnabled = false;
            Server_Button.Text = "Serwer: ON";
        }

        /// <summary>
        /// Aktualizacja listy podłączonych urządzeń
        /// </summary>
        public void UpdateDeviceList()
        {
            string text = "Lista urządzeń: \n";
            foreach (string device in _server._clients.Keys)
            {
                text += "ID: " + device + '\n';
            }
            DeviceList_Label.Text = text;
            if (_server._clients.Count == 4)
            {
                Start_Button.IsEnabled = true;
            }
        }
    }
}