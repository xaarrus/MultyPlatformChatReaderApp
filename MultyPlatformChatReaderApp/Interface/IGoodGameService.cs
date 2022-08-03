using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using static MultyPlatformChatReaderApp.Models.AllChatMessage;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.Interface
{
    public interface IGoodGameService
    {

        UserLoginFormGGModel AuthUserForm { get; set; }
        UserGGModel GoodGameUser { get; set; }
        List<GGSmilesLibrary> AllSmilesGoodGame { get; set; }
        ClientWebSocket ClientGoodGame { get; set; }
        Task Connect();
        Task Send(string message);
        Task Receive(ClientWebSocket webSocket);
        Task AuthInGoodGame();
        delegate void GetNewMessage(FromService serviceName, string fromUserName, List<ChatMessage.MessageWordsAndSmiles> messageWAS);
        event GetNewMessage OnMessageReceive;
    }
}
