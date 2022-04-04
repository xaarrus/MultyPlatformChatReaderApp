using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultyPlatformChatReaderApp.Models
{
    public class AllChatMessage
    {
        public enum FromService
        {
            Twitch = 1,
            YouTube = 2,
            GoodGame = 3,
            sys = 99
        }
        public class ChatMessage
        {
            public FromService FromServiceName { get; set; }
            public string FromUserName { get; set; }
            public string UriImg { get { return VoidUriImg(FromServiceName); } set { } }
            public string? MessageContext { get; set; }
            public List<string>? SmileUrls { get; set; }
            public List<string>? MessageWords { get; set; }
            public List<MessageWordsAndSmiles>? ListWordsAndSmiles { get; set; }
            public string VoidUriImg(FromService serviceName)
            {
                switch (serviceName)
                {
                    case FromService.Twitch:
                        return "/Data/img/twi.png";
                    case FromService.YouTube:
                        return "/Data/img/yti.png";
                    case FromService.GoodGame:
                        return "/Data/img/ggi.png";
                    case FromService.sys:
                        return "/Data/img/sysi.png";
                    default:
                        return "/Data/img/sysi.png";
                }
            }
            public class MessageWordsAndSmiles
            {
                public string? MessageWord { get; set; }
                public string? SmileUrl { get; set; }
            }
        }
    }
}
