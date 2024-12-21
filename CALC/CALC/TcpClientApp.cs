#if ANDROID
using Android.Widget;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CALC
{
    public class TcpClientApp
    {
        private ClientWebSocket _webSocket;
        public static Action<string> IDReceived;
        public static Action<JObject> ConfigReceived;
        public static Action<string> ResultReceived;

        public TcpClientApp()
        {
            Argument.SendUpdate += async (config, id, value) => await SendUpdate(config, id, value);
            Operator.SendUpdate += async (config, id, value) => await SendUpdate(config, id, value);
            Result.RequestUpdate += async id => await RequestUpdate(id);
        }
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

        private async void SendMessage(string message)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

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

        private async Task RequestUpdate(string id)
        {
            JObject message = new JObject
            {
                { "type", "request" },
                { "id", id }
            };
            var test = message.ToString();
            SendMessage(message.ToString());
        }

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
