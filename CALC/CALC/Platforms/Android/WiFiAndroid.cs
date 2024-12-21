using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Android.Net.ConnectivityManager;

namespace CALC
{
    public class WiFiAndroid
    {
        string ssid, password, ip;
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

                var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                connectivityManager.RequestNetwork(request, new NetworkCallback(ip));
            }
            else
            {
                // Obsługa starszych wersji Androida (SDK < Q)
                WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                WifiConfiguration wifiConfig = new WifiConfiguration
                {
                    Ssid = $"\"{ssid}\"",
                    PreSharedKey = $"\"{password}\""
                };

                int netId = wifiManager.AddNetwork(wifiConfig);
                wifiManager.Disconnect();
                wifiManager.EnableNetwork(netId, true);
                wifiManager.Reconnect();
            }
        }

        public class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            string ip;
            public NetworkCallback(string ip)
            {
                this.ip = ip;
            }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
                connectivityManager.BindProcessToNetwork(network);
                TcpClientApp tcpClientApp = new TcpClientApp();
                tcpClientApp.ConnectToServer(ip);
            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
            }
        }

    }
}
