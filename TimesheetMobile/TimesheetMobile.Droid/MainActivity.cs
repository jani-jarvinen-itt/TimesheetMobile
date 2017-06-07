using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Runtime.Remoting.Contexts;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;

namespace TimesheetMobile.Droid
{
    public static class ImageInfo
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Android.Graphics.Bitmap bitmap;
    }

    [Activity(Label = "TimesheetMobile", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity :
        global::Xamarin.Forms.Platform.Android.FormsApplicationActivity,
        ILocationListener
    {
        public static Android.Locations.LocationManager LocationManager;
        public static MainActivity AndroidMainActivity;

        // kuvan ottamisen jälkeen suoritettava metodi eli Action
        private Action pictureTaken;

        #region Paikkatieto
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            AndroidMainActivity = this;

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
        #endregion


        #region Kuvan ottaminen

        private ImageView _imageView;

        //Tätä kutsutaan kun StartActivityForResult on suoritettu eli kuva otettu
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // varmistetaan, että kyse on kuvan ottamisesta (requestCode) ja että kuvanotto onnistui (resultCode)
            if ((requestCode == 0) && (resultCode == Android.App.Result.Ok))
            {
                // Make it available in the gallery
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(ImageInfo._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // kutsutaan tapahtumankäsittelijää, jos sellainen on määritelty
                pictureTaken?.Invoke();

                // Display in ImageView. We will resize the bitmap to fit the display
                // Loading the full sized image will consume to much memory 
                // and cause the application to crash.
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imageView.Height;
                ImageInfo.bitmap = ImageInfo._file.Path.LoadAndResizeBitmap(width, height);
                if (ImageInfo.bitmap != null)
                {
                    _imageView.SetImageBitmap(ImageInfo.bitmap);
                    ImageInfo.bitmap = null;
                }
                /*
                var url = "http://heinoar.azurewebsites.net/WebApi/UploadImage";

                WebClient myWebClient = new WebClient();
                try
                {
                    myWebClient.UploadFile(url, App._file.ToString());
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                }
                */

                // Dispose of the Java side bitmap.
                GC.Collect();
            }
        }

        //Luodaan hakemisto kuville. Tätä kutsutaan OnCreate kohdassa
        private void CreateDirectoryForPictures()
        {
            ImageInfo._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!ImageInfo._dir.Exists())
            {
                ImageInfo._dir.Mkdirs();
            }
        }

        //tarkistetaan onko laitteessa kamera
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        //Kutsutaan kun nappia painetaan
        public void TakeAPicture(Action pictureTaken)
        {
            //Intent tyyppiä käytetään käynnistämään androidissa muita sovelluksia
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //määritellään tiedosto, johon kuva tallennetaan
            ImageInfo._file = new Java.IO.File(ImageInfo._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            //kerrotaan intentille mihin tiedostoon kuva tallennetaan
            intent.PutExtra(MediaStore.ExtraOutput,
                Android.Net.Uri.FromFile(ImageInfo._file));

            // tallennetaan annettu tapahtuma/action
            this.pictureTaken = pictureTaken;

            //käynnistetään määritelty intent
            StartActivityForResult(intent, 0);
        }
        #endregion

    }
}

