using MultyPlatformChatReaderApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static MultyPlatformChatReaderApp.Models.TrovoChatMessagesModel;

namespace MultyPlatformChatReaderApp.Services
{
    public class TrovoApiService
    {
        private TrovoResourceModel TrResource = new TrovoResourceModel();
        public TrovoUser Truser = new TrovoUser();
        public TrovoUserLocalInfo TempInfo = new TrovoUserLocalInfo();
        private readonly StoreService _storeService;
        public TrovoApiService(StoreService storeService)
        {
            _storeService = storeService;
            if (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token)
                || !_storeService.SettingApp.SettingsTr.TrovoUserLogIn.CheckNeedUpdateTokenTrovo())
            {
                Truser = _storeService.SettingApp.SettingsTr.TrovoUserLogIn;
                StartTrovoApi();
            }
        }
        public async Task StartTrovoApi() { }
        public async Task GetTrovoToken() 
        {
            string allScope = String.Join("+", TrResource.allScopeTrovo);
            string scopetr = Uri.EscapeUriString(allScope);
            var http = new HttpListener();
            http.Prefixes.Add(TrResource.authorizationEndpointLocal + "/");
            http.Start();

            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}",
            TrResource.authorizationEndpointTrovo,
            scopetr,
            System.Uri.EscapeDataString(TrResource.authorizationEndpointLocal),
            TrResource.clientID_tr);
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
            string responseString = string.Format("<html><head><meta charset='utf-8'></head><body style='background-color:#158171; color:black'>Please return to the app. Пора вернуться в программу.</body></html>");
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
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("client-id", TrResource.clientID_tr);
            HttpContent content = new StringContent("{\"client_secret\": \"" + TrResource.clientSecret_tr + "\",\"grant_type\": \"authorization_code\",\"code\":\"" + code + 
                "\",\"redirect_uri\": \""+ TrResource.authorizationEndpointLocal + "\"}", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var responseExtoken = await _httpClient.PostAsync(TrResource.tokenRequestURITrovo, content);
            if (responseExtoken.IsSuccessStatusCode)
            {
                string resultExToken = await responseExtoken.Content.ReadAsStringAsync();
                Dictionary<string, object> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultExToken);
                var access_token = tokenEndpointDecoded["access_token"];
                Truser = JsonConvert.DeserializeObject<TrovoUser>(resultExToken);
                Truser.Issued = DateTime.Now;
                _storeService.SaveSettings(Truser);
            }
        }
        public async Task RefreshTrovoToken() 
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Add("client-id", TrResource.clientID_tr);

            HttpContent content = new StringContent("{\"client_secret\": \"" + TrResource.clientSecret_tr + "\", \"grant_type\": \"refresh_token\", \"refresh_token\": \"" + Truser.refresh_token + "\"}",
                Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await http.PostAsync("https://open-api.trovo.live/openplatform/refreshtoken", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                TrovoUser tempTruser = new TrovoUser();
                tempTruser = JsonConvert.DeserializeObject<TrovoUser>(result);
                Truser.access_token = tempTruser.access_token;
                Truser.refresh_token = tempTruser.refresh_token;
                Truser.Issued = DateTime.Now;
                _storeService.SaveSettings(Truser);
            }
        }
        public async Task CheckTrToken() 
        {
            Truser = _storeService.SettingApp.SettingsTr.TrovoUserLogIn;
            if (string.IsNullOrEmpty(Truser.access_token)) { await GetTrovoToken(); }
            if (Truser.CheckNeedUpdateTokenTrovo()) { await RefreshTrovoToken(); }
            await StartTrovoApi();
        }
        public async Task<TrovoUserInfo> GetTrovoUserInfo()
        {
            TrovoUserInfo NewUserInfo = new TrovoUserInfo();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);
            _httpClient.DefaultRequestHeaders.Add("Authorization", ("OAuth " + Truser.access_token));            
            var response = await _httpClient.GetAsync("https://open-api.trovo.live/openplatform/getuserinfo");
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                NewUserInfo = JsonConvert.DeserializeObject<TrovoUserInfo>(resultResponse);
            }
            return NewUserInfo;
        }
        public async Task<TrovoChannelInfo> GetTrovoChannelInfo(int? channel_id = null, string? username = null)
        {
            TrovoChannelInfo temp = new TrovoChannelInfo();
            if (channel_id > 0 | username != null)
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);

                HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
                if (channel_id > 0)
                {
                    string SearchOnId = "{\"channel_id\":"+channel_id+"}";
                    content = new StringContent(SearchOnId, Encoding.UTF8, "application/json");
                }
                if (username != null)
                {
                    string SearchOnUserName = "{\"username\":\""+username+"\"}";
                    content = new StringContent(SearchOnUserName, Encoding.UTF8, "application/json");
                }                
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PostAsync("https://open-api.trovo.live/openplatform/channels/id", content);
                if (response.IsSuccessStatusCode)
                {
                    string resultResponse = await response.Content.ReadAsStringAsync();
                    temp = JsonConvert.DeserializeObject<TrovoChannelInfo>(resultResponse);
                }
            }
            return temp;
        }
        public async Task<TrovoChatToken> GetChatToken()
        {
            TrovoChatToken NewChatToken = new TrovoChatToken();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);
            _httpClient.DefaultRequestHeaders.Add("Authorization", ("OAuth " + Truser.access_token));
            var response = await _httpClient.GetAsync("https://open-api.trovo.live/openplatform/chat/token");
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                NewChatToken = JsonConvert.DeserializeObject<TrovoChatToken>(resultResponse);
            }
            return NewChatToken;
        }
        public async Task<TrovoEmotes> GetEmotes(string? channelid = null)
        {
            TrovoEmotes _TrovoEmotes = new TrovoEmotes();
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", TrResource.clientID_tr);

            HttpContent content = new StringContent("{\"emote_type\":0,\"emote_type\":1,\"emote_type\":2}", Encoding.UTF8, "application/json");
            if (channelid != null)
            {
                content = new StringContent("{\"emote_type\":0,\"emote_type\":1,\"emote_type\":2, \"channel_id\":["+channelid+"]}", Encoding.UTF8, "application/json");
            }
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync("https://open-api.trovo.live/openplatform/getemotes", content);
            if (response.IsSuccessStatusCode)
            {
                string resultResponse = await response.Content.ReadAsStringAsync();
                _TrovoEmotes = JsonConvert.DeserializeObject<TrovoEmotes>(resultResponse);
            }
            return _TrovoEmotes;
        }
    }
}
