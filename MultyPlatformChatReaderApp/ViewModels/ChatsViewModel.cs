using Google.Apis.YouTube.v3.Data;
using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Interface;
using MultyPlatformChatReaderApp.Models;
using MultyPlatformChatReaderApp.Services;
using MultyPlatformChatReaderApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Communication.Events;
using static MultyPlatformChatReaderApp.Models.AllChatMessage;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class ChatsViewModel : BaseViewModel
    {
        private TwitchApiService _twapiService;
        private StoreService _storeService;
        private YouTubeAppService _ytService;
        public TrovoApiService _trapiService;
        public GoodGameService _ggService;
        public WASDService _wasdService;

        private static ChatsWindow CW;

        private List<LiveChatMessage> ListAllYTMessage = new List<LiveChatMessage>();
        public string _YTchatId = String.Empty;
        
        public TwitchUserLocalInfo TempInfo = new TwitchUserLocalInfo();

        public List<GGSmilesLibrary> AllSmilesGoodGame = new List<GGSmilesLibrary>();
        public ICommand ConnectToChatTW { get; set; }
        public ICommand ConnectToChatGG { get; set; }
        public ICommand ConnectToChatYT { get; set; }
        public ICommand ConnectToChatTR { get; set; }
        public ICommand ClearChat { get; set; }
        public ICommand ChachCommand { get; set; }
        public ObservableCollection<AllChatMessage.ChatMessage> Chat { get; set; } = new ObservableCollection<AllChatMessage.ChatMessage>();
        public ChatsViewModel(TwitchApiService twapiService, IGoodGameService ggService, StoreService storeService, YouTubeAppService ytService, TrovoApiService trapiService, WASDService wasdService)
        {
            _twapiService = twapiService;
            _storeService = storeService;
            _ytService = ytService;
            _trapiService = trapiService;
            _ggService = (GoodGameService)ggService;
            _wasdService = wasdService;

            _ggService.OnMessageReceive += AddMessageInChat;
            _trapiService.OnMessageReceive += AddMessageInChat;
            _wasdService.OnMessageReceive += AddMessageInChat;
            ConnectToChatTW = new AsyncCommand(async () =>
            {
                if (!_twapiService.TwitchClientListen.IsConnected)
                {
                    await StartTwClient();
                }
            }
            );
            ConnectToChatGG = new AsyncCommand(async () => await _ggService.Connect());
            ConnectToChatYT = new AsyncCommand(async () => await ListenYTChat());
            ConnectToChatTR = new AsyncCommand(async () => await _trapiService.Connect());
            ClearChat = new AsyncCommand(async () => await ClearChatMessages());
            ChachCommand = new AsyncCommand(async () => ChachWindow());

            _ = StartTwClient();
            _ = StartYTListen();
            _wasdService = wasdService;
        }
        private async Task StartYTListen()
        {
            while (_ytService.credential == null)
            {
                await Task.Delay(10000);
            }
            await ListenYTChat();
        }
        public async Task ListenYTChat()
        {
            while (_ytService.credential != null)
            {
                if (string.IsNullOrEmpty(_YTchatId))
                {
                    LiveBroadcastListResponse resultBroadcast = await _ytService.LiveBroadcastListAsync();
                    if (resultBroadcast != null)
                    {
                        foreach (var lbs in resultBroadcast.Items)
                        {
                            if (lbs != null && lbs.Status.LifeCycleStatus != "complete")
                            {
                                _YTchatId = lbs.Snippet.LiveChatId;
                            }
                        }
                    }
                }
                if (ListAllYTMessage.Count == 0)
                {
                    var startYtMes =new LiveChatMessage()
                    {
                        AuthorDetails = new LiveChatMessageAuthorDetails() { DisplayName = FromService.sys.ToString() },
                        Snippet = new LiveChatMessageSnippet() { DisplayMessage = "Подключился к YouTube.", PublishedAt = DateTime.Now }
                    };
                    ListAllYTMessage.Add(startYtMes);
                    AddMessageInChat(FromService.sys, startYtMes.AuthorDetails.DisplayName, startYtMes.Snippet.DisplayMessage);
                }
                
                IList<LiveChatMessage> newResponseYTLiveChatMessages = await _ytService.GetAllLiveChatMessages(_YTchatId);
                if (ListAllYTMessage != newResponseYTLiveChatMessages)
                {
                    foreach (var liveMes in newResponseYTLiveChatMessages)
                    {
                        if (ListAllYTMessage.Exists(x => x.AuthorDetails.DisplayName.Equals(liveMes.AuthorDetails.DisplayName)) == true
                            && ListAllYTMessage.Exists(x => x.Snippet.DisplayMessage.Equals(liveMes.Snippet.DisplayMessage)) == true
                            && ListAllYTMessage.Exists(x => x.Snippet.PublishedAt.Equals(liveMes.Snippet.PublishedAt)) == true)
                        { }
                        else
                        {
                            ListAllYTMessage.Add(liveMes);
                            AddMessageInChat(FromService.YouTube, liveMes.AuthorDetails.DisplayName, liveMes.Snippet.DisplayMessage);
                        }
                    }
                }
                await Task.Delay(10000);
            }
            if (_ytService.credential == null)
            {
                ListAllYTMessage = new List<LiveChatMessage>();
                AddMessageInChat(FromService.sys, FromService.sys.ToString(), "Отключился от YouTube.");
            }
            await StartYTListen();
        }
        private void ChachWindow()
        {
            if (Application.Current.Windows.Cast<Window>().Any(x => x == CW) == false)
            { CW = new ChatsWindow(); }
            if (CW.Visibility.ToString() == "Visible")
            { CW.Hide(); }
            else
            { CW.Show(); }
        }
        public async Task StartTwClient()
        {
            if (string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token) || _storeService.SettingApp.SettingsTw.TwitchUserLogIn.CheckNeedUpdateTokenTwitch())
            {
                while (string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token) || _storeService.SettingApp.SettingsTw.TwitchUserLogIn.CheckNeedUpdateTokenTwitch())
                {
                    await Task.Delay(1000);
                }
                await StartTwClient();
            }
            TempInfo = await _twapiService.TwStatusStream();
            if (!_twapiService.TwitchClientListen.IsConnected)
            {
                _twapiService.TwitchClientListen.OnConnected += Twitch_OnConnected;
                _twapiService.TwitchClientListen.OnDisconnected += Twitch_OnDisconnected;
                _twapiService.TwitchClientListen.OnMessageReceived += Twitch_OnMessageReceived;
                _twapiService.TwitchClientListen.OnNewSubscriber += Twitch_OnNewSubscriber;
                _twapiService.TwitchClientListen.OnReSubscriber += Twitch_OnReSubscriber;
                _twapiService.TwitchClientListen.Connect();
                await StartTwFollow();
            }            
        }
        public async Task StartTwFollow()
        {
            _twapiService.FollowerTWService = new FollowerService(_twapiService.TwitchAPISevice,60,10,1000);
            _twapiService.FollowerTWService.SetChannelsByName(new List<string> { TempInfo.TwitchName });
            _twapiService.FollowerTWService.OnNewFollowersDetected += FS_OnNewFollowerDetected;
            _twapiService.FollowerTWService.Start();
            await Task.Delay(-1);
        }
        private void Twitch_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            string buildmes = $"{e.ReSubscriber.DisplayName} переподписался {e.ReSubscriber.Months.ToString()} {(string.IsNullOrEmpty(e.ReSubscriber.ResubMessage) ? "" : (" и передает: " + e.ReSubscriber.ResubMessage))}";
            AddMessageInChat(FromService.sys, "sys", buildmes);
        }
        private void Twitch_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            string buildmes = $"{e.Subscriber.DisplayName} подписался {e.Subscriber.SubscriptionPlan.ToString()}";
            AddMessageInChat(FromService.sys, "sys", buildmes);
        }
        private void Twitch_OnConnected(object sender, OnConnectedArgs e)
        {
            AddMessageInChat(FromService.sys, "sys", "Подключился к Twitch.");
        }
        private void Twitch_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            AddMessageInChat(FromService.sys, "sys", "Отключился от Twitch.");
            StartTwClient();
        }
        private void Twitch_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("badword"))
                _twapiService.TwitchClientListen.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
            else
            {
                var ListTwitchMessageEnd = new List<ChatMessage.MessageWordsAndSmiles>();
                var SortListEmotesByStartIndex = e.ChatMessage.EmoteSet.Emotes.OrderBy(x => x.StartIndex).ToList();                
                
                if (SortListEmotesByStartIndex.Count > 0)
                {
                    int tempIndex = 0;
                    for (int i = 0; i <= SortListEmotesByStartIndex.Count - 1; i++)
                    {
                        var MessageWord = new ChatMessage.MessageWordsAndSmiles();
                        var MessageSmile = new ChatMessage.MessageWordsAndSmiles();
                        string partStringNoSmile = "";
                        partStringNoSmile = e.ChatMessage.Message.Substring(tempIndex, (SortListEmotesByStartIndex[i].StartIndex - tempIndex));
                        tempIndex = SortListEmotesByStartIndex[i].StartIndex + SortListEmotesByStartIndex[i].Name.Length;
                        MessageWord.MessageWord = partStringNoSmile;
                        MessageSmile.SmileUrl = SortListEmotesByStartIndex[i].ImageUrl;
                        ListTwitchMessageEnd.Add(MessageWord);
                        ListTwitchMessageEnd.Add(MessageSmile);
                    }
                    if (e.ChatMessage.Message.Length > tempIndex)
                    {
                        var MessageWordOther = new ChatMessage.MessageWordsAndSmiles();
                        MessageWordOther.MessageWord = e.ChatMessage.Message.Substring(tempIndex, (e.ChatMessage.Message.Length - tempIndex));                        
                        ListTwitchMessageEnd.Add(MessageWordOther);
                    }
                }
                else
                {
                    var MessageWordOnly = new ChatMessage.MessageWordsAndSmiles();
                    MessageWordOnly.MessageWord = e.ChatMessage.Message;
                    ListTwitchMessageEnd.Add(MessageWordOnly);
                }
                if (Chat != null)
                {
                    AddMessageInChat(FromService.Twitch, e.ChatMessage.DisplayName, ListTwitchMessageEnd);
                }
            }
        }
        private void FS_OnNewFollowerDetected(object sender, OnNewFollowersDetectedArgs e)
        {
            foreach (var item in e.NewFollowers.Where(x=>x.FollowedAt >= DateTime.Today))
            {
                AddMessageInChat(FromService.Twitch, item.FromUserName, " теперь отслеживает вас.");                
            }
        }
        public async Task AddMessageInChat(FromService serviceName, string fromUserName, string userMessage)
        {
            var mes = new AllChatMessage.ChatMessage
            {
                FromServiceName = serviceName,
                FromUserName = fromUserName,
                ListWordsAndSmiles = new List<ChatMessage.MessageWordsAndSmiles> {
                    new ChatMessage.MessageWordsAndSmiles { MessageWord = userMessage } 
                }
            };
            App.Current.Dispatcher.Invoke(() => Chat.Add(mes));
            OnPropertyChanged(nameof(Chat));
        }
        public async void AddMessageInChat(FromService serviceName, string fromUserName, List<ChatMessage.MessageWordsAndSmiles> messageWAS)
        {
            var mes = new AllChatMessage.ChatMessage
            {
                FromServiceName = serviceName,
                FromUserName = fromUserName,
                ListWordsAndSmiles = messageWAS
            };
            App.Current.Dispatcher.Invoke(() => Chat.Add(mes));
            OnPropertyChanged(nameof(Chat));
        }
        public async Task ClearChatMessages() 
        {
            App.Current.Dispatcher.Invoke(() => Chat = new ObservableCollection<AllChatMessage.ChatMessage>()); 
            OnPropertyChanged(nameof(Chat));
        }

    }
}
