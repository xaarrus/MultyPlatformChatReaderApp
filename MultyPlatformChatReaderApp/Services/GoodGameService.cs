using MultyPlatformChatReaderApp.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static MultyPlatformChatReaderApp.Models.AllChatMessage;
using static MultyPlatformChatReaderApp.Models.GGWSCommand;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;
using static MultyPlatformChatReaderApp.Models.PortalGGModel.UserGGModel;
using static MultyPlatformChatReaderApp.Models.SettingModel;

namespace MultyPlatformChatReaderApp.Services
{
    public class GoodGameService : IGoodGameService
    {
        public UserLoginFormGGModel AuthUserForm { get; set; } = new();
        public UserGGModel GoodGameUser { get; set; } = new();
        public ClientWebSocket ClientGoodGame { get; set; } = new();

        public delegate void GetNewMessage(FromService serviceName, string fromUserName, List<ChatMessage.MessageWordsAndSmiles> messageWAS);
        public event IGoodGameService.GetNewMessage OnMessageReceive;
        public List<GGSmilesLibrary> AllSmilesGoodGame { get; set; } = new();
        private readonly StoreService _storeService;
        public GoodGameService(StoreService storeService)
        {
            _storeService = storeService;
            if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.UserGGLogin.username))
            {
                AuthUserForm.username = _storeService.SettingApp.SettingsGG.UserGGLogin.username;
                AuthUserForm.password = _storeService.SettingApp.SettingsGG.UserGGLogin.password;                
            }
            if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.nickname))
            {
                GetSmilesGoodGame();
            }
        }
        public async Task Connect()
        {
            if (ClientGoodGame != null)
            {
                if (ClientGoodGame.State == WebSocketState.Open)
                {
                    await ClientGoodGame.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    OnMessageReceive?.Invoke(FromService.sys, "sys",
                        new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Отключился от GoodGame."}
                        });
                }
            }
            if (_storeService.SettingApp.SettingsGG.LogInUser.success)
            {
                using (ClientGoodGame = new ClientWebSocket())
                {
                    await ClientGoodGame.ConnectAsync(new Uri("wss://chat.goodgame.ru/chat/websocket"), CancellationToken.None);
                    if (ClientGoodGame.State == WebSocketState.Open)
                    {
                        string commandJoinGGOnId = JsonConvert.SerializeObject(new JoinChatCommand() { data = { channel_id = _storeService.SettingApp.SettingsGG.LogInUser.user.channelInfo.idi } });
                        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(commandJoinGGOnId));
                        await ClientGoodGame.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                        await Task.Delay(100);
                        OnMessageReceive?.Invoke(FromService.sys, "sys",
                            new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord ="Подключился к GoodGame."} 
                            });
                    }
                    await Task.WhenAll(Receive(ClientGoodGame));
                }
            }            
        }

        public async Task Receive(ClientWebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[91920]);
                var result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
                string responseFromWS = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                WSResponseType typeResponse = JsonConvert.DeserializeObject<WSResponseType>(responseFromWS);
                if (typeResponse.type == "message")
                {
                    GetMessageResponse responseMessage = JsonConvert.DeserializeObject<GetMessageResponse>(responseFromWS);
                    if (AllSmilesGoodGame.Count > 0)
                    {
                        var ListGGMessageEnd = new List<ChatMessage.MessageWordsAndSmiles>();
                        string[] arMessageGGWord = responseMessage.data.text.Split(' ', ':');

                        for (int z = 0; z < arMessageGGWord.Count(); z++)
                        {
                            var MessageWordGG = new ChatMessage.MessageWordsAndSmiles();
                            var MessageSmileGG = new ChatMessage.MessageWordsAndSmiles();
                            GGSmilesLibrary smileGG = AllSmilesGoodGame.FirstOrDefault(x => x.key == arMessageGGWord[z]);
                            if (smileGG != null && smileGG.images.big != null)
                            {
                                MessageSmileGG.SmileUrl = smileGG.images.big;
                                ListGGMessageEnd.Add(MessageSmileGG);
                            }
                            else
                            {
                                var MessageWordGGBadSmile = new ChatMessage.MessageWordsAndSmiles();
                                if (arMessageGGWord[z].Length > 0)
                                {
                                    MessageWordGGBadSmile.MessageWord = arMessageGGWord[z] + " ";
                                    ListGGMessageEnd.Add(MessageWordGGBadSmile);
                                }
                            }
                        }
                        OnMessageReceive?.Invoke(FromService.GoodGame, responseMessage.data.user_name, ListGGMessageEnd);
                    }
                    else
                    {                        
                        OnMessageReceive?.Invoke(FromService.GoodGame, responseMessage.data.user_name,
                            new List<ChatMessage.MessageWordsAndSmiles>() { 
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord = responseMessage.data.text } 
                            });
                    }
                }
                if (typeResponse.type == "premium")
                {
                    GetPremiumSubscribeResponse newSubscriber = JsonConvert.DeserializeObject<GetPremiumSubscribeResponse>(responseFromWS);
                    OnMessageReceive?.Invoke(FromService.GoodGame, newSubscriber.data.userName, new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() { MessageWord = "Подписался на вас!" + 
                                (newSubscriber.data.resub.Length > 0? (" Переподписан " + newSubscriber.data.resub + " месяц"):"") 
                                }});
                }
                if (typeResponse.type == "payment")
                {
                    GetDonateResponse newDonate = JsonConvert.DeserializeObject<GetDonateResponse>(responseFromWS);
                    string messageFromDonate = "";
                    if (string.IsNullOrEmpty(newDonate.data.message))
                    {
                        messageFromDonate = "Пожертвовал: " + newDonate.data.amount;
                    }
                    else
                    {
                        messageFromDonate = "Пожертвовал: " + newDonate.data.amount + " и сообщает: " + newDonate.data.message;
                    }
                    OnMessageReceive?.Invoke(FromService.GoodGame, newDonate.data.userName, new List<ChatMessage.MessageWordsAndSmiles>() {
                                new ChatMessage.MessageWordsAndSmiles() {                                     
                                    MessageWord = messageFromDonate }

                            });
                }
            }
        }
        public async Task Send(string message)
        {
            if (ClientGoodGame.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await ClientGoodGame.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);                
                await Task.Delay(100);
            }
        }
        public async Task AuthInGoodGame()
        {
            HttpClient http = new HttpClient();
            string json = JsonConvert.SerializeObject(AuthUserForm);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PostAsync("https://goodgame.ru/api/4/login/password", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var GetUserSucces = JsonConvert.DeserializeObject<UserGGModel>(result);
                ChannelId GetUserChannelSucces = new ChannelId();
                try
                {
                    GetUserChannelSucces = JsonConvert.DeserializeObject<ChannelId>(result);
                }
                catch (Exception)
                {
                    MessageBox.Show("У вас нет канала!", "GoodGame", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (GetUserSucces.success & GetUserChannelSucces.user.channel.idi > 0)
                {
                    GetUserSucces.user.channelInfo.id = GetUserChannelSucces.user.channel.id;
                    SettingGG saveSettingUserGG = new SettingGG();
                    saveSettingUserGG.LogInUser = GetUserSucces;
                    saveSettingUserGG.UserGGLogin = AuthUserForm;
                    _storeService.SaveSettings(saveSettingUserGG);
                    await Connect();
                }
                else
                {
                    MessageBox.Show(GetUserSucces.error,
                        "GoodGame", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (GetUserSucces.success & GetUserChannelSucces.user.channel.idi == 0)
                {
                    MessageBox.Show("У вас нет канала!",
                        "GoodGame", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Неудача: " + response.StatusCode + response.Content.ToString(),
                    "GoodGame", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }        
        public async Task GetSmilesGoodGame()
        {
            HttpClient http = new HttpClient();
            string getUrl = "https://goodgame.ru/api/4/smiles";
            var response = await http.GetAsync(getUrl);
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                AllSmilesGoodGame = JsonConvert.DeserializeObject<List<GGSmilesLibrary>>(result);               
            }            
        }        
    }
}
