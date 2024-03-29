﻿using MultyPlatformChatReaderApp.Commands;
using MultyPlatformChatReaderApp.Models;
using MultyPlatformChatReaderApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class TRIntroViewModel : BaseViewModel
    {
        public TrovoApiService _trapiService;
        public StoreService _storeService;
        private TrovoUserLocalInfo TrLocalInfo = new TrovoUserLocalInfo();
        public TrovoUserInfo tempInfo = new();

        public string TrAvatarUrl { get { return TrLocalInfo.AvatarLink; } set { TrLocalInfo.AvatarLink = value; OnPropertyChanged(nameof(TrAvatarUrl)); } }
        public string TrName { get { return TrLocalInfo.NameOnTr; } set { TrLocalInfo.NameOnTr = value; OnPropertyChanged(nameof(TrName)); } }
        public string TrGameName { get { return TrLocalInfo.GameName; } set { TrLocalInfo.GameName = value; OnPropertyChanged(nameof(TrGameName)); } }
        public string TrStreamTitle { get { return TrLocalInfo.StreamTitle; } set { TrLocalInfo.StreamTitle = value; OnPropertyChanged(nameof(TrStreamTitle)); } }
        public Brush TrUserStatusBrush { get { return TrLocalInfo.StreamStatus; } set { TrLocalInfo.StreamStatus = value; OnPropertyChanged(nameof(TrUserStatusBrush)); } }
        public int TrCountViewer { get { return TrLocalInfo.CountViewers; } set { TrLocalInfo.CountViewers = value; OnPropertyChanged(nameof(TrCountViewer)); } }
        public Visibility _logOutButton = Visibility.Hidden;
        public Visibility logOutButton { get { return _logOutButton; } set { _logOutButton = value; OnPropertyChanged(nameof(logOutButton)); } }
        public ICommand TrAvatar { get; set; }
        public ICommand TRLogOut { get; set; }
        public TRIntroViewModel(TrovoApiService trapiService, StoreService storeService)
        {
            _trapiService = trapiService;
            _storeService = storeService;

            _ = CheckTrStatusLogin();
            _ = _trapiService.CheckTrToken();
            TRLogOut = new AsyncCommand(async () => await TRLogOutApp());
            TrAvatar = new AsyncCommand(async () => {
                if (string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token))
                {
                    await _trapiService.CheckTrToken();
                }
                if (_storeService.SettingApp.SettingsTr.TrovoUserLogIn.CheckNeedUpdateTokenTrovo())
                {
                    await _trapiService.CheckTrToken();
                }
                if (logOutButton == Visibility.Hidden)
                {
                    logOutButton = Visibility.Visible;
                }
                await GetTrStatusStream();
            });
        }
        public async Task CheckTrStatusLogin() 
        {
            while (string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token))
            {
                await Task.Delay(1000);
            }
            await GetTrStatusStream();
        }
        public async Task GetTrStatusStream() 
        {
            while (!string.IsNullOrEmpty(_storeService.SettingApp.SettingsTr.TrovoUserLogIn.access_token) && !_storeService.SettingApp.SettingsTr.TrovoUserLogIn.CheckNeedUpdateTokenTrovo())
            {
                if (logOutButton == Visibility.Hidden)
                {
                    logOutButton = Visibility.Visible;
                }
                tempInfo = await _trapiService.GetTrovoUserInfo();
                if (tempInfo != null)
                {
                    TrAvatarUrl = tempInfo.profilePic;
                    TrName = tempInfo.userName;
                        TrovoChannelInfo NewTrovoChannelInfo = await _trapiService.GetTrovoChannelInfo(channel_id: Int32.Parse(tempInfo.channelId));
                        TrGameName = NewTrovoChannelInfo.category_name;
                        TrStreamTitle = NewTrovoChannelInfo.live_title;
                        TrCountViewer = NewTrovoChannelInfo.current_viewers;
                        if (NewTrovoChannelInfo.is_live == true)
                        { TrUserStatusBrush = new SolidColorBrush(Colors.Green); }
                        if (NewTrovoChannelInfo.is_live == false)
                        { TrUserStatusBrush = new SolidColorBrush(Colors.Red); }
                }
                await Task.Delay(1000);
            }
            TrLocalInfo = new TrovoUserLocalInfo();
        }
        public async Task TRLogOutApp() 
        {
            TrovoUser clearTruser = new TrovoUser();
            tempInfo = new();
            _storeService.SaveSettings(clearTruser);
            _trapiService.Truser = clearTruser;

            if (logOutButton == Visibility.Visible)
            {
                logOutButton = Visibility.Hidden;
            }
            TrLocalInfo = new TrovoUserLocalInfo();
            OnPropertyChanged(nameof(TrAvatarUrl));
            OnPropertyChanged(nameof(TrName));
            OnPropertyChanged(nameof(TrGameName));
            OnPropertyChanged(nameof(TrStreamTitle));
            OnPropertyChanged(nameof(TrUserStatusBrush));
            OnPropertyChanged(nameof(TrCountViewer));
            await _trapiService.Connect();
            await CheckTrStatusLogin();
        }
    }
}
