using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Runtime.Remoting.Contexts;

namespace TimesheetMobile.Droid
{
    [Activity(Label = "TimesheetMobile", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity :
        global::Xamarin.Forms.Platform.Android.FormsApplicationActivity,
        ILocationListener
    {
        public static Android.Locations.LocationManager LocationManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            // käynnistetään GPS-paikannus
            try
            {
                LocationManager = GetSystemService(
                    "location") as LocationManager;
                string Provider = LocationManager.GpsProvider;

                if (LocationManager.IsProviderEnabled(Provider))
                {
                    LocationManager.RequestLocationUpdates(
                        Provider, 2000, 1, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void OnLocationChanged(Location location)
        {
            TimesheetMobile.Models.GpsLocationModel.Latitude =
                location.Latitude;
            TimesheetMobile.Models.GpsLocationModel.Longitude =
                location.Longitude;
            TimesheetMobile.Models.GpsLocationModel.Altitude =
                location.Altitude;
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }
}

