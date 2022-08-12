using MultyPlatformChatReaderApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MultyPlatformChatReaderApp.Models.AllChatMessage;
using static MultyPlatformChatReaderApp.Models.TrovoChatMessagesModel;

namespace MultyPlatformChatReaderApp.Services
{
    public class TrovoApiService
    {
        private TrovoResourceModel TrResource = new TrovoResourceModel();
        public TrovoUserInfo NewUserInfo = new();
        public TrovoListSubscribers TempListSubscribers = new();
        public TrovoUser Truser = new TrovoUser();
        public TrovoUserLocalInfo TempInfo = new TrovoUserLocalInfo();
        public TrovoEmotes AllSmilesTrovo = new TrovoEmotes();
        public ClientWebSocket ClientTrovo = new();
        public long StartTimeUnix = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        public delegate void GetNewMessage(FromService serviceName, string fromUserName, List<ChatMessage.MessageWordsAndSmiles> messageWAS);
        public event GetNewMessage OnMessageReceive;
        private readonly StoreService _storeService;
        public TrovoApiService(StoreService storeService)
        {
            _storeService = storeService;
            Truser = _storeService.SettingApp.SettingsTr.TrovoUserLogIn;
            GetTrovoSmiles();            
        }
        public async Task Connect()
        {
            if (ClientTrovo.State == WebSocketState.Open)
            {
                await ClientTrovo.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                OnMessageReceive?.Invoke(FromService.sys, "sys", new List<ChatMessage.MessageWordsAndSmiles>() {
                    new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Отключился от Trovo."}});
            }
            if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token) & !_storeService.SettingApp.SettingsTr.TrovoUserLogIn.CheckNeedUpdateTokenTrovo())
            {
                TrovoChatToken _trovoChatToken = new TrovoChatToken();
                while (_trovoChatToken.token == null)
                {
                    _trovoChatToken = await GetChatToken();
                    await Task.Delay(500);
                }

                using (ClientTrovo = new ClientWebSocket() { Options = { KeepAliveInterval = TimeSpan.FromSeconds(30) } })
                {
                    await ClientTrovo.ConnectAsync(new Uri("wss://open-chat.trovo.live/chat"), CancellationToken.None);
                    string commandAuth = "{\"type\":\"AUTH\",\"nonce\":\"Hi\",\"data\":{\"token\":\"" + _trovoChatToken.token + "\"}}";
                    ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(commandAuth));
                    await ClientTrovo.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                    if (ClientTrovo.State == WebSocketState.Open)
                    {
                        OnMessageReceive?.Invoke(FromService.sys, "sys", new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Подключился к Trovo."}
                        });
                        await CheckListSubscriber();
                    }
                    while (ClientTrovo.State == WebSocketState.Open & !string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token))
                    {
                        ArraySegment<byte> bytesReceivedFromTrovows = new ArraySegment<byte>(new byte[40960]);
                        WebSocketReceiveResult resultTrovo = await ClientTrovo.ReceiveAsync(bytesReceivedFromTrovows, CancellationToken.None);
                        string responseFromWSTrovo = Encoding.UTF8.GetString(bytesReceivedFromTrovows.Array, 0, resultTrovo.Count);
                        TrovoMessage responseMessage = JsonConvert.DeserializeObject<TrovoMessage>(responseFromWSTrovo);
                        if (responseMessage.type == "CHAT")
                        {
                            for (int i = 0; i < responseMessage.data.chats.Count(); i++)
                            {
                                if (AllSmilesTrovo != null)
                                {
                                    string[] _arTrovoMessage = responseMessage.data.chats[i].content.Split(' ', ':');
                                    var ListTrovoMessageEnd = new List<ChatMessage.MessageWordsAndSmiles>();
                                    for (int z = 0; z < _arTrovoMessage.Count(); z++)
                                    {
                                        var MessageSmileTrovo = new ChatMessage.MessageWordsAndSmiles();
                                        Emote smileTrovoCustom = new Emote();
                                        if (AllSmilesTrovo.channels.customizedEmotes != null)
                                        {
                                            var smileTrovoCustomChannel = AllSmilesTrovo.channels.customizedEmotes.channel.FirstOrDefault(x => x.emotes.FirstOrDefault(e => e.name == _arTrovoMessage[z]) != null);
                                            smileTrovoCustom = smileTrovoCustomChannel.emotes.FirstOrDefault(x => x.name == _arTrovoMessage[z]);
                                        }
                                        var smileTrovoEvent = AllSmilesTrovo.channels.eventEmotes.FirstOrDefault(x => x.name == _arTrovoMessage[z]);
                                        var smileTrovoGlobal = AllSmilesTrovo.channels.globalEmotes.FirstOrDefault(x => x.name == _arTrovoMessage[z]);
                                        if (smileTrovoCustom?.url != null | smileTrovoEvent != null | smileTrovoGlobal != null)
                                        {
                                            if (smileTrovoCustom?.url != null)
                                            {
                                                MessageSmileTrovo.SmileUrl = smileTrovoCustom.url;
                                                ListTrovoMessageEnd.Add(MessageSmileTrovo);
                                            }
                                            if (smileTrovoEvent != null)
                                            {
                                                MessageSmileTrovo.SmileUrl = smileTrovoEvent.url;
                                                ListTrovoMessageEnd.Add(MessageSmileTrovo);
                                            }
                                            if (smileTrovoGlobal != null)
                                            {
                                                MessageSmileTrovo.SmileUrl = smileTrovoGlobal.url;
                                                ListTrovoMessageEnd.Add(MessageSmileTrovo);
                                            }
                                        }
                                        else
                                        {
                                            var MessageWordTrovoBadSmile = new ChatMessage.MessageWordsAndSmiles();
                                            if (_arTrovoMessage[z].Length > 0)
                                            {
                                                MessageWordTrovoBadSmile.MessageWord = _arTrovoMessage[z] + " ";
                                                ListTrovoMessageEnd.Add(MessageWordTrovoBadSmile);
                                            }
                                        }
                                    }
                                    OnMessageReceive?.Invoke(FromService.Trovo, responseMessage.data.chats[i].nick_name, ListTrovoMessageEnd);
                                }
                                else
                                {
                                    OnMessageReceive?.Invoke(FromService.Trovo, responseMessage.data.chats[i].nick_name, new List<ChatMessage.MessageWordsAndSmiles>() {
                                        new ChatMessage.MessageWordsAndSmiles() { MessageWord =responseMessage.data.chats[i].content}});
                                }
                            }
                        }
                    }
                    await ClientTrovo.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    OnMessageReceive?.Invoke(FromService.sys, "sys", new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Отключился от Trovo."}});
                }
            }
        }
        public async Task GetTrovoToken() 
        {
            string allScope = String.Join("+", TrResource.allScopeTrovo);
            string scopetr = Uri.EscapeUriString(allScope);
            var http = new HttpListener();
            http.Prefixes.Add(TrResource.authorizationEndpointLocal + "/");
            http.Start();
            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}",
            TrResource.authorizationEndpointTrovo,
            scopetr,
            System.Uri.EscapeDataString(TrResource.authorizationEndpointLocal),
            TrResource.clientID_tr);
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = authorizationRequest,
                    UseShellExecute = true
                }
            };
            p.Start();
            var context = await http.GetContextAsync();
            var response = context.Response;
            string responseString = string.Format("<html><head><meta charset='utf-8'></head><body style='background-color:#158171; color:black'>Please return to the app. Пора вернуться в программу.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server остановлен.");
            });
            if (context.Request.QueryString.Get("error") != null)
            {
                Console.WriteLine(String.Format("OAuth ошибка авторизации: {0}.", context.Request.QueryString.Get("error")));
            }
            var code = context.Request.QueryString.Get("code");
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("client-id", TrResource.clientID_tr);
            HttpContent content = new StringContent("{\"client_secret\": \"" + TrResource.clientSecret_tr + "\",\"grant_type\": \"authorization_code\",\"code\":\"" + code + 
                "\",\"redirect_uri\": \""+ TrResource.authorizationEndpointLocal + "\"}", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseExtoken = await _httpClient.PostAsync(TrResource.tokenRequestURITrovo, content);
            if (responseExtoken.IsSuccessStatusCode)
            {
                string resultExToken = await responseExtoken.Content.ReadAsStringAsync();
                Dictionary<string, object> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultExToken);
                var access_token = tokenEndpointDecoded["access_token"];
                Truser = JsonConvert.DeserializeObject<TrovoUser>(resultExToken);
                Truser.Issued = DateTime.Now;
                _storeService.SaveSettings(Truser);
            }
        }
        public async Task RefreshTrovoToken() 
        {
            TrovoUser tempTruser = new TrovoUser();
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Add("client-id", TrResource.clientID_tr);

            HttpContent content = new StringContent("{\"client_secret\": \"" + TrResource.clientSecret_tr + "\", \"grant_type\": \"refresh_token\", \"refresh_token\": \"" + Truser.refresh_token + "\"}",
                Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await http.PostAsync("https://open-api.trovo.live/openplatform/refreshtoken", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                tempTruser = JsonConvert.DeserializeObject<TrovoUser>(result);
                Truser.access_token = tempTruser.access_token;
                Truser.refresh_token = tempTruser.refresh_token;
                Truser.Issued = DateTime.Now;
                _storeService.SaveSettings(Truser);
            }
            else
            {
                _storeService.SaveSettings(tempTruser);
            }
        }
        public async Task CheckTrToken() 
        {
            if (string.IsNullOrEmpty(Truser.access_token)) { await GetTrovoToken(); }
            if (Truser.CheckNeedUpdateTokenTrovo()) { await RefreshTrovoToken(); }
            Truser = _storeService.SettingApp.SettingsTr.TrovoUserLogIn;
            if (ClientTrovo.State != WebSocketState.Open){ await Connect(); }
        }
        public async Task<TrovoUserInfo> GetTrovoUserInfo()
        {
            NewUserInfo = new TrovoUserInfo();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);
            _httpClient.DefaultRequestHeaders.Add("Authorization", ("OAuth " + Truser.access_token));            
            var response = await _httpClient.GetAsync("https://open-api.trovo.live/openplatform/getuserinfo");
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                NewUserInfo = JsonConvert.DeserializeObject<TrovoUserInfo>(resultResponse);
            }
            return NewUserInfo;
        }
        public async Task<TrovoChannelInfo> GetTrovoChannelInfo(int? channel_id = null, string? username = null)
        {
            TrovoChannelInfo temp = new TrovoChannelInfo();
            if (channel_id > 0 | username != null)
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);

                HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
                if (channel_id > 0)
                {
                    string SearchOnId = "{\"channel_id\":"+channel_id+"}";
                    content = new StringContent(SearchOnId, Encoding.UTF8, "application/json");
                }
                if (username != null)
                {
                    string SearchOnUserName = "{\"username\":\""+username+"\"}";
                    content = new StringContent(SearchOnUserName, Encoding.UTF8, "application/json");
                }                
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PostAsync("https://open-api.trovo.live/openplatform/channels/id", content);
                if (response.IsSuccessStatusCode)
                {
                    string resultResponse = await response.Content.ReadAsStringAsync();
                    temp = JsonConvert.DeserializeObject<TrovoChannelInfo>(resultResponse);
                }
            }
            return temp;
        }
        public async Task<TrovoChatToken> GetChatToken()
        {
            TrovoChatToken NewChatToken = new TrovoChatToken();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);
            _httpClient.DefaultRequestHeaders.Add("Authorization", ("OAuth " + Truser.access_token));
            var response = await _httpClient.GetAsync("https://open-api.trovo.live/openplatform/chat/token");
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                NewChatToken = JsonConvert.DeserializeObject<TrovoChatToken>(resultResponse);
            }
            return NewChatToken;
        }
        public async Task<TrovoEmotes> GetEmotes(string? channelid = null)
        {
            TrovoEmotes _TrovoEmotes = new TrovoEmotes();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);

            HttpContent content = new StringContent("{\"emote_type\":0,\"emote_type\":1,\"emote_type\":2}", Encoding.UTF8, "application/json");
            if (channelid != null)
            {
                content = new StringContent("{\"emote_type\":0,\"emote_type\":1,\"emote_type\":2, \"channel_id\":["+channelid+"]}", Encoding.UTF8, "application/json");
            }
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync("https://open-api.trovo.live/openplatform/getemotes", content);
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                _TrovoEmotes = JsonConvert.DeserializeObject<TrovoEmotes>(resultResponse);
            }
            return _TrovoEmotes;
        }
        public async Task GetTrovoSmiles()
        {
            AllSmilesTrovo = await GetEmotes();
        }
        public async Task CheckListSubscriber()
        {
            while (ClientTrovo.State == WebSocketState.Open)
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);
                _httpClient.DefaultRequestHeaders.Add("Authorization", ("OAuth " + Truser.access_token));
                string getUrl = "https://open-api.trovo.live/openplatform/channels/" + NewUserInfo.channelId + "/subscriptions?limit=100&offset=0&direction=desc";
                var response = await _httpClient.GetAsync(getUrl);
                if (response.IsSuccessStatusCode)
                {
                    string resultResponse = await response.Content.ReadAsStringAsync();
                    TrovoListSubscribers responseTrovoListSubscribers = JsonConvert.DeserializeObject<TrovoListSubscribers>(resultResponse);
                    foreach (var item in responseTrovoListSubscribers.subscriptions)
                    {
                        if (TempListSubscribers.subscriptions.Any(x=>x.user.display_name != item.user.display_name & StartTimeUnix < long.Parse(item.user.created_at)))
                        {
                            TempListSubscribers.subscriptions.Add(item);
                            OnMessageReceive?.Invoke(FromService.Trovo, item.user.display_name, new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Оформил подписку уровня " + item.sub_tier}});
                        }
                    }
                    TempListSubscribers.total = TempListSubscribers.subscriptions.Count;                    
                }                
                await Task.Delay(10000);
            }
        }
    }
}
