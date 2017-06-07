using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TimesheetMobile
{
    public partial class EmployeePage : ContentPage
    {
        public EmployeePage()
        {
            InitializeComponent();

            employeeList.ItemsSource = new string[] { "" };
            employeeList.ItemSelected += EmployeeList_ItemSelected;
        }

        private async void EmployeeList_ItemSelected(object sender,
            SelectedItemChangedEventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(employee))
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://pointcol-timesheetmobile.azurewebsites.net");
                    string json = await client.GetStringAsync("/api/employee?employeeName="+ employee);
                    byte[] imageBytes = JsonConvert.DeserializeObject<byte[]>(json);

                    employeeImage.Source = ImageSource.FromStream(
                        () => new MemoryStream(imageBytes));
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    employeeList.ItemsSource = new string[] { errorMessage };
                }
            }
        }

        public async void LoadEmployees(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://pointcol-timesheetmobile.azurewebsites.net");
                string json = await client.GetStringAsync("/api/employee");
                string[] employees = JsonConvert.DeserializeObject<string[]>(json);

                employeeList.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                employeeList.ItemsSource = new string[] { errorMessage };
            }
        }

        private async void ListWorkAssignments(object sender, EventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(employee))
            {
                await DisplayAlert("List Work", "You must select employee first.", "OK");
            }
            else
            {
                Navigation.PushAsync(new WorkAssignmentPage());
            }
        }

        private async void TakeEmployeePicture(object sender, EventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(employee))
            {
                ICamera camera = DependencyService.Get<ICamera>();

                camera.PictureTaken = PictureTakenSuccessfully;
                camera.TakePicture(employee);
            }
        }

        private void PictureTakenSuccessfully()
        {
            // TODO: päivitetään ruudulla näkyvä kuva
            takePictureButton.Text = "Picture Taken!";
        }
    }
}
