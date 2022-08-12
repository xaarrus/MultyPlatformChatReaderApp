using System.Collections.Generic;

namespace MultyPlatformChatReaderApp.Models
{
    public class GGWSCommand
    {
        public class WSResponseType
        {
            public string type { get; set; } = "";
        }
        public class JoinChatCommand
        {
            public string type = "join";
            public Data data { get; set; } = new Data() { channel_id = 0 };
            public class Data
            {
                public int channel_id { get; set; }
                public bool hidden { get; set; } = false;
            }
        }
        public class LeaveChatCommand
        {
            public string type = "unjoin";
            
            public Data data { get; set; } = new Data() { channel_id = 0};
            public class Data
            {
                public int channel_id { get; set; }                
            }
        }
        public class ListChannelCommand
        {
            public string type = "get_channels_list";

            public Data data { get; set; } = new Data() { start = 0, count = 50 };
            public class Data
            {
                public int start { get; set; }
                public int count { get; set; }
            }
        }
        public class GetListChannelResponse
        {
            public string type = "channels_list";

            public Data data { get; set; } = new();
            public class Data
            {
                public List<Channel> channels { get; set; } = new();
            }
            public class Channel
            {
                public string channel_id { get; set; } = "";
                public string channel_name { get; set; } = "";
                public string clients_in_channel { get; set; } = "";
                public int users_in_channel { get; set; }
            }
        }

        public class GetMessageResponse
        {
            public string type = "message";
            public Data data { get; set; } = new Data();
            public class Data
            {
                public string channel_id { get; set; } = "";
                public int user_id { get; set; }
                public string user_name { get; set; } = "";
                public int user_rights { get; set; }
                public int premium { get; set; }
                public List<string> premiums { get; set; } = new() { "" };
                public int staff { get; set; }
                public string color { get; set; } = "";
                public string icon { get; set; } = "";
                public string role { get; set; } = "";
                public int mobile { get; set; }
                public int payments { get; set; }                
                public int gg_plus_tier { get; set; }
                public int isStatus { get; set; }
                public long message_id { get; set; }
                public int timestamp { get; set; }
                public string text { get; set; } = "";
            }
        }
        //premium = subscribe
        public class GetPremiumSubscribeResponse
        {
            public string type = "premium";

            public Data data { get; set; } = new Data();
            public class Data
            {
                public string channel_id { get; set; } = "";
                public string userId { get; set; } = "";
                public string userName { get; set; } = "";
                public string paymenth { get; set; } = "";
                public string resub { get; set; } = "";
            }
        }
        public class GetDonateResponse
        {
            public string type = "payment";

            public Data data { get; set; } = new Data();
            public class Data
            {
                public string channel_id { get; set; } = "";
                public string userName { get; set; } = "";
                public int amount { get; set; }
                public string message { get; set; } = "";
                public int? total { get; set; }
                public string? title { get; set; } = "";
            }
        }
    }
}
