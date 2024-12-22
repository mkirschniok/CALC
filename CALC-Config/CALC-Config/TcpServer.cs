using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;


#if ANDROID
using Android.Widget;
#endif

namespace CALC_Config
{
    public class TcpServer
    {
        private HttpListener _httpListener;
        public Dictionary<string, WebSocket> _clients = new();
        public static Action DeviceAdded = delegate { };
        public static Action<string, string> UpdateValue;
        public static Action<string> UpdateResult;

        public TcpServer()
        {
            MainPage.SendConfig += async (config) => await SendConfig(config);
            Preview.SendResult += async (result, id) => await SendResult(result, id);
        }
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
                    // Send the client its ID
                    // Create a JSON object with the client ID
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
                    // Convert the message from JSON to a string
                    JObject json = JObject.Parse(message);
                    if (json["type"].ToString() == "update")
                    {
                        UpdateValue.Invoke(json["role"].ToString(), json["value"].ToString());
                    }
                    else if (json["type"].ToString() == "request")
                    {
                        UpdateResult.Invoke(json["id"].ToString());
                    }
                    await BroadcastMessage(message);
                }
            }
        }

        private async Task BroadcastMessage(string message)
        {
            foreach (var client in _clients.ToList())
            {
                if (client.Value.State == WebSocketState.Open)
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await client.Value.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

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

        private void DisplayMessage(string message)
        {
            // Generate a toast message
#if ANDROID
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
#endif
        }
    }
}
