using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        public void Results()
        {
            conn.CreateTable<Appointment>();
            var query1 = conn.Table<Appointment>();
            AppointmentListView.ItemsSource = query1.ToList();
        }

        private async void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string CDay = appointmentDate.Date.Day.ToString();
                string CMonth = appointmentDate.Date.Month.ToString();
                string CYear = appointmentDate.Date.Year.ToString();
                string FinalDate = " On " + CMonth + "/" + CDay + "/" + CYear;

                string Hours = startTimePicker.Time.Hours.ToString();
                string Minutes = startTimePicker.Time.Minutes.ToString();

                string finalStartTime = " Starts at " + Hours + ":" + Minutes;

                string Hours2 = endTimePicker.Time.Hours.ToString();
                string Minutes2 = endTimePicker.Time.Minutes.ToString();

                string finalEndTime = " Ends at " + Hours2 + ":" + Minutes2 ;


                if (txtEventName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointment>();
                    conn.Insert(new Appointment
                    {
                        EventName = txtEventName.Text.ToString(),
                        Location = txtLocation.Text.ToString(),
                        EventDate = FinalDate.ToString(),
                        StartTime = finalStartTime.ToString(),
                        EndTime = finalEndTime.ToString()
                    });
                    // Creating table
                    Results();

                    txtEventName.Text = "";
                    txtLocation.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the date or entered an invalid Date", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Event Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection = ((Appointment)AppointmentListView.SelectedItem).AppointmentID;
                if (AccSelection == -1)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointment>();
                    var query1 = conn.Table<Appointment>();
                    var query3 = conn.Query<Appointment>("DELETE FROM Appointment WHERE AppointmentID ='" + AccSelection + "'");
                    AppointmentListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not Selected the item", "Oops..!");
                await dialog.ShowAsync();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (AppointmentListView.SelectedIndex > -1)
            {
                btnUpdate.Visibility = Visibility.Visible;
                string eventName = ((Appointment)AppointmentListView.SelectedItem).EventName;
                string eventLocation = ((Appointment)AppointmentListView.SelectedItem).Location;
                string eventDate = ((Appointment)AppointmentListView.SelectedItem).EventDate;
                string startTime = ((Appointment)AppointmentListView.SelectedItem).StartTime;
                string endTime = ((Appointment)AppointmentListView.SelectedItem).EndTime;

                txtEventName.Text = eventName;
                txtLocation.Text = eventLocation;
                editPrompTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MessageDialog dialog = new MessageDialog("No Selected item to Edit", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int AccSelection = ((Appointment)AppointmentListView.SelectedItem).AppointmentID;
            string CDay = appointmentDate.Date.Day.ToString();
            string CMonth = appointmentDate.Date.Month.ToString();
            string CYear = appointmentDate.Date.Year.ToString();
            string FinalDate = " On " + CMonth + "/" + CDay + "/" + CYear;

            string Hours = startTimePicker.Time.Hours.ToString();
            string Minutes = startTimePicker.Time.Minutes.ToString();
            string finalStartTime = " Starts at " + Hours + ":" + Minutes;

            string Hours2 = endTimePicker.Time.Hours.ToString();
            string Minutes2 = endTimePicker.Time.Minutes.ToString();
            string finalEndTime = " Ends at " + Hours2 + ":" + Minutes2;

            string eventName = txtEventName.Text.ToString();
            string eventLocation = txtLocation.Text.ToString();
            string eventDate = FinalDate.ToString();
            string startTime = finalStartTime.ToString();
            string endTime = finalEndTime.ToString();

            var query = conn.Query<Appointment>("UPDATE Appointment SET EventName = '" + eventName + "', Location = '" + eventLocation + "', EventDate = '" + FinalDate + "', StartTime = '" + startTime + "', EndTime = '" + endTime + "' WHERE AppointmentID ='" + AccSelection + "'");

            txtEventName.Text = "";
            txtLocation.Text = "";

            btnUpdate.Visibility = Visibility.Collapsed;
            editPrompTextBlock.Visibility = Visibility.Collapsed;
            Results();
            
        }
    }
}
