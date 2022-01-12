using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Services;
using MultyPlatformChatReaderApp.Views;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private UserControlService _userControlService;
        private UserControl _userControlSource;        
        public UserControl UserControlSource 
        {
            get { return _userControlSource; }
            set { _userControlSource = value; OnPropertyChanged(nameof(UserControlSource)); }
        }

        public ICommand UpdateViewCommand { get; set; }
        private readonly StoreService _storeService;
        public MainViewModel(UserControlService userControlService, StoreService storeService)
        {            
            _storeService = storeService;
            _userControlService = userControlService;            
            _userControlService.OnUserControlChanged += (usercontrol) => UserControlSource = usercontrol;
            _userControlService.ChangeUserControl(new IntroView());
            UpdateViewCommand = new UpdateViewCommand(this, _storeService);
        }
    }
}
