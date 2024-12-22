using Shiny.BluetoothLE.Hosting;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;

namespace CALC
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Dark;
            Scanner.GotData += JumpToConnectionScreen;
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Scanner());
            });
        }

        private async void OnManualClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Manual());
            });
        }

        private void JumpToConnectionScreen(string ssid, string password, string ip)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Manual(ssid, password, ip));
            });
        }
    }

}
