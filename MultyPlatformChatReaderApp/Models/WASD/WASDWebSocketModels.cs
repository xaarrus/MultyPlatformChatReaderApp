using System;
using System.Collections.Generic;

namespace MultyPlatformChatReaderApp.Models.WASD
{
    public class WASDWebSocketModels
    {
        public class ResponseJoinedWASD
        {
            public List<string> actions { get; set; } = new();
            public string user_channel_role { get; set; } = string.Empty;
            public List<string> other_roles { get; set; } = new();
            public List<string> user_sticker_packs { get; set; } = new();
            public ChatSettings chat_settings { get; set; } = new();
            public class ChatSettings
            {
                public string goodbyeBetaGifts { get; set; } = string.Empty;
            }
        }
        public class ResponseMessageWASD
        {
            public string id { get; set; } = string.Empty;
            public int user_id { get; set; } 
            public string message { get; set; } = "";
            public string user_login { get; set; } = "";
            public UserAvatar user_avatar { get; set; } = new UserAvatar();
            public string hash { get; set; } = string.Empty;
            public bool is_follower { get; set; }
            public List<string> other_roles { get; set; } = new List<string>();
            public string user_channel_role { get; set; } = string.Empty;
            public int channel_id { get; set; }
            public int stream_id { get; set; }
            public DateTime date_time { get; set; }
            public class UserAvatar
            {
                public string large { get; set; } = string.Empty;
                public string small { get; set; } = string.Empty;
                public string medium { get; set; } = string.Empty;
            }
        }
        public class ResponseSystemMessageWASD
        {
            public string message { get; set; } = string.Empty;
        }
    }
}
