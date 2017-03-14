﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TimesheetMobile.Models;
using Xamarin.Forms;

namespace TimesheetMobile
{
    public partial class WorkAssignmentPage : ContentPage
    {
        public WorkAssignmentPage()
        {
            InitializeComponent();

            assignmentList.ItemsSource = new string[] { "AAA", "BBB" };
        }

        public async void LoadWorkAssignments(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://pointcol-timesheetmobile.azurewebsites.net");
                string json = await client.GetStringAsync("/api/workassignment");
                string[] assignments = JsonConvert.DeserializeObject<string[]>(json);

                assignmentList.ItemsSource = assignments;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                assignmentList.ItemsSource = new string[] { errorMessage };
            }
        }

        public async void StartWork(object sender, EventArgs e)
        {
            string assignmentName = assignmentList.SelectedItem.ToString();
            if (assignmentName != "")
            {
                try
                {
                    WorkAssignmentOperationModel data = new WorkAssignmentOperationModel()
                    {
                        Operation = "Start",
                        AssignmentTitle = assignmentName
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://pointcol-timesheetmobile.azurewebsites.net");
                    string input = JsonConvert.SerializeObject(data);
                    StringContent content = new StringContent(input,Encoding.UTF8,"application/json");

                    HttpResponseMessage message = await client.PostAsync("/api/workassignment",content);
                    string reply = await message.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(reply);

                    if (success)
                    {
                        await DisplayAlert("Start Work", "Work has been started.", "Close");
                    }
                    else
                    {
                        await DisplayAlert("Start Work", "Could not start work.", "Close");
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    assignmentList.ItemsSource = new string[] { errorMessage };
                }
            }
        }

        public async void StopWork(object sender, EventArgs e)
        {
        }
    }
}
