namespace CALC
{
    /// <summary>
    /// Klasa ekranu głównego aplikacji
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Konstruktor klasy MainPage
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Dark;
            Scanner.GotData += JumpToConnectionScreen;
        }

        /// <summary>
        /// Metoda wywoływana po naciśnięciu przycisku "Skanuj kod QR"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnScanClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Scanner());
            });
        }

        /// <summary>
        /// Metoda wywoływana po naciśnięciu przycisku "Wprowadź dane sieci ręcznie"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnManualClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Manual());
            });
        }

        /// <summary>
        /// Metoda przenosząca do ekranu połączenia z tabletem
        /// </summary>
        /// <param name="ssid">nazwa sieci</param>
        /// <param name="password">hasło</param>
        /// <param name="ip">adres ip jako string</param>
        private void JumpToConnectionScreen(string ssid, string password, string ip)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Manual(ssid, password, ip));
            });
        }
    }

}
