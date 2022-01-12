using System.Collections.Specialized;
using System.Windows.Controls;

namespace MultyPlatformChatReaderApp.Views
{
    /// <summary>
    /// Interaction logic for ChatsView.xaml
    /// </summary>
    public partial class ChatsView : UserControl
    {
        public ChatsView()
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
