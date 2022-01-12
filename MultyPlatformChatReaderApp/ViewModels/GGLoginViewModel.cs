using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;
using static MultyPlatformChatReaderApp.Models.PortalGGModel.UserGGModel;
using static MultyPlatformChatReaderApp.Models.SettingModel;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class GGLoginViewModel : BaseViewModel
    {
        private UserLoginFormGGModel _userGG = new UserLoginFormGGModel();
        public string Login { get { return _userGG.username; } set { _userGG.username = value; OnPropertyChanged(nameof(Login)); } }
        public string Password { get { return _userGG.password; } set { _userGG.password = value; OnPropertyChanged(nameof(Password)); } }
        private string _ResponseError = string.Empty;
        public string ResponseError
        {
            get { return _ResponseError; }
            set { _ResponseError = value; OnPropertyChanged(nameof(ResponseError)); }
        }

        public ICommand LoginGG { get; set; }
        private readonly StoreService _storeService;        
        public GGLoginViewModel(StoreService storeService)
        {
            _storeService = storeService;
            _userGG = new UserLoginFormGGModel() { username = _storeService.SettingApp.SettingsGG.UserGGLogin.username, password = _storeService.SettingApp.SettingsGG.UserGGLogin.password };
            if (!string.IsNullOrEmpty(Login) & !string.IsNullOrEmpty(Password))
            {
                GetLoginGG();
            }
            LoginGG = new AsyncCommand(async () => await GetLoginGG());
        }
        private async Task GetLoginGG()
        {
            ResponseError = "";            
            HttpClient http = new HttpClient();
            string json = JsonConvert.SerializeObject(_userGG);
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
                    ResponseError = "У вас нет канала!";                    
                }

                
                if (GetUserSucces.success & GetUserChannelSucces.user.channel.idi > 0)
                {
                    GetUserSucces.user.channelInfo.id = GetUserChannelSucces.user.channel.id;
                    SettingGG saveSettingUserGG = new SettingGG();
                    saveSettingUserGG.LogInUser = GetUserSucces;
                    saveSettingUserGG.UserGGLogin = _userGG;
                    _storeService.SaveSettings(saveSettingUserGG);                
                }
                else
                {
                    ResponseError = "" + GetUserSucces.error;
                }
                if (GetUserSucces.success & GetUserChannelSucces.user.channel.idi == 0)
                {
                    ResponseError = "У вас нет канала!";
                }
            }
            else
            {
                ResponseError = "Неудача: " + response.StatusCode + response.Content.ToString();
            }
        }
    }
}
