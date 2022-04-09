using System.Collections.Generic;

namespace MultyPlatformChatReaderApp.Models
{
    public class TrovoChatMessagesModel
    {
        public class TrovoMessage
        {
            public string type { get; set; }
            public ChannelInfo channel_info { get; set; }
            public Data data { get; set; }
        }
        public class ChannelInfo
        {
            public string channel_id { get; set; }
        }
        public class Data
        {
            public string eid { get; set; }
            public List<Chat> chats { get; set; }
        }
        public class Chat
        {
            public int type { get; set; }
            public string content { get; set; }
            public string nick_name { get; set; }
            public string avatar { get; set; }
            public string sub_lv { get; set; }
            public string sub_tier { get; set; }
            public List<string> medals { get; set; }
            public List<string> roles { get; set; }
            public string message_id { get; set; }
            public int sender_id { get; set; }
            public int send_time { get; set; }
            public int uid { get; set; }
            public string user_name { get; set; }
            public ContentData content_data { get; set; }
            public string custom_role { get; set; }
        }
        public class ContentData
        {
            //public string custom_emote_enabled { get; set; }
            public List<Emote> custom_emote_enabled { get; set; }
            //public string normal_emote_enabled { get; set; }
            public List<GlobalEmote> normal_emote_enabled { get; set; }
            public string gift_display_name { get; set; }
            public int gift_id { get; set; }
            public int gift_num { get; set; }
            public int specialEffectID { get; set; }
            public int magicChatGiftID { get; set; }
            public string rich { get; set; }
            public string chatShowAuth { get; set; }
            public string chatViewerThreshold { get; set; }
            public int viewers { get; set; }
            public string NickName { get; set; }
            public string UserID { get; set; }
            public int isReSub { get; set; }
        }

        public class TrovoEmotes
        {
            public Channels channels { get; set; }
        }
        public class Channels
        {
            public CustomizedEmotes customizedEmotes { get; set; }
            public List<EventEmote> eventEmotes { get; set; }
            public List<GlobalEmote> globalEmotes { get; set; }
        }
        public class CustomizedEmotes
        {
            public List<Channel> channel { get; set; }
        }
        public class Channel
        {
            public string channel_id { get; set; }
            public List<Emote> emotes { get; set; }
        }
        public class Emote
        {
            public string name { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string status { get; set; }
        }
        public class EventEmote
        {
            public string activity_name { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string gifp { get; set; }
            public string status { get; set; }
            public string update_time { get; set; }
            public string url { get; set; }
            public string webp { get; set; }
        }
        public class GlobalEmote
        {
            public string name { get; set; }
            public string description { get; set; }
            public string gifp { get; set; }
            public string status { get; set; }
            public string url { get; set; }
            public string webp { get; set; }
        }
    }
}
