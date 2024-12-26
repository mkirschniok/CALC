using Android.App;
using Android.Net.Wifi;
using System.Net;

namespace CALC_Config
{
    /// <summary>
    /// Klasa zawierająca metody do obsługi WiFi na platformie Android
    /// </summary>
    public static class WiFiAndroid
    {
        /// <summary>
        /// Metoda zwracająca SSID aktualnie połączonej sieci WiFi
        /// </summary>
        /// <returns>SSID sieci</returns>
        public static string GetSSID()
        {
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
            string ssid = wifiManager.ConnectionInfo.SSID;
            if (ssid.StartsWith("\"") && ssid.EndsWith("\""))
            {
                ssid = ssid.Substring(1, ssid.Length - 2);
            }
            return ssid;
        }

        /// <summary>
        /// Metoda zwracająca lokalny adres IP urządzenia
        /// </summary>
        /// <returns>string z adresem IP</returns>
        public static string GetLocalIPAddress()
        {
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
            byte[] ipaddress = BitConverter.GetBytes(wifiManager.ConnectionInfo.IpAddress);
            var result = (new IPAddress(ipaddress)).ToString();
            return result;
        }

        /// <summary>
        /// Metoda sprawdzająca i żądająca uprawnień do lokalizacji (wymagane do pobrania SSID)
        /// </summary>
        public static async void CheckAndRequestPermissions()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}
