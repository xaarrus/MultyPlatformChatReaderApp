using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MultyPlatformChatReaderApp.Models.AllChatMessage;
using static MultyPlatformChatReaderApp.Models.WASD.WASDModels;
using static MultyPlatformChatReaderApp.Models.WASD.WASDWebSocketModels;

namespace MultyPlatformChatReaderApp.Services
{
    public class WASDService
    {
        public ClientWebSocket ClientWASD { get; set; } = new ClientWebSocket();
        public ResponseChatToken ChatTokenWASD = new ResponseChatToken();
        public string UserAlias = "";
        public ResponseV2BroadcastsPublic UserInfo = new ResponseV2BroadcastsPublic();
        public delegate void GetNewMessage(FromService serviceName, string fromUserName, List<ChatMessage.MessageWordsAndSmiles> messageWAS);
        public event GetNewMessage OnMessageReceive;
        public WASDService()
        {
            GetChatToken();
            UpdateUserInfo();
            CheckStartStream();
        }
        public async Task Start()
        {
            if (UserAlias.Length < 4)
            {
                return;
            }

            if (ChatTokenWASD.result.Length > 1 & UserInfo.result.media_container?.media_container_streams.Count > 0)
            {
                await Connect();
            }
        }
        public async Task Connect()
        {
            if (ClientWASD != null)
            {
                if (ClientWASD.State == WebSocketState.Open)
                {
                    await ClientWASD.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    OnMessageReceive?.Invoke(FromService.sys, "sys",
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Отключился от WASD."}
                        });
                    return;
                }
            }
            using (ClientWASD = new ClientWebSocket())
            {
                await ClientWASD.ConnectAsync(new Uri("wss://chat.wasd.tv/socket.io/?EIO=3&transport=websocket"), CancellationToken.None);
                if (ClientWASD.State == WebSocketState.Open)
                {
                    OnMessageReceive?.Invoke(FromService.sys, "sys",
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Подключился к WASD."}
                        });
                }
                await Task.WhenAll(Receive(ClientWASD), HeartBeat(ClientWASD));
            }
        }
        public async Task Receive(ClientWebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[91920]);
                var result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
                string responseFromWS = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                string[] searchTypeReceiveCommand = responseFromWS.Split('"');
                if (searchTypeReceiveCommand.Length > 2)
                {
                    if (searchTypeReceiveCommand[1] == "message")
                    {
                        OnMessageReceive?.Invoke(FromService.WASD, searchTypeReceiveCommand[15],
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord = searchTypeReceiveCommand[11]}
                        });
                    }
                    if (searchTypeReceiveCommand[1] == "sticker")
                    {
                        OnMessageReceive?.Invoke(FromService.WASD, searchTypeReceiveCommand[41],
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { SmileUrl = searchTypeReceiveCommand[25]}
                        });
                    }
                }
                if (responseFromWS == "40")
                { await JoinChatRoom(); }


                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    OnMessageReceive?.Invoke(FromService.sys, "sys",
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Отключился от WASD."}
                        });
                }
            }
        }
        public async Task Send(string message)
        {
            if (ClientWASD.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await ClientWASD.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(100);
            }
        }
        public async Task HeartBeat(ClientWebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                await Send("2");
                await Task.Delay(23000);
            }
        }
        public async Task GetChatToken()
        {
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", APIkeyWASD);
            string nullContent = "";
            HttpContent content = new StringContent(nullContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://wasd.tv/api/auth/chat-token", content);
            string stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                ChatTokenWASD = JsonConvert.DeserializeObject<ResponseChatToken>(stringResponse);
            }
        }
        public async Task<ResponseV2BroadcastsPublic> GetUserInfo()
        {
            ResponseV2BroadcastsPublic temp = new ResponseV2BroadcastsPublic();
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://wasd.tv/api/v2/broadcasts/public?channel_name=" + UserAlias);
            string stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                temp = JsonConvert.DeserializeObject<ResponseV2BroadcastsPublic>(stringResponse);
            }
            return temp;
        }
        public async void UpdateUserInfo()
        {
            while (true)
            {
                UserInfo = await GetUserInfo();
                await Task.Delay(2000);
            }
            
        }
        public async void CheckStartStream()
        {
            while (true)
            {
                if (ChatTokenWASD.result.Length > 1 & UserInfo.result.media_container?.media_container_streams.Count > 0)
                {
                    if (ClientWASD != null & ClientWASD?.State != WebSocketState.Open)
                    {
                        await Connect();
                    }                    
                }
                await Task.Delay(2000);
            }
            
        }
        public string JoinChatCommand(int streamId, int channelId, string token)
        {
            return "42[\"join\",{\"streamId\":" + streamId + ",\"channelId\":" + channelId + ",\"jwt\":\"" + token + "\",\"excludeStickers\":true}]";
        }
        public async Task JoinChatRoom()
        {
            await Send(JoinChatCommand(UserInfo.result.media_container.media_container_streams[0].stream_id, UserInfo.result.channel.channel_id, ChatTokenWASD.result));
        }
    }
}
