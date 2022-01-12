using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MultyPlatformChatReaderApp.Models
{
    public class TwitchResourceModel
    {
        public readonly string clientID_tw = "ENTER_YOU_VALUE";
        public readonly string clientSecret_tw = "ENTER_YOU_VALUE";
        public string authorizationEndpointTwitch = "https://id.twitch.tv/oauth2/authorize";
        public readonly string authorizationEndpointLocal = "http://localhost:8080/twitch/callback";
        public readonly string tokenRequestURITwitch = "https://id.twitch.tv/oauth2/token";
        public readonly string[] allScopeTwitch = new string[]
        {
                "analytics:read:extensions", "analytics:read:games", "bits:read", "channel:edit:commercial", "channel:manage:broadcast", "channel:manage:extensions",
                "channel:manage:redemptions", "channel:manage:videos", "channel:read:editors", "channel:read:hype_train", "channel:read:redemptions", "channel:read:stream_key",
                "channel:read:subscriptions", "clips:edit", "moderation:read", "user:edit", "user:edit:follows", "user:read:blocked_users",
                "user:manage:blocked_users", "user:read:broadcast", "user:read:email",
                "channel_subscriptions","channel_commercial","channel_editor","user_follows_edit","channel_read","user_read","user_blocks_read","user_blocks_edit",
                "channel:moderate","chat:edit","chat:read", "whispers:read","whispers:edit"    
        };
    }
    public class TwitchUser
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public List<string> scope { get; set; }
        public string token_type { get; set; }
        public DateTime Issued { get; set; }
    }
    public class TwitchUserLocalInfo
    {
        public Brush StreamStatus { get; set; } = new SolidColorBrush(Colors.Gray);
        public string AvatarLink { get; set; } = "pack://application:,,,/Data/img/app-anonavatar.png";
        public string NameOnTw { get; set; } = "User not LogIn";
        public string TwitchName { get; set; } = "";
        public string GameName { get; set; } = "Game not set or Twitch not response";
        public string StreamTitle { get; set; } = "";
        public int CountViewers { get; set; } = 0;
    }
}
