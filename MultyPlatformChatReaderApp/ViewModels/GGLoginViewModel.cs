using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Interface;
using MultyPlatformChatReaderApp.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class GGLoginViewModel : BaseViewModel
    {
        private UserLoginFormGGModel _userGG;
        public string Login { get { return _userGG.username; } set { _userGG.username = value; OnPropertyChanged(nameof(Login)); } }
        public string Password { get { return _userGG.password; } set { _userGG.password = value; OnPropertyChanged(nameof(Password)); } }

        public ICommand LoginGG { get; set; }
        private readonly GoodGameService _ggService;        
        public GGLoginViewModel(IGoodGameService ggService)
        {
            _ggService = (GoodGameService)ggService;
            _userGG = _ggService.AuthUserForm;
            if (!string.IsNullOrEmpty(Login) & !string.IsNullOrEmpty(Password))
            {
                GetLoginGG();
            }
            LoginGG = new AsyncCommand(async () => await GetLoginGG());
        }
        private async Task GetLoginGG()
        {
            await _ggService.AuthInGoodGame();
        }
    }
}
