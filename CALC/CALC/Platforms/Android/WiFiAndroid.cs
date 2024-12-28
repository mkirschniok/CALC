using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;

namespace CALC
{
    /// <summary>
    /// Klasa odpowiedzialna za łączenie się z siecią WiFi
    /// </summary>
    public class WiFiAndroid
    {
        /// <summary>
        /// Dane do połączenia z siecią WiFi i adresem IP
        /// </summary>
        string? ssid, password, ip;
        /// <summary>
        /// Metoda łącząca z siecią WiFi
        /// </summary>
        /// <param name="ssid">nazwa sieci</param>
        /// <param name="password">hasło</param>
        /// <param name="ip">adres ip jako string</param>
        public void ConnectToWifi(string ssid, string password, string ip)
        {
            this.ssid = ssid;
            this.password = password;
            this.ip = ip;
            var context = Android.App.Application.Context;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                var specifier = new WifiNetworkSpecifier.Builder()
                    .SetSsid(ssid)
                    .SetWpa2Passphrase(password)
                    .Build();

                var request = new NetworkRequest.Builder()
                    .AddTransportType(TransportType.Wifi)
                    .SetNetworkSpecifier(specifier)
                    .Build();

                var connectivityManager = (ConnectivityManager?)context.GetSystemService(Context.ConnectivityService);
                connectivityManager.RequestNetwork(request, new NetworkCallback(ip));
            }
        }

        /// <summary>
        /// Klasa odpowiedzialna za obsługę callbacków związanych z siecią
        /// </summary>
        public class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            /// <summary>
            /// Adres IP
            /// </summary>
            string ip;

            /// <summary>
            /// Konstruktor klasy
            /// </summary>
            /// <param name="ip">adres IP jako string</param>
            public NetworkCallback(string ip)
            {
                this.ip = ip;
            }

            /// <summary>
            /// Metoda wywoływana, gdy sieć jest dostępna
            /// </summary>
            /// <param name="network">sieć</param>
            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
                connectivityManager.BindProcessToNetwork(network);
                TcpClientApp tcpClientApp = new TcpClientApp();
                tcpClientApp.ConnectToServer(ip);
            }

            /// <summary>
            /// Metoda wywoływana, gdy sieć jest niedostępna
            /// </summary>
            public override void OnUnavailable()
            {
                base.OnUnavailable();
            }
        }

    }
}
