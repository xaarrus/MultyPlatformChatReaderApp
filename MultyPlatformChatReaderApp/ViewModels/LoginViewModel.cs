using MultyPlatformChatReaderApp.Views;
using System.Windows.Controls;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private UserControl _GGlogin = new GGLoginView();
        public UserControl GGlogin
        {
            get => _GGlogin;
            set { _GGlogin = value; OnPropertyChanged(nameof(GGlogin)); }
        }
        public LoginViewModel()
        {
           
        }
    }
}
