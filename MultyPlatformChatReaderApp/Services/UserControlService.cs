using System;
using System.Windows.Controls;

namespace MultyPlatformChatReaderApp.Services
{
    public class UserControlService
    {
        public event Action<UserControl> OnUserControlChanged;
        public void ChangeUserControl(UserControl usercontrol)=> OnUserControlChanged?.Invoke(usercontrol);
    }
}
