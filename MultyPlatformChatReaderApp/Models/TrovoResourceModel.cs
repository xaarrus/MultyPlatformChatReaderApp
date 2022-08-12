using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MultyPlatformChatReaderApp.Models
{
    public class TrovoResourceModel
    {
        public readonly string clientID_tr = "EnterYouValue";
        public readonly string clientSecret_tr = "EnterYouValue";
        public string authorizationEndpointTrovo = "https://open.trovo.live/page/login.html";
        public readonly string authorizationEndpointLocal = "http://127.0.0.1:8080/trovo/callback";
        public readonly string tokenRequestURITrovo = "https://open-api.trovo.live/openplatform/exchangetoken";
        public readonly string[] allScopeTrovo = new string[]
        {
            "user_details_self", "channel_details_self", "channel_update_self",
            "channel_subscriptions", "chat_send_self", "send_to_my_channel", "manage_messages",
            "chat_connect"
        };
    }
    public class TrovoUser
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public List<string> scope { get; set; }
        public string token_type { get; set; }
        public DateTime Issued { get; set; }
        public bool CheckNeedUpdateTokenTrovo()
        {
            DateTime thisTime = DateTime.Now;
            int day, hours, min, sec;
            day = (thisTime - Issued).Days;
            hours = (thisTime - Issued).Hours;
            min = (thisTime - Issued).Minutes;
            sec = (thisTime - Issued).Seconds;
            int resultSeconds = sec + (min * 60) + (hours * 60 * 60) + (day * 24 * 60 * 60);
            if (resultSeconds > 14400)
                return true;
            else
                return false;
        }
    }
    public class TrovoUserInfo
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string nickName { get; set; }
        public string email { get; set; }
        public string profilePic { get; set; }
        public string info { get; set; }
        public string channelId { get; set; }
    }
    public class TrovoChannelInfo
    {
        public bool is_live { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string live_title { get; set; }
        public string audi_type { get; set; }
        public string language_code { get; set; }
        public string thumbnail { get; set; }
        public int current_viewers { get; set; }
        public int followers { get; set; }
        public string streamer_info { get; set; }
        public string profile_pic { get; set; }
        public string channel_url { get; set; }
        public string created_at { get; set; }
        public int subscriber_num { get; set; }
        public string username { get; set; }
        public List<SocialLink> social_links { get; set; }
        public string started_at { get; set; }
        public string ended_at { get; set; }
    }
    public class SocialLink
    {
        public string type { get; set; }
        public string url { get; set; }
    }
    public class TrovoChatToken
    {
        public string token { get; set; }
    }
    public class TrovoUserLocalInfo
    {
        public Brush StreamStatus { get; set; } = new SolidColorBrush(Colors.Gray);
        public string AvatarLink { get; set; } = "pack://application:,,,/Data/img/app-anonavatar.png";
        public string NameOnTr { get; set; } = "User not LogIn";
        public string TrovoName { get; set; } = "";
        public string GameName { get; set; } = "Game not set or Trovo not response";
        public string StreamTitle { get; set; } = "";
        public int CountViewers { get; set; } = 0;
    }
    public class TrovoListSubscribers
    {
        public int total { get; set; }
        public List<Subscription> subscriptions { get; set; } = new();
        public class Subscription
        {
            public User user { get; set; } = new();
            public int sub_created_at { get; set; }
            public string sub_lv { get; set; } = "";
            public string sub_tier { get; set; } = "";
        }
        public class User
        {
            public string user_id { get; set; } = "";
            public string username { get; set; } = "";
            public string display_name { get; set; } = "";
            public string profile_pic { get; set; } = "";
            public string created_at { get; set; } = "";
        }
    }
}
