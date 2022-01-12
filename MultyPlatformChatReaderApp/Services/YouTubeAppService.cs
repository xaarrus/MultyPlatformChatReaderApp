using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MultyPlatformChatReaderApp.Services
{
    public class YouTubeAppService
    {
        const string ClientIdYt = "ENTER_YOU_VALUE";
        const string ClientSecretYt = "ENTER_YOU_VALUE";
        private readonly ClientSecrets ClientSecretYT = new ClientSecrets() { ClientId = ClientIdYt, ClientSecret = ClientSecretYt};
        public UserCredential credential;
        public YouTubeService youtubeService;
        public IList<LiveChatMessage> YTListMessages = new List<LiveChatMessage>();
        public YouTubeAppService()
        {
            _ = CheckLoginFile();
        }
        private async Task CheckLoginFile()
        {
            if (File.Exists("./YouTube/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"))
            {
                await StartYTService();
            }
        }
        public async Task StartYTService()
        {
            if (credential == null)
            {
                await YTUserAuthInApp();
            }
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });
        }
        public async Task<IList<LiveChatMessage>> GetAllLiveChatMessages(string id)
        {
            var response = await LiveChatMessagesListGetAsync(id);
            if (response.Items != null)
            {
                YTListMessages = response.Items;
            }
            return YTListMessages;
        }
        public async Task<LiveChatMessageListResponse> LiveChatMessagesListGetAsync(string id)
        {
            var response = youtubeService.LiveChatMessages.List(id, "snippet, authorDetails");
            return await response.ExecuteAsync();
        }
        public async Task YTUserAuthInApp()
        {
            if (Directory.Exists("./YouTube/") == false) { Directory.CreateDirectory("./YouTube/"); }
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            ClientSecretYT,
            new[] { YouTubeService.Scope.YoutubeReadonly },
            "user",
            CancellationToken.None,
            new FileDataStore(System.IO.Path.GetFullPath("./YouTube/")));
        }
        public async Task YTUserLogOutApp()
        {
            if (File.Exists("./YouTube/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"))
            {
                File.Delete("./YouTube/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
            }
            credential = null;
            youtubeService.Dispose();
        }
        public async Task<ChannelListResponse> ChannelListAsync()
        {
            ChannelsResource.ListRequest response = youtubeService.Channels.List("snippet");
            response.Mine = true;
            return await response.ExecuteAsync();
        }
        public async Task<LiveBroadcastListResponse> LiveBroadcastListAsync()
        {
            LiveBroadcastsResource.ListRequest response = youtubeService.LiveBroadcasts.List("status, snippet, ContentDetails, statistics");
            response.Mine = true;
            return await response.ExecuteAsync();
        }
        public async Task<VideoListResponse> LiveStreamingListAsync(string id)
        {
            var response = youtubeService.Videos.List("liveStreamingDetails");
            response.Id = id;
            return await response.ExecuteAsync();
        }
        public async Task<LiveStreamListResponse> LiveStreamListAsync()
        {
            LiveStreamsResource.ListRequest response = youtubeService.LiveStreams.List("status, snippet, ContentDetails");
            response.Mine = true;
            return await response.ExecuteAsync();
        }
    }
}
