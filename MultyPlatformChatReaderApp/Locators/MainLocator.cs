using Microsoft.Extensions.DependencyInjection;
using MultyPlatformChatReaderApp.Interface;
using MultyPlatformChatReaderApp.Services;
using MultyPlatformChatReaderApp.ViewModels;

namespace MultyPlatformChatReaderApp.Locators
{
    public class MainLocator
    {
        private static ServiceProvider _provider;

        public static void Init()
        {
            var services = new ServiceCollection();
                        
            services.AddScoped<IntroViewModel>();
            services.AddScoped<ChatsViewModel>();
            services.AddScoped<GGLoginViewModel>();              
            services.AddScoped<TWIntroViewModel>();            
            services.AddScoped<TRIntroViewModel>();            
            services.AddScoped<YTIntroViewModel>();            
            services.AddScoped<GGIntroViewModel>();
            services.AddScoped<WASDIntroViewModel>();
            services.AddScoped<MainViewModel>();

            services.AddSingleton<UserControlService>();            
            services.AddSingleton<StoreService>();
            services.AddSingleton<TwitchApiService>();
            services.AddSingleton<TrovoApiService>();
            services.AddSingleton<YouTubeAppService>();
            services.AddSingleton<WASDService>();
            services.AddSingleton<IGoodGameService, GoodGameService>();

            _provider = services.BuildServiceProvider();

            foreach (var item in services)
            {
                _provider.GetRequiredService(item.ServiceType);
            }
        }
        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();        
        public GGLoginViewModel GGLoginViewModel => _provider.GetRequiredService<GGLoginViewModel>();        
        public IntroViewModel IntroViewModel => _provider.GetRequiredService<IntroViewModel>();        
        public YTIntroViewModel YTIntroViewModel => _provider.GetRequiredService<YTIntroViewModel>();
        public TWIntroViewModel TWIntroViewModel => _provider.GetRequiredService<TWIntroViewModel>();
        public TRIntroViewModel TRIntroViewModel => _provider.GetRequiredService<TRIntroViewModel>();
        public GGIntroViewModel GGIntroViewModel => _provider.GetRequiredService<GGIntroViewModel>();
        public WASDIntroViewModel WASDIntroViewModel => _provider.GetRequiredService<WASDIntroViewModel>();
        public ChatsViewModel ChatsViewModel => _provider.GetRequiredService<ChatsViewModel>();
    }
}
