using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Models;
using MultyPlatformChatReaderApp.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TwitchLib.Api;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class TWIntroViewModel : BaseViewModel
    {
        public TwitchApiService _twapiService;        
        public StoreService _storeService;        
        private TwitchUserLocalInfo TwLocalInfo = new TwitchUserLocalInfo();        
        public string TwAvatarUrl { get { return TwLocalInfo.AvatarLink; } set { TwLocalInfo.AvatarLink = value; OnPropertyChanged(nameof(TwAvatarUrl)); } }        
        public string TwName { get { return TwLocalInfo.NameOnTw; } set { TwLocalInfo.NameOnTw = value; OnPropertyChanged(nameof(TwName)); } }        
        public string TwGameName { get { return TwLocalInfo.GameName; } set { TwLocalInfo.GameName = value; OnPropertyChanged(nameof(TwGameName)); } }
        public string TwStreamTitle { get { return TwLocalInfo.StreamTitle; } set { TwLocalInfo.StreamTitle = value; OnPropertyChanged(nameof(TwStreamTitle)); } }
        public Brush TwUserStatusBrush { get { return TwLocalInfo.StreamStatus; } set { TwLocalInfo.StreamStatus = value; OnPropertyChanged(nameof(TwUserStatusBrush)); } }
        public int TwCountViewer { get { return TwLocalInfo.CountViewers; } set { TwLocalInfo.CountViewers = value; OnPropertyChanged(nameof(TwCountViewer)); } }
        public Visibility _logOutButton = Visibility.Hidden;
        public Visibility logOutButton { get { return _logOutButton; } set { _logOutButton = value; OnPropertyChanged(nameof(logOutButton)); } }
        public ICommand TwAvatar{ get; set; }
        public ICommand TWLogOut{ get; set; }
        public TWIntroViewModel(TwitchApiService twapiService, StoreService storeService)
        {            
            _twapiService = twapiService;
            _storeService = storeService;

            _ = CheckTwStatusLogin();

            TWLogOut = new AsyncCommand(async () => await TWLogOutApp());
            TwAvatar = new AsyncCommand(async()=> 
            {
                if (string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token)
                | _storeService.SettingApp.SettingsTw.TwitchUserLogIn.CheckNeedUpdateTokenTwitch())
                {
                    await _twapiService.CheckTwToken();                    
                }
                await GetTwStatusStream();
            });
        }
        public async Task TWLogOutApp()
        {
            TwitchUser clearTwuser = new TwitchUser();            
            _storeService.SaveSettings(clearTwuser);
            if (_twapiService.TwitchClientListen.IsConnected)
            {
                _twapiService.TwitchClientListen.Disconnect();
            }            
            if (logOutButton == Visibility.Visible)
            {
                logOutButton = Visibility.Hidden;
            }
            TwLocalInfo = new TwitchUserLocalInfo();
            OnPropertyChanged(nameof(TwAvatarUrl));
            OnPropertyChanged(nameof(TwName));
            OnPropertyChanged(nameof(TwGameName));
            OnPropertyChanged(nameof(TwStreamTitle));
            OnPropertyChanged(nameof(TwUserStatusBrush));
            OnPropertyChanged(nameof(TwCountViewer));            
        }
        public async Task CheckTwStatusLogin()
        {            
            while(string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token)
                | _storeService.SettingApp.SettingsTw.TwitchUserLogIn.CheckNeedUpdateTokenTwitch())
            {
                await Task.Delay(8000);
            }
            if (logOutButton == Visibility.Hidden)
            {
                logOutButton = Visibility.Visible;
            }
            await GetTwStatusStream();
        }
        public async Task GetTwStatusStream()
        {            
            while (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token)
                & !_storeService.SettingApp.SettingsTw.TwitchUserLogIn.CheckNeedUpdateTokenTwitch())
            {
                TwitchUserLocalInfo temp = await _twapiService.TwStatusStream();
                TwAvatarUrl = temp.AvatarLink;
                TwName = temp.NameOnTw;                
                TwUserStatusBrush = temp.StreamStatus;
                TwCountViewer = temp.CountViewers;
                TwGameName = temp.GameName;
                TwStreamTitle = temp.StreamTitle;                
                await Task.Delay(8000);
            }
            await CheckTwStatusLogin();
        }
    }
}
