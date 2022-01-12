# C# MVVM sample
The application work with api services and get message from chat and track information about you broadcast.

## Sample work with:
[![GoodGame](https://xaarrus.github.io/Blog/img/ico/igg.png)](https://goodgame.ru/) [![Twitch](https://xaarrus.github.io/Blog/img/ico/itw.png)](https://twitch.tv/) [![YouTube](https://xaarrus.github.io/Blog/img/ico/iyt.png)](https://youtube.com/)

## Features
* Check status stream
* Counter viewers
* See game name and broadcast title
* Receive message from services and see in only chat window
* This time only RUS localization


| Service | GoodGame | Twitch | YouTube |
|-|-|-|-|
| Get message | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| Follow | :black_square_button: | :white_check_mark: |:black_square_button: |
| Sub | :black_square_button: | :white_check_mark:|:black_square_button: |

## In progress
- [ ] Change Title broadcast
- [ ] Settings chat as font size, color text and etc
- [ ] Bad word list
- [ ] Quiz on base receive message from all service
- [ ] Localization

... and more

## Settings before build
1. Open Models/TwitchResourceModel enter you clientID, clientSecret and authorizationEndpointLocal
2. Open Services/YouTubeAppService enter you clientID, clientSecret

## Start use
For LogIn click on the question mark.

![](https://cdn.discordapp.com/attachments/354939174165544961/930841255439642714/unknown.png)

## NuGet
* [Microsoft.Extensions.DependencyInjection](https://dot.net)
* [Newtonsoft.Json](https://www.newtonsoft.com/json/)
* [TwitchLib.Api](https://github.com/TwitchLib/TwitchLib.Api)
* [TwitchLib.Client](https://github.com/TwitchLib/TwitchLib.Client)
* [Google.Apis.YouTube.v3](https://github.com/googleapis/google-api-dotnet-client)
