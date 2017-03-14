using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TimesheetMobile
{
    public partial class WorkAssignmentPage : ContentPage
    {
        public WorkAssignmentPage()
        {
            InitializeComponent();

            projectList.ItemsSource = new string[] { "AAA", "BBB" };
        }

        public async void LoadProjects(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://pointcol-timesheetmobile.azurewebsites.net");
                string json = await client.GetStringAsync("/api/workassignment");
                string[] employees = JsonConvert.DeserializeObject<string[]>(json);

                projectList.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                projectList.ItemsSource = new string[] { errorMessage };
            }
        }
    }
}
