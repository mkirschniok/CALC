using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace CALC_Config
{
    public static class WiFiAndroid
    {
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
        public static string GetLocalIPAddress()
        {
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
            byte[] ipaddress = BitConverter.GetBytes(wifiManager.ConnectionInfo.IpAddress);
            var result = (new IPAddress(ipaddress)).ToString();
            return result;
        }

        public static async void CheckAndRequestPermissions()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }
    }
}
