using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Tasks;
using System;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WhereIsMyCar.Models;

namespace WhereIsMyCar.ViewModel
{
    public class ChosePageViewModel : ViewModelBase
    {
        public String titleApp
        {
            get { return "Where Is My Car ?!"; }
        }

        public String seekCar
        {
            get { return "Itinéraire vers ma voiture"; }
        }

        public String saveCar
        {
            get { return "Je sauvegarde ma position";  }
        }

        public String parkingReminderText
        {
            get { return "Rappel de fin de parking"; }
        }

        private String _TextCarStatus;
        public String TextCarStatus
        {
            get { return _TextCarStatus; }
            set { NotifyPropertyChanged(ref _TextCarStatus, value); }
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
        public ICommand RouteToCarCommand { get; set; }
        public ICommand GoToSaveCarCommand { get; set; }
        public ICommand GoToParkingReminderCommand { get; set; }

        public ChosePageViewModel()
        {

            UpdateStatusCar();
            Messenger.Default.Register<String>(this, ManageMessage);

            RouteToCarCommand = new RelayCommand(RouteToCar);
            GoToSaveCarCommand = new RelayCommand(GoToSaveCar);
            GoToParkingReminderCommand = new RelayCommand(GoToParkingReminder);
        }

        private void RouteToCar()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                Car car = (Car)IsolatedStorageSettings.ApplicationSettings["car"];
                MapsDirectionsTask mapsDirectionsTask = new MapsDirectionsTask();
                LabeledMapLocation carMapLocation = new LabeledMapLocation("Ma voiture", car.Geo);
                mapsDirectionsTask.End = carMapLocation;
                mapsDirectionsTask.Show();
            }
            else
            {
                this.OnUiThread(() =>
                {
                    MessageBox.Show("Vous devez d'abord sauvegarder la position de votre voiture.");
                });                
            }
        }

        private void GoToSaveCar()
        {
            Messenger.Default.Send<GoToPageMessage>(new GoToPageMessage() { PageName = "SaveCar" });
        }

        private void GoToParkingReminder()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                Messenger.Default.Send<GoToPageMessage>(new GoToPageMessage() { PageName = "ParkingReminder" });
            }
            else
            {
                this.OnUiThread(() =>
                {
                    MessageBox.Show("Vous devez d'abord sauvegarder la position de votre voiture.");
                }); 

            }
            
        }

        private void ManageMessage(String str)
        {
            if (str.Equals("CarPositionChanged"))
            {
                UpdateStatusCar();
            }


        }

        private void UpdateStatusCar()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                Car car = (Car)IsolatedStorageSettings.ApplicationSettings["car"];

                this.OnUiThread(() =>
                {
                    TextCarStatus = "Position de voiture sauvegardée : \n " + car.Address;
                });
                
            }
            else
            {
                this.OnUiThread(() =>
                {
                    TextCarStatus = "Aucune positition de voiture sauvegardée.";
                });
                
            }
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
