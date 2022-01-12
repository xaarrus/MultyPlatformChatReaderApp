using MultyPlatformChatReaderApp.Services;
using MultyPlatformChatReaderApp.ViewModels;
using MultyPlatformChatReaderApp.Views;
using System;
using System.Windows.Input;

namespace MultyPlatformChatReaderApp.Commands
{
    public class UpdateViewCommand : ICommand
    {
        private MainViewModel _viewModel;
        private readonly StoreService _storeService;

        public UpdateViewCommand(MainViewModel viewModel, StoreService storeService)
        {
            _viewModel = viewModel;
            _storeService = storeService;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter?.ToString() == "chat")
            {
                _viewModel.UserControlSource = new ChatsView();
            }
            if (parameter?.ToString() == "intro")
            {
                _viewModel.UserControlSource = new IntroView();
            }
            if (parameter?.ToString() == "twintro")
            {
                _viewModel.UserControlSource = new TWIntroView();
            }
            if (parameter?.ToString() == "ggintro")
            {
                _viewModel.UserControlSource = new GGIntroView();
            }
            if (parameter?.ToString() == "ytintro")
            {
                _viewModel.UserControlSource = new YTIntroView();
            }
            if (parameter?.ToString() == "login")
            {
                _viewModel.UserControlSource = new LoginView();
            }
            if (parameter?.ToString() == "gglogin")
            {
                if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsGG.LogInUser.user.nickname))
                {
                    if (_storeService.SettingApp.SettingsGG.LogInUser.success)
                    {
                        _viewModel.UserControlSource = new IntroView();
                    }                    
                }
                else
                    _viewModel.UserControlSource = new GGLoginView();
            }
        }
    }
}
