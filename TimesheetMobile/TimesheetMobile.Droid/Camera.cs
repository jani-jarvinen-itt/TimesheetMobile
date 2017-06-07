using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TimesheetMobile.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Camera))]

namespace TimesheetMobile.Droid
{
    public class Camera : TimesheetMobile.ICamera
    {
        private Action pictureTaken;

        public Action PictureTaken
        {
            get
            {
                return pictureTaken;
            }

            set
            {
                pictureTaken = value;
            }
        }

        public void TakePicture(string employeeName)
        {
            MainActivity.AndroidMainActivity.TakeAPicture(PictureTaken);
        }
    }
}