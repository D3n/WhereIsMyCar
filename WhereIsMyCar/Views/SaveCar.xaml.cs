using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Device.Location;
using GalaSoft.MvvmLight.Messaging;


namespace WhereIsMyCar
{
    public partial class SaveCar : PhoneApplicationPage
    {
        // Constructor
        public SaveCar()
        {
            InitializeComponent();
            Messenger.Default.Register<GeoCoordinate>(this, MessageReceived);
        }

        private void MessageReceived(GeoCoordinate obj)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Carte.Center = obj ;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Messenger.Default.Send<String>("OnNavigatedTo");
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Send<String>("OnNavigatedFrom");
            base.OnNavigatedFrom(e);
        }

            
        private void Zoom_click(object sender, EventArgs e)
        {
            try
            {
                Carte.ZoomLevel++;
            }
            catch (Exception)
            {
                MessageBox.Show("Impossible de zoomer plus prêt.");
            }        
        }


        private void Dezoom_click(object sender, EventArgs e)
        {
            try {
                Carte.ZoomLevel--;
            }
            catch (Exception)
            {
                MessageBox.Show("Impossible de zoomer plus loin.");
            }
        }
    }
}