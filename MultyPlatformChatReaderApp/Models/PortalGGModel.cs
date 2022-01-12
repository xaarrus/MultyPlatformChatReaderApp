using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyPlatformChatReaderApp.Models
{
    public class PortalGGModel
    {
        public class UserGGModel
        {
            public bool success { get; set; }
            public string? error { get; set; }
            public User user { get; set; } = new User();
            public class User
            {
                public int id { get; set; }
                public string username { get; set; }
                public string nickname { get; set; }
                public string avatar { get; set; }
                public string token { get; set; }                
                public Channel channelInfo { get; set; } = new Channel() { id ="0"};
                public Settings settings { get; set; }
                public int dialogs { get; set; }
                public bool bl { get; set; }
                public List<object> bl_data { get; set; }
                public string rights { get; set; }
                public bool premium { get; set; }                
                public bool is_banned { get; set; }
                public string jwt { get; set; }
            }
            public class UserChannel
            {
                public Channel channel { get; set; } = new Channel();
            }
            public class ChannelId
            {
                public UserChannel user { get; set; } = new UserChannel();
            }
            public class Channel
            {
                public string id { get; set; }
                public int idi { get { int temp = 0; temp = Convert.ToInt32(id); return temp; } }
            }
            public class Settings
            {
                public Chat chat { get; set; }
                public Beta beta { get; set; }
            }
            public class Chat
            {
                public int alignType { get; set; }
                public int pekaMod { get; set; }                
                public int soundVolume { get; set; }
                public int smilesType { get; set; }
                public int hide { get; set; }
                public int quickBan { get; set; }
                public int quickDelete { get; set; }
            }

            public class Beta
            {
                public int _4 { get; set; }
                public int _9 { get; set; }
                public int _10 { get; set; }
                public int _14 { get; set; }
                public int _16 { get; set; }
            }
        }
        public class UserLoginFormGGModel
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        public class UserLoginFalse
        {            
            public string error { get; set; }
        }
        public class GGStreamInfoModel
        {
            public GGStream stream { get; set; } = new GGStream();            
        }
        public class GGStream
        {
            public int id { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public string streamer { get; set; }
            public string avatar { get; set; }
            public string hidden { get; set; }
            public GGGame game { get; set; } = new GGGame();
            public int viewers { get; set; } = 0;
            public string preview { get; set; }
            public string poster { get; set; }
            public bool premium { get; set; }
            public string streamkey { get; set; }
            public string channelkey { get; set; }
            public bool status { get; set; }
            public object favorite { get; set; }            
        }
        public class GGGame
        {
            public string id { get; set; }
            public string poster { get; set; }
            public string poster3d { get; set; }
            public string title { get; set; } = "";
            public string url { get; set; }
        }
    }
}
