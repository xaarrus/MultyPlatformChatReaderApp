using System.Collections.Specialized;
using System.Windows;

namespace MultyPlatformChatReaderApp.Views
{
    /// <summary>
    /// Interaction logic for ChatsWindow.xaml
    /// </summary>
    public partial class ChatsWindow : Window
    {
        public ChatsWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)ListMessages.Items).CollectionChanged += AutoScroll;
        }
        private void AutoScroll(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ListMessages.Items.Count > 0)
            {
                ListMessages.SelectedIndex = ListMessages.Items.Count - 1;
                ListMessages.ScrollIntoView(ListMessages.SelectedItem);
            }
        }
    }
}
