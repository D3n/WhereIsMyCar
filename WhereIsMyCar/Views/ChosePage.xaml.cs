using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;
using WhereIsMyCar.Models;
using System.Text;

namespace WhereIsMyCar
{
    public partial class ChosePage : PhoneApplicationPage
    {
        public ChosePage()
        {
            InitializeComponent();
            Messenger.Default.Register<GoToPageMessage>
            (
                 this,
                 (action) => ReceiveMessage(action)
            );
        }

        private void ReceiveMessage(GoToPageMessage action)
        {
            StringBuilder sb = new StringBuilder("/Views/");
            sb.Append(action.PageName);
            sb.Append(".xaml");
            NavigationService.Navigate(
                new System.Uri(sb.ToString(),
                    System.UriKind.Relative));
        }
    }
}