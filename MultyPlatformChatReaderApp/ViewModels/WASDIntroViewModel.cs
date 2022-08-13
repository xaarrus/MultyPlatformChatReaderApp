using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static MultyPlatformChatReaderApp.Models.WASD.WASDModels;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class WASDIntroViewModel : BaseViewModel
    {
        public WASDService _wasdService;
        public StoreService _storeService;
        public ResponseV2BroadcastsPublic WASDUserInfo = new ResponseV2BroadcastsPublic();
        public WASDUserLocalInfo WASDUserInfoLocal = new WASDUserLocalInfo();
        public Brush WASDUserStatusBrush1()
        {
            if(WASDUserInfo.result.channel.channel_status == "")
                return new SolidColorBrush(Colors.Green);
            else
                return new SolidColorBrush(Colors.Orange);
        }
        public int WASDViewverCounter()
        {
            if (WASDUserInfo.result.media_container.media_container_streams.Count > 0)
                return WASDUserInfo.result.media_container.media_container_streams[0].stream_total_viewers;
            else
                return 0;
        }
        public string WASDAvatarUrl { get { return WASDUserInfoLocal.AvatarLink; } set { WASDUserInfoLocal.AvatarLink = value; OnPropertyChanged(nameof(WASDAvatarUrl)); } }
        public string WASDName { get { return _wasdService.UserAlias; } set { _wasdService.UserAlias = value; OnPropertyChanged(nameof(WASDName)); } }
        public string WASDGameName { get { return WASDUserInfoLocal.GameName; } set { WASDUserInfoLocal.GameName = value; OnPropertyChanged(nameof(WASDGameName)); } }
        public string WASDStreamTitle { get { return WASDUserInfoLocal.StreamTitle; } set { WASDUserInfoLocal.StreamTitle = value; OnPropertyChanged(nameof(WASDStreamTitle)); } }
        public Brush WASDUserStatusBrush { get { return WASDUserInfoLocal.StreamStatus; } set { WASDUserInfoLocal.StreamStatus = value; OnPropertyChanged(nameof(WASDUserStatusBrush)); } }
        public int WASDCountViewer { get { return WASDUserInfoLocal.CountViewers; } set { WASDUserInfoLocal.CountViewers = value; OnPropertyChanged(nameof(WASDCountViewer)); } }
        public ICommand WASDAvatar { get; set; }
        public ICommand WASDReserve { get; set; }
        public Visibility _logOutButton = Visibility.Hidden;
        public Visibility logOutButton { get { return _logOutButton; } set { _logOutButton = value; OnPropertyChanged(nameof(logOutButton)); } }
        public WASDIntroViewModel(StoreService storeService, WASDService wasdService)
        {
            _storeService = storeService;
            _wasdService = wasdService;
            UpdateUserInfo();
            WASDAvatar = new AsyncCommand(async () => {
                await _wasdService.Start(); 
            });
        }
        public async Task UpdateUserInfo()
        {
            while (true)
            {
                WASDUserInfo = _wasdService.UserInfo;

                if (WASDUserInfo.result.channel.channel_image.small.Length > 5)
                {
                    WASDAvatarUrl = WASDUserInfo.result.channel.channel_image.small;
                }
                OnPropertyChanged(nameof(WASDAvatarUrl));

                if (WASDUserInfo.result.media_container?.game?.game_name.Length > 1)
                {
                    WASDGameName = WASDUserInfo.result.media_container.game.game_name;
                }
                OnPropertyChanged(nameof(WASDGameName));

                if (WASDUserInfo.result.media_container?.media_container_status == "RUNNING")
                {
                    WASDUserStatusBrush = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    WASDUserStatusBrush = new SolidColorBrush(Colors.Red);
                }
                OnPropertyChanged(nameof(WASDUserStatusBrush));

                if (WASDUserInfo.result.media_container?.media_container_name.Length > 1)
                {
                    WASDStreamTitle = WASDUserInfo.result.media_container.media_container_name;
                }
                OnPropertyChanged(nameof(WASDStreamTitle));

                if (WASDUserInfo.result.media_container?.media_container_streams.Count > 0)
                    WASDCountViewer = WASDUserInfo.result.media_container.media_container_streams[0].stream_total_viewers;
                else
                    WASDCountViewer = 0;
                OnPropertyChanged(nameof(WASDCountViewer));

                await Task.Delay(2000);
            }
        }
    }
}
