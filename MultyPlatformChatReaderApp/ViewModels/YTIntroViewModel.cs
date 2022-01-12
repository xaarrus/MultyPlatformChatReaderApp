using Google.Apis.YouTube.v3.Data;
using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class YTIntroViewModel : BaseViewModel
    {
        public string YtAvatarUrl { get; set; } = "pack://application:,,,/Data/img/app-anonavatar.png";
        public string YtName { get; set; } = "";
        public string YtGameName { get; set; } = "";
        public string YtStreamTitle { get; set; } = "";
        public string YtStreamId { get; set; } = "";
        public Brush YtUserStatusBrush { get; set; } = new SolidColorBrush(Colors.Gray);
        public int YtCountViewer { get; set; } = 0;
        public Visibility _logOutButton = Visibility.Hidden;
        public Visibility logOutButton { get { return _logOutButton; } set { _logOutButton = value; OnPropertyChanged(nameof(logOutButton)); } }
        private YouTubeAppService _ytService;
        public ICommand YtAvatar { get; set; }
        public ICommand YTLogOut { get; set; }
        public YTIntroViewModel(YouTubeAppService ytService)
        {
            _ytService = ytService;
            _ = Checkyt();

            YTLogOut = new AsyncCommand(async () => await YTLogOutApp());
            YtAvatar = new AsyncCommand(async () =>
            {
                if (_ytService.credential == null)
                {
                    await _ytService.StartYTService();
                }
                await Checkyt();
            });
        }
        private async Task YTLogOutApp()
        {
            await _ytService.YTUserLogOutApp();
            if (logOutButton == Visibility.Visible)
            {
                logOutButton = Visibility.Hidden;
            }
            YtAvatarUrl = "pack://application:,,,/Data/img/app-anonavatar.png"; OnPropertyChanged(nameof(YtAvatarUrl));
            YtName = ""; OnPropertyChanged(nameof(YtName));
            YtGameName = ""; OnPropertyChanged(nameof(YtGameName));
            YtStreamTitle = ""; OnPropertyChanged(nameof(YtStreamTitle));
            YtStreamId = ""; OnPropertyChanged(nameof(YtStreamId));
            YtCountViewer = 0; OnPropertyChanged(nameof(YtCountViewer));
            YtUserStatusBrush = new SolidColorBrush(Colors.Gray); OnPropertyChanged(nameof(YtUserStatusBrush));
            await Checkyt();
        }
        public async Task Checkyt()
        {
            while (_ytService.credential != null)
            {
                if (logOutButton == Visibility.Hidden)
                {
                    logOutButton = Visibility.Visible;
                }
                ChannelListResponse result = await _ytService.ChannelListAsync();
                foreach (Google.Apis.YouTube.v3.Data.Channel channel in result.Items)
                {
                    ImageBrush avatar = new ImageBrush();
                    YtAvatarUrl = channel.Snippet.Thumbnails.Medium.Url;
                    YtName = channel.Snippet.Title;
                    OnPropertyChanged(nameof(YtAvatarUrl));
                    OnPropertyChanged(nameof(YtName));
                }

                LiveBroadcastListResponse resultBroadcast = await _ytService.LiveBroadcastListAsync();
                if (resultBroadcast != null)
                {
                    foreach (var lbs in resultBroadcast.Items)
                    {
                        if (lbs != null && lbs.Status.LifeCycleStatus != "complete")
                        {
                            YtStreamTitle = lbs.Snippet.Title;
                            YtStreamId = lbs.Id;
                            OnPropertyChanged(nameof(YtStreamTitle));
                            OnPropertyChanged(nameof(YtStreamId));
                        }
                    }
                }

                VideoListResponse vlr = await _ytService.LiveStreamingListAsync(YtStreamId);
                if (vlr.Items != null)
                {
                    foreach (Video lscd in vlr.Items)
                    {
                        if (lscd.LiveStreamingDetails.ConcurrentViewers == null)
                        { YtCountViewer = 0; OnPropertyChanged(nameof(YtCountViewer)); }
                        else
                        { YtCountViewer = (int)lscd.LiveStreamingDetails.ConcurrentViewers; OnPropertyChanged(nameof(YtCountViewer)); }
                    }
                }
            
                LiveStreamListResponse resultLiveStream = await _ytService.LiveStreamListAsync();
                if (resultLiveStream != null)
                {
                    BrushConverter bc = new BrushConverter();
                    foreach (LiveStream ls in resultLiveStream.Items)
                    {
                        if (ls.Status.StreamStatus == "active")
                        {
                            YtUserStatusBrush = new SolidColorBrush(Colors.Green);
                            OnPropertyChanged(nameof(YtUserStatusBrush));
                        }
                        else
                        {
                            YtUserStatusBrush = new SolidColorBrush(Colors.Red);
                            OnPropertyChanged(nameof(YtUserStatusBrush));
                        }
                    }
                }
                await Task.Delay(8000);
            }            
        }
    }
}
