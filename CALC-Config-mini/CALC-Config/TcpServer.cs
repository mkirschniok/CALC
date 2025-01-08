using System.Net;
using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;
#if ANDROID
using Android.Widget;
#endif

namespace CALC_Config
{
    /// <summary>
    /// Klasa odpowiedzialna za logikę serwera WebSocket.
    /// </summary>
    public class TcpServer
    {
        /// <summary>
        /// HttpListener serwera
        /// </summary>
        private HttpListener? _httpListener;
        /// <summary>
        /// Słownik (lista) klientów połączonych z serwerem - klucz to identyfikator klienta, wartość to obiekt WebSocket
        /// </summary>
        public Dictionary<string, WebSocket> _clients = new();
        /// <summary>
        /// Delegat wywoływany po dodaniu nowego urządzenia
        /// </summary>
        public static Action DeviceAdded = delegate { };
        /// <summary>
        /// Akcja wywoływana po otrzymaniu nowej wartości (liczby lub operatora)
        /// </summary>
        public static Action<string?, string?>? UpdateValue;

        /// <summary>
        /// Konstruktor klasy TcpServer, dodaje obsługę zdarzenia wysłania konfiguracji i wyniku
        /// </summary>
        public TcpServer()
        {
            MainPage.SendConfig += async (config) => await SendConfig(config);
            Preview.SendResult += async (result, id) => await SendResult(result, id);
        }

        /// <summary>
        /// Metoda uruchamiająca serwer WebSocket
        /// </summary>
        public async void StartServer()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://*:8080/ws/");
            _httpListener.Start();
            DisplayMessage("Serwer uruchomiono!");
            int i = 1;
            while (true)
            {
                var context = await _httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var clientSocket = wsContext.WebSocket;
                    JObject id = new JObject
                    {
                        { "type", "id" },
                        { "id", i.ToString() }
                    };
                    var idString = id.ToString();
                    _clients.Add(i.ToString(), clientSocket);
                    i++;
                    await clientSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(idString)), WebSocketMessageType.Text, true, CancellationToken.None);
                    DeviceAdded.Invoke();
                    _ = HandleClient(clientSocket, idString);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        /// <summary>
        /// Metoda obsługująca komunikację z klientem
        /// </summary>
        /// <param name="clientSocket">obiekt WebSocket</param>
        /// <param name="id">identyfikator klienta</param>
        /// <returns></returns>
        private async Task HandleClient(WebSocket clientSocket, string id)
        {
            var buffer = new byte[1024 * 4];
            while (clientSocket.State == WebSocketState.Open)
            {
                var result = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _clients.Remove(id);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    JObject json = JObject.Parse(message);
                    if (json["type"]?.ToString() == "update")
                    {
                        UpdateValue?.Invoke(json["role"]?.ToString(), json["value"]?.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Metoda wysyłająca konfigurację do wszystkich klientów
        /// </summary>
        /// <param name="config">obiekt JSON z danymi konfiguracji</param>
        /// <returns></returns> 
        public async Task SendConfig(JObject config)
        {
            int i = 0;
            foreach (var client in _clients.ToList())
            {
                JObject tempConfig = config;
                if (i == 0) tempConfig["role"] = "arg1";
                else if (i == 1) tempConfig["role"] = "oper";
                else if (i == 2) tempConfig["role"] = "arg2";
                else if (i == 3) tempConfig["role"] = "res";
                if (client.Value.State == WebSocketState.Open)
                {
                    var bytes = Encoding.UTF8.GetBytes(tempConfig.ToString());
                    await client.Value.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                i++;
            }
        }

        /// <summary>
        /// Metoda wysyłająca wynik do urządzenia o podanym identyfikatorze
        /// </summary>
        /// <param name="result">string z wynikiem</param>
        /// <param name="id">identyfikator urządzenia</param>
        /// <returns></returns>
        public async Task SendResult(string result, string id)
        {
            WebSocket client = _clients[id];
            JObject json = new JObject
            {
                { "type", "result" },
                { "value", result }
            };
            var bytes = Encoding.UTF8.GetBytes(json.ToString());
            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Metoda wyświetlająca komunikat (Toast na Androidzie) na ekranie
        /// </summary>
        /// <param name="message">wiadomość</param>
        private void DisplayMessage(string message)
        {
#if ANDROID
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short)?.Show();
#endif
        }
    }
}
