using MultyPlatformChatReaderApp.Views;
using System.Windows.Controls;

namespace MultyPlatformChatReaderApp.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        public TRIntroView TRIntro { get; set; } = new TRIntroView();
        public TWIntroView TWIntro { get; set; } = new TWIntroView();
        public UserControl GGIntro { get; set; } = new GGIntroView();
        public UserControl YTIntro { get; set; } = new YTIntroView();
        public UserControl WASDIntro { get; set; } = new WASDIntroView();
        public ChatsView ChatAll { get;set; } = new ChatsView();        
        public IntroViewModel()
        {
            
        }
    }
}
