using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.Models
{
    public class SettingModel
    {
        public SettingTw SettingsTw { get; set; } = new SettingTw();
        public SettingGG SettingsGG { get; set;} = new SettingGG();
        public class SettingGG
        {
            public UserLoginFormGGModel UserGGLogin { get; set; } = new UserLoginFormGGModel();
            public UserGGModel LogInUser { get; set; } = new UserGGModel();
        }
        public class SettingTw
        {
            public TwitchUser TwitchUserLogIn { get; set; } = new TwitchUser();            
        }
    }
}
