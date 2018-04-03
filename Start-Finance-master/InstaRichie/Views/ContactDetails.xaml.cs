using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage()
        {

            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();

        }

        public void Results()
        {
            conn.CreateTable<ContactDetails>();
            var query1 = conn.Table<ContactDetails>();
            ContactDetailsView.ItemsSource = query1.ToList();
        }

        private async void AddContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (firstName.Text.ToString() == "" || lastName.Text.ToString() == "" || companyName.Text.ToString() == "" || mobilePhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ContactDetails>();
                    conn.Insert(new ContactDetails
                    {
                        FirstName = firstName.Text.ToString(),
                        LastName = lastName.Text.ToString(),
                        CompanyName = companyName.Text.ToString(),
                        MobilePhone = mobilePhone.Text.ToString()
                    });
                    // Creating table
                    Results();

                    //reset fields
                    firstName.Text = "";
                    lastName.Text = "";
                    companyName.Text = "";
                    mobilePhone.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Wish Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void EditContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ContactDetails)ContactDetailsView.SelectedItem).ContactID.ToString();
                
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {

                    string f_Name = ((ContactDetails)ContactDetailsView.SelectedItem).FirstName;
                    string l_Name = ((ContactDetails)ContactDetailsView.SelectedItem).LastName;
                    string co_Name = ((ContactDetails)ContactDetailsView.SelectedItem).CompanyName;
                    string mob_Number = ((ContactDetails)ContactDetailsView.SelectedItem).MobilePhone;

                    firstName.Text = f_Name;
                    lastName.Text = l_Name;
                    companyName.Text = co_Name;
                    mobilePhone.Text = mob_Number;

                    updateButton.Visibility = Visibility;
                }

                var query3 = conn.Query<ContactDetails>("DELETE FROM ContactDetailsTbl WHERE ContactID ='" + AccSelection + "'");
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ContactDetails)ContactDetailsView.SelectedItem).ContactID.ToString();
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ContactDetails>();
                    var query1 = conn.Table<ContactDetails>();
                    var query3 = conn.Query<ContactDetails>("DELETE FROM ContactDetails WHERE ContactID ='" + AccSelection + "'");
                    ContactDetailsView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            string selectContactID = ((ContactDetails)ContactDetailsView.SelectedItem).ContactID.ToString();

            conn.CreateTable<ContactDetails>();

            string updateFirstName = firstName.Text.ToString();
            string updateLastName = lastName.Text.ToString();
            string updateCompanyName = companyName.Text.ToString();
            string updateMobilePhone = mobilePhone.Text.ToString();

            var query3 = conn.Query<ContactDetails>("UPDATE ContactDetails SET FirstName =  '" + updateFirstName + "', LastName = '" + updateLastName + "', CompanyName = '" + updateCompanyName + "', MobilePhone = '" + updateMobilePhone + "' WHERE ContactID ='" + selectContactID + "'");

            firstName.Text = "";
            lastName.Text = "";
            companyName.Text = "";
            mobilePhone.Text = "";

            updateButton.Visibility = Visibility.Collapsed;

            // Updating table
            Results();

        }
    }
}
