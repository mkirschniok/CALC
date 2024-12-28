#if ANDROID
using Android.Widget;
#endif
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace CALC
{
    /// <summary>
    /// Klasa klienta TCP (WebSocket)
    /// </summary>
    public class TcpClientApp
    {
        /// <summary>
        /// Klient WebSocket
        /// </summary>
        private ClientWebSocket         _webSocket;
        /// <summary>
        /// Akcja wywoływana po otrzymaniu identyfikatora
        /// </summary>
        public static Action<string>?   IDReceived;
        /// <summary>
        /// Akcja wywoływana po otrzymaniu konfiguracji
        /// </summary>
        public static Action<JObject>?  ConfigReceived;
        /// <summary>
        /// Akcja wywoływana po otrzymaniu wyniku
        /// </summary>
        public static Action<string>?   ResultReceived;

        /// <summary>
        /// Konstruktor klasy TcpClientApp
        /// </summary>
        public TcpClientApp()
        {
            Argument.SendUpdate += async (config, id, value) => await SendUpdate(config, id, value);
            Operator.SendUpdate += async (config, id, value) => await SendUpdate(config, id, value);
        }
        /// <summary>
        /// Metoda łącząca z tabletem
        /// </summary>
        /// <param name="ip">adres IP jako string</param>
        public async void ConnectToServer(string ip)
        {
            _webSocket = new ClientWebSocket();
            try
            {
                await _webSocket.ConnectAsync(new Uri("ws://" + ip + ":8080/ws/"), CancellationToken.None);
                DisplayMessage("Połączono z tabletem");
                _ = ListenToServer();
            }
            catch (Exception ex)
            {
                DisplayMessage($"Błąd przy łączeniu z tabletem: {ex.Message}");
            }
        }

        /// <summary>
        /// Metoda nasłuchująca serwera
        /// </summary>
        /// <returns></returns>
        private async Task ListenToServer()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    JObject messageJson = JObject.Parse(message);
                    switch (messageJson["type"].ToString())
                    {
                        case "id":
                            IDReceived.Invoke(messageJson["id"].ToString());
                            break;
                        case "config":
                            ConfigReceived.Invoke(messageJson);
                            break;
                        case "result":
                            ResultReceived.Invoke(messageJson["value"].ToString());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Metoda wysyłająca wiadomość
        /// </summary>
        /// <param name="message"></param>
        private async void SendMessage(string message)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// Metoda wysyłająca aktualizację danych
        /// </summary>
        /// <param name="config">obiekt konfiguracji</param>
        /// <param name="id">identyfikator urządzenia</param>
        /// <param name="value">wartość do zaktualizowania</param>
        /// <returns></returns>
        private async Task SendUpdate(JObject config, string id, string value)
        {
            JObject message = new JObject
            {
                { "type", "update" },
                { "id", id },
                { "role", config["role"] },
                { "value", value }
            };
            SendMessage(message.ToString());
        }

        /// <summary>
        /// Metoda wyświetlająca wiadomość na ekranie (Toast na Androidzie)
        /// </summary>
        /// <param name="message">wiadomość</param>
        private void DisplayMessage(string message)
        {
#if ANDROID
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
            });
#endif
        }

    }
}
