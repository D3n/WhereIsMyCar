using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using System;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WhereIsMyCar.Models;

namespace WhereIsMyCar.ViewModel
{
    public class ParkingReminderViewModel : ViewModelBase
    {
        public String TitleApp
        {
            get { return "WhereIsMyCar"; }
        }

        public String TitlePage1
        {
            get { return "Parking"; }
        }

        public String TitlePage2
        {
            get { return "Programmation fin de parking";  }
        }

        public String TextButtonCreateEvent
        {
            get { return "Créer l'évènement"; }
        }

        private DateTime? _DateEvent;
        public DateTime? DateEvent
        {
            get {
                if (_DateEvent == null)
                {
                    _DateEvent = DateTime.Now;
                }
                return _DateEvent;
            }
            set { NotifyPropertyChanged(ref _DateEvent, value); }
        }

        private DateTime? _HourEvent;
        public DateTime? HourEvent
        {
            get {
                if (_HourEvent == null)
                {
                    _HourEvent = DateTime.Now;
                }
                return _HourEvent;
            }
            set { NotifyPropertyChanged(ref _HourEvent, value); }
        }

        public ICommand CreateEventParkingCommand { get; set; }

        // Constructor
        public ParkingReminderViewModel()
        {
            CreateEventParkingCommand = new RelayCommand(CreateEventParking);
        }

        private void CreateEventParking()
        {
            DateTime timeParkingEnd = (DateTime)HourEvent;
            DateTime dateParkingEnd = (DateTime)DateEvent;
            DateTime endParking = dateParkingEnd.Date + timeParkingEnd.TimeOfDay;

            // Make sure that the begin time has not already passed.
            if (endParking <= DateTime.Now)
            {
                MessageBox.Show("La date doit être dans le futur.");
            }
            else if (!IsolatedStorageSettings.ApplicationSettings.Contains("car"))
            {
                MessageBox.Show("Aucune position de voiture sauvegardée");
            }
            else
            {
                Car car = (Car)IsolatedStorageSettings.ApplicationSettings["car"];
                SaveAppointmentTask saveAppointmentTask = new SaveAppointmentTask();

                saveAppointmentTask.StartTime = DateTime.Now;
                saveAppointmentTask.EndTime = endParking;
                saveAppointmentTask.Subject = "Fin parking voiture";
                saveAppointmentTask.Location = car.Address;
                saveAppointmentTask.Details = "";
                saveAppointmentTask.IsAllDayEvent = false;
                saveAppointmentTask.Reminder = Reminder.FifteenMinutes;
                saveAppointmentTask.AppointmentStatus = Microsoft.Phone.UserData.AppointmentStatus.Busy;

                saveAppointmentTask.Show();
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
