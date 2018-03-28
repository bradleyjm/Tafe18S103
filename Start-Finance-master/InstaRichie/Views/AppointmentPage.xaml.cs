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
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqllite");

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
                string CDay = appointmentDate.Date.Value.Day.ToString();
                string CMonth = appointmentDate.Date.Value.Month.ToString();
                string CYear = appointmentDate.Date.Value.Year.ToString();
                string FinalDate = " On " + CMonth + "/" + CDay + "/" + CYear;

                string Hours = startTimePicker.Time.Hours.ToString();
                string Minutes = startTimePicker.Time.Minutes.ToString();
                string iden;
                if (startTimePicker.Time.Hours > 11)
                {
                    iden = "pm";
                }
                else { iden = "am"; }

                string finalStartTime = " Starts at " + Hours + ":" + Minutes + iden;

                string Hours2 = endTimePicker.Time.Hours.ToString();
                string Minutes2 = endTimePicker.Time.Minutes.ToString();
                string iden2;
                if (startTimePicker.Time.Hours > 11)
                {
                    iden2 = "pm";
                }
                else { iden2 = "am"; }

                string finalEndTime = " Ends at " + Hours2 + ":" + Minutes2 + iden2;


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
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the date or entered an Date", "Oops..!");
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
    }
}
