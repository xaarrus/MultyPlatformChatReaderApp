using MultyPlatformChatReaderApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Search;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Services;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace MultyPlatformChatReaderApp.Services
{
    public class TwitchApiService
    {
        private TwitchResourceModel TwResource = new TwitchResourceModel();
        public TwitchUser Twuser = new TwitchUser();
        public TwitchUserLocalInfo TempInfo = new TwitchUserLocalInfo();        
        public TwitchAPI TwitchAPISevice = new TwitchAPI();
        public TwitchClient TwitchClientListen = new TwitchClient();
        public FollowerService FollowerTWService;

        private readonly StoreService _storeService;
        public TwitchApiService(StoreService storeService)
        {
            _storeService = storeService;
            if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token))
            {
                Twuser = _storeService.SettingApp.SettingsTw.TwitchUserLogIn;
                StartTwApi();
            }
        }
        public async Task StartTwApi()
        {
            if(!string.IsNullOrEmpty(Twuser.access_token) & !Twuser.CheckNeedUpdateTokenTwitch())
            {
                TwitchAPISevice = new TwitchAPI() { Settings = {
                        AccessToken = Twuser.access_token, 
                        ClientId = TwResource.clientID_tw,
                        Secret = TwResource.clientSecret_tw,
                        Scopes = new List<AuthScopes>() { AuthScopes.Any },
                        SkipDynamicScopeValidation = true 
                    } };
                TempInfo = await TwStatusStream();
                ConnectionCredentials credentials = new ConnectionCredentials(TempInfo.TwitchName, _storeService.SettingApp.SettingsTw.TwitchUserLogIn.access_token);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                TwitchClientListen = new TwitchClient(customClient);
                TwitchClientListen.Initialize(credentials, TempInfo.TwitchName);
            }
            else
            {               
               await CheckTwToken();               
            }
        }
        public async Task CheckTwToken()
        {
            Twuser = _storeService.SettingApp.SettingsTw.TwitchUserLogIn;
            if (string.IsNullOrEmpty(Twuser.access_token)) { await GetTwToken(); }
            if (Twuser.CheckNeedUpdateTokenTwitch()) { await RefreshTwToken(); }
            await StartTwApi();
        }
        public async Task<TwitchUserLocalInfo> TwStatusStream()
        {
            TempInfo = new TwitchUserLocalInfo();
            if (Twuser.access_token != null)
            {                
                var responseTwUserAuthedList = await TwitchAPISevice.Helix.Users.GetUsersAsync(accessToken:Twuser.access_token);
                User responseTwUserAuthed = new User();
                if (responseTwUserAuthedList.Users.Count() > 0)
                {
                    responseTwUserAuthed = responseTwUserAuthedList.Users.FirstOrDefault();
                }
                
                if (responseTwUserAuthed != null)
                {
                    SearchChannelsResponse searchChannels = await TwitchAPISevice.Helix.Search.SearchChannelsAsync(responseTwUserAuthed.DisplayName);
                    if (searchChannels != null)
                    {
                        foreach (Channel myChannel in searchChannels.Channels)
                        {
                            if (myChannel.Id == responseTwUserAuthed.Id)
                            {
                                if (myChannel.IsLive == true)
                                {  TempInfo.StreamStatus = new SolidColorBrush(Colors.Green); }
                                else
                                {  TempInfo.StreamStatus = new SolidColorBrush(Colors.Red); }
                                TempInfo.AvatarLink = responseTwUserAuthed.ProfileImageUrl;
                                TempInfo.NameOnTw = responseTwUserAuthed.DisplayName;
                                TempInfo.TwitchName = responseTwUserAuthed.Login;
                            }
                        }
                    }
                }
                if (responseTwUserAuthed?.Id != null)
                {                    
                    var TwChannelInfo = await TwitchAPISevice.Helix.Channels.GetChannelInformationAsync(broadcasterId: responseTwUserAuthed.Id);
                    if (TwChannelInfo.Data.Count() > 0)
                    {
                        TempInfo.GameName = TwChannelInfo.Data.FirstOrDefault().GameName;                        
                        TempInfo.StreamTitle = TwChannelInfo.Data.FirstOrDefault().Title;
                    }
                    var TwOnlineCount = await TwitchAPISevice.Helix.Streams.GetStreamsAsync(userIds: new List<string> { responseTwUserAuthed.Id }, type: "all");
                    if (TwOnlineCount.Streams.Count() > 0)
                    {
                        TempInfo.CountViewers = TwOnlineCount.Streams.FirstOrDefault(x=>x.UserId == responseTwUserAuthed.Id).ViewerCount;
                    }
                }
            }
            return TempInfo;
        }
        public async Task RefreshTwToken()
        {
            HttpClient http = new HttpClient();
            HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
            string requestUri = $"https://id.twitch.tv/oauth2/token?grant_type=refresh_token&refresh_token={_storeService.SettingApp.SettingsTw.TwitchUserLogIn.refresh_token}&client_id={TwResource.clientID_tw}&client_secret={TwResource.clientSecret_tw}";
            var response = await http.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                TwitchUser tempTwuser = new TwitchUser();
                tempTwuser = JsonConvert.DeserializeObject<TwitchUser>(result);
                Twuser.access_token = tempTwuser.access_token;
                Twuser.refresh_token = tempTwuser.refresh_token;
                Twuser.Issued = DateTime.Now;
                _storeService.SaveSettings(Twuser);
            }
        }
        public async Task GetTwToken()
        {
            string allScope = String.Join(" ", TwResource.allScopeTwitch);
            string scopetw = Uri.EscapeUriString(allScope);
            var http = new HttpListener();
            http.Prefixes.Add(TwResource.authorizationEndpointLocal + "/");
            http.Start();

            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}",
            TwResource.authorizationEndpointTwitch,
            scopetw,
            System.Uri.EscapeDataString(TwResource.authorizationEndpointLocal),
            TwResource.clientID_tw);
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = authorizationRequest,
                    UseShellExecute = true
                }
            };
            p.Start();

            var context = await http.GetContextAsync();

            var response = context.Response;
            string responseString = string.Format("<html><head><meta charset='utf-8'></head><body style='background-color:#6441a5; color:black'>Please return to the app. Пора вернуться в программу.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server остановлен.");
            });

            if (context.Request.QueryString.Get("error") != null)
            {
                Console.WriteLine(String.Format("OAuth ошибка авторизации: {0}.", context.Request.QueryString.Get("error")));
            }
            var code = context.Request.QueryString.Get("code");
            string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&code={2}&grant_type=authorization_code&redirect_uri={3}",
                TwResource.clientID_tw, TwResource.clientSecret_tw, code, System.Uri.EscapeDataString(TwResource.authorizationEndpointLocal));
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(TwResource.tokenRequestURITwitch);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    string responseText = await reader.ReadToEndAsync();
                    Dictionary<string, object> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
                    var access_token = tokenEndpointDecoded["access_token"];
                    Twuser = JsonConvert.DeserializeObject<TwitchUser>(responseText);
                    Twuser.Issued = DateTime.Now;
                    _storeService.SaveSettings(Twuser);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response as HttpWebResponse != null)
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            string responseText = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
        }
    }
}
