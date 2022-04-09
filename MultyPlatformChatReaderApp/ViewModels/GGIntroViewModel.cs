using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Services;
using MultyPlatformChatReaderApp.Views;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class GGIntroViewModel : BaseViewModel
    {
        
        public UserGGModel GGProfile = new UserGGModel();
        public GGStreamInfoModel GGStreamInfo = new GGStreamInfoModel();
        public string GGAvatarUrl 
        { 
            get { return (string.IsNullOrEmpty(GGProfile.user.avatar) ? "pack://application:,,,/Data/img/app-anonavatar.png" : "https://static.goodgame.ru" + GGProfile.user.avatar); } 
            set { GGProfile.user.avatar = value; OnPropertyChanged(nameof(GGAvatarUrl)); } 
        }
        public string GGName { get { return (string.IsNullOrEmpty(GGProfile.user.username) ? "User not LogIn" : GGProfile.user.username); } set { GGProfile.user.username = value; OnPropertyChanged(nameof(GGName)); } }

        public string GGGameName { get { return GGStreamInfo.stream.game.title; } set { GGStreamInfo.stream.game.title = value; OnPropertyChanged(nameof(GGGameName)); } }
        public string GGStreamTitle { get { return GGStreamInfo.stream.title; } set { GGStreamInfo.stream.title = value; OnPropertyChanged(nameof(GGStreamTitle)); } }
        public Brush _GGUserStatusBrush = new SolidColorBrush(Colors.Gray);
        public Brush GGUserStatusBrush { get { return _GGUserStatusBrush; } set { _GGUserStatusBrush = value; OnPropertyChanged(nameof(GGUserStatusBrush)); } }        
        public int GGCountViewer { get { return GGStreamInfo.stream.viewers; } set { GGStreamInfo.stream.viewers = value; OnPropertyChanged(nameof(GGCountViewer)); } }
        public Visibility _logOutButton = Visibility.Hidden;
        public Visibility logOutButton { get { return _logOutButton; } set { _logOutButton = value; OnPropertyChanged(nameof(logOutButton)); } }

        private readonly StoreService _storeService;
        private static GGLoginWindow _GGLoginWindow;
        public ICommand GGAvatar { get; set; }
        public ICommand GGLogOut { get; set; }
        public GGIntroViewModel(StoreService storeService)
        {
            _storeService = storeService;
            
            _ = CheckGGStatusLogin();
            GGLogOut = new AsyncCommand(async () => await GGLogOutApp());
            GGAvatar = new AsyncCommand(async () =>
            {
                if (string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.jwt))
                {
                    await GGLoginWindowOpen();
                }
                await GetGGStatusStream();
            });
        }
        private async Task GGLogOutApp()
        {
            UserGGModel clearUserGG = new UserGGModel();
            _storeService.SettingApp.SettingsGG.LogInUser = clearUserGG;
            var saveclear = _storeService.SettingApp.SettingsGG;
            _storeService.SaveSettings(saveclear);
            if (logOutButton == Visibility.Visible)
            {
                logOutButton = Visibility.Hidden;
            }
            GGProfile = new UserGGModel();
            GGStreamInfo = new GGStreamInfoModel();
            GGUserStatusBrush = new SolidColorBrush(Colors.Gray);
            OnPropertyChanged(nameof(GGAvatarUrl));
            OnPropertyChanged(nameof(GGName));
            OnPropertyChanged(nameof(GGGameName));
            OnPropertyChanged(nameof(GGStreamTitle));
            OnPropertyChanged(nameof(GGUserStatusBrush));
            OnPropertyChanged(nameof(GGCountViewer));
            await CheckGGStatusLogin();
        }
        private async Task GGLoginWindowOpen()
        {
            if (Application.Current.Windows.Cast<Window>().Any(x => x == _GGLoginWindow) == false)
            { 
                _GGLoginWindow = new GGLoginWindow();
                _GGLoginWindow.Show();
            }

            while (string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.jwt))
            {
                await Task.Delay(2000);
            }
            _GGLoginWindow.Close();
        }
        public async Task CheckGGStatusLogin()
        {
            while (string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.username))
            {
                await Task.Delay(8000);
            }
            await GetGGStatusStream();
        }
        public async Task GetGGStatusStream()
        {
            while (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.jwt))
            {
                HttpClient http = new HttpClient();
                string getUrl = $"https://goodgame.ru/api/4/user/" + _storeService.SettingApp.SettingsGG.LogInUser.user.id + "/view";
                var response = await http.GetAsync(getUrl);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var GetStreamInfo = JsonConvert.DeserializeObject<GGStreamInfoModel>(result);
                    GGGameName = GetStreamInfo.stream.game.title;
                    GGStreamTitle = GetStreamInfo.stream.title;
                    GGCountViewer = GetStreamInfo.stream.viewers;
                    if (GetStreamInfo.stream.status)
                    {
                        GGUserStatusBrush = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        GGUserStatusBrush = new SolidColorBrush(Colors.Red);
                    }
                }                
                GGAvatarUrl = _storeService.SettingApp.SettingsGG.LogInUser.user.avatar;
                GGName = _storeService.SettingApp.SettingsGG.LogInUser.user.username;
                if (logOutButton == Visibility.Hidden)
                {
                    logOutButton = Visibility.Visible;
                }
                await Task.Delay(8000);
            }
        }
    }
}
