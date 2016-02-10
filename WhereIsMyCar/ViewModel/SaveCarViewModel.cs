using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using System;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WhereIsMyCar.Models;
using Windows.Devices.Geolocation;

namespace WhereIsMyCar.ViewModel
{
    public class SaveCarViewModel : ViewModelBase
    {
        private Geolocator geolocator;
        private bool statusGeoLoc = false;
        ShellTile liveTile;

        private Car car;
        public Car Car
        {
            get { return car; }
            set { NotifyPropertyChanged(ref car, value); }
        }

        private GeoCoordinate _UserCurrentPosition;
        public GeoCoordinate UserCurrentPosition
        {
            get { return _UserCurrentPosition; }
            set { NotifyPropertyChanged(ref _UserCurrentPosition, value); }
        }

        private GeoCoordinate _CarPosition;
        public GeoCoordinate CarPosition
        {
            get { return _CarPosition; }
            set { NotifyPropertyChanged(ref _CarPosition, value); }
        }

        private String _PushpinVisibility;
        public String PushpinVisibility
        {
            get { return _PushpinVisibility; }
            set { NotifyPropertyChanged(ref _PushpinVisibility, value); }
        }

        // UI Elements
        public String TitleApp
        {
            get { return "WhereIsMyCar"; }
        }

        public String ButtonSaveCarContent
        {
            get { return "Sauvegarder ma position"; }
        }

        // UI Thread safe
        protected delegate void OnUiThreadDelegate();

        protected void OnUiThread(OnUiThreadDelegate onUiThreadDelegate)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                onUiThreadDelegate();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(onUiThreadDelegate);
            }
        }
      
        // Commands
        public ICommand SaveTheCarCommand { get; set; }

        // Constructor
        public SaveCarViewModel()
        {
            Car = new Car();
            this.OnUiThread(() =>
            {
                CarPosition = new GeoCoordinate(0, 0);
                UserCurrentPosition = new GeoCoordinate(0, 0);
                PushpinVisibility = "Collapsed";
            });

            Messenger.Default.Register<String>(this, ManageMessage);

            SaveTheCarCommand = new RelayCommand(SaveTheCar);
        }

        private async void SaveTheCar()
        {
            Car.Geo = UserCurrentPosition;

            this.OnUiThread(() =>
            {
                CarPosition = UserCurrentPosition;
                PushpinVisibility = "Visible";
            });
            
            // Perform the reverse geocode query 
            var query = new ReverseGeocodeQuery() { GeoCoordinate = UserCurrentPosition };
            var geoCodeResults = await query.GetMapLocationsAsync();
            var address = geoCodeResults.First().Information.Address;
            Car.Address = FormatAddress(address);

            IsolatedStorageSettings.ApplicationSettings["car"] = Car;
            IsolatedStorageSettings.ApplicationSettings.Save();
            updateLiveTile();
            Messenger.Default.Send<String>("CarPositionChanged");
        }

        private void ManageMessage(String str)
        {
            if (str.Equals("BackupTheCar"))
            {
                BackupTheCarPosition();
            }
            if (str.Equals("OnNavigatedTo"))
            {
                geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High, MovementThreshold = 20 };
                geolocator.StatusChanged += geolocator_StatusChanged;
                geolocator.PositionChanged += geolocator_PositionChanged;

                BackupTheCarPosition();
            }
            if (str.Equals("OnNavigatedFrom"))
            {
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator.StatusChanged -= geolocator_StatusChanged;
                geolocator = null;
            }
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (statusGeoLoc)
            {
                this.OnUiThread(() =>
                {
                    UserCurrentPosition = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                    Messenger.Default.Send<GeoCoordinate>(UserCurrentPosition);
                });
            }
        }

        private void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";
            bool showMsg = false;

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    status = "Le service de localisation est désactivé dans les paramètres";
                    showMsg = true;
                    break;
                case PositionStatus.Initializing:
                    status = "En cours d'initialisation";
                    break;
                case PositionStatus.Ready:
                    status = "Service de localisation prêt";
                    statusGeoLoc = true;
                    break;
                case PositionStatus.NotAvailable:
                    status = "Service de localisation non disponible";
                    showMsg = true;
                    break;
                case PositionStatus.NotInitialized:
                    status = "Service de localisation non initialisé";
                    showMsg = true;
                    break;
                case PositionStatus.NoData:
                    break;
            }

            if (showMsg)
            {
                this.OnUiThread(() =>
                {
                    MessageBox.Show(status);
                });
            }
        }

        private void BackupTheCarPosition()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                Car backedUpCar = (Car)IsolatedStorageSettings.ApplicationSettings["car"];
                Car = backedUpCar;

                this.OnUiThread(() =>
                {
                    CarPosition = backedUpCar.Geo;
                    PushpinVisibility = "Visible";
                });

                updateLiveTile();
            }
        }

        private void updateLiveTile()
        {
            liveTile = ShellTile.ActiveTiles.First();
            int _Count = 0;
            String _BackContent = "";

            if (IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                _Count = 1;
                _BackContent = Car.Address;
            }
            else
            {
                _Count = 0;
                _BackContent = "Aucune voiture sauvegardée";
            }

            if (liveTile != null)
            {
                FlipTileData flipTileData = new FlipTileData
                {
                    Title = "WhereIsMyCar ?!",
                    Count = _Count,
                    BackTitle = "WhereIsMyCar ?!",
                    BackContent = _BackContent,
                };

                liveTile.Update(flipTileData);
            }
        }

        private string FormatAddress(MapAddress address)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(address.HouseNumber).Append(", ")
                .Append(address.Street).Append(". ")
                .Append(address.City).Append(". ")
                .Append(address.Country).Append(". ")
                .Append(address.Continent);

            return sb.ToString();
        }

        private bool NotifyPropertyChanged<T>(ref T variable, T valeur, [CallerMemberName] string nomPropriete = null)
        {
            if (object.Equals(variable, valeur)) return false;

            variable = valeur;
            RaisePropertyChanged(nomPropriete);
            return true;
        }
    }
}
