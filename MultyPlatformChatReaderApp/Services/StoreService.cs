using MultyPlatformChatReaderApp.Models;
using Newtonsoft.Json;
using System.IO;
using static MultyPlatformChatReaderApp.Models.PortalGGModel;

namespace MultyPlatformChatReaderApp.Services
{
    public class StoreService
    {
        public SettingModel SettingApp = new SettingModel();
        private readonly string settingFileName = "settings.json";
        public StoreService()
        {
            LoadSettings();
        }
        public void SaveSettings(SettingModel settingToSave)
        {
            using (StreamWriter streamWriter = File.CreateText(settingFileName))
            {
                string settingInJson = JsonConvert.SerializeObject(settingToSave);
                streamWriter.Write(settingInJson);
            }
            LoadSettings();
        }
        public void SaveSettings(SettingModel.SettingGG settingToSave)
        {
            SettingApp.SettingsGG = settingToSave;
            using (StreamWriter streamWriter = File.CreateText(settingFileName))
            {
                string settingInJson = JsonConvert.SerializeObject(SettingApp);
                streamWriter.Write(settingInJson);
            }
            LoadSettings();
        }
        public void SaveSettings(UserLoginFormGGModel settingToSave)
        {
            SettingApp.SettingsGG.UserGGLogin = settingToSave;
            using (StreamWriter streamWriter = File.CreateText(settingFileName))
            {
                string settingInJson = JsonConvert.SerializeObject(SettingApp);
                streamWriter.Write(settingInJson);
            }
            LoadSettings();
        }
        public void SaveSettings(TwitchUser settingToSave)
        {
            SettingApp.SettingsTw.TwitchUserLogIn = settingToSave;
            using (StreamWriter streamWriter = File.CreateText(settingFileName))
            {
                string settingInJson = JsonConvert.SerializeObject(SettingApp);
                streamWriter.Write(settingInJson);
            }
            LoadSettings();
        }
        public void SaveSettings(TrovoUser settingToSave)
        {
            SettingApp.SettingsTr.TrovoUserLogIn = settingToSave;
            using (StreamWriter streamWriter = File.CreateText(settingFileName))
            {
                string settingInJson = JsonConvert.SerializeObject(SettingApp);
                streamWriter.Write(settingInJson);
            }
            LoadSettings();
        }
        public void LoadSettings()
        {
            if (!File.Exists(settingFileName))
            {
                SaveSettings(SettingApp);
            }
            using (StreamReader streamReader = File.OpenText(settingFileName))
            {
                string settingFromFile = streamReader.ReadToEnd();
                SettingApp = JsonConvert.DeserializeObject<SettingModel>(settingFromFile);
            }
            if (SettingApp == null)
            {
                SettingApp = new SettingModel();
                SaveSettings(SettingApp);                
            }
        }
    }
}
