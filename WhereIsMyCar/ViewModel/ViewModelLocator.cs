/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WhereIsMyCar"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using WhereIsMyCar.Models;

namespace WhereIsMyCar.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()   
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<SaveCarViewModel>();
            SimpleIoc.Default.Register<ChosePageViewModel>();
            SimpleIoc.Default.Register<ParkingReminderViewModel>();
        }

        public SaveCarViewModel SaveCarVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SaveCarViewModel>();
            }
        }

        public ChosePageViewModel ChosePageVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChosePageViewModel>();
            }
        }

        public ParkingReminderViewModel ParkingReminderVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ParkingReminderViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<SaveCarViewModel>();
            SimpleIoc.Default.Unregister<ChosePageViewModel>();
            SimpleIoc.Default.Unregister<ParkingReminderViewModel>();
        }
    }
}