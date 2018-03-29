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
    public sealed partial class PersonalPage : Page
    {




        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalPage()
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
            conn.CreateTable<Personal>();
            var query1 = conn.Table<Personal>();
            PersonalView.ItemsSource = query1.ToList();
        }

        private async void AddPersonalDetails_Click(object sender, RoutedEventArgs e)
        {
            string selectGender = "";
            if (radioButtonMale.IsChecked == true)
            {
                selectGender = "Male";
            } else if(radioButtonFemale.IsChecked == true)
            {
                selectGender = "Female";
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Gender Not Selected", "Oops..!");
                await dialog.ShowAsync();
            }


            try
            {
                if (_FirstName.Text.ToString() == "" || _LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                   
                    conn.CreateTable<Personal>();
                    conn.Insert(new Personal
                    {
                        

                    FirstName = _FirstName.Text.ToString(),
                        LastName = _LastName.Text.ToString(),
                        DOB = _DOB.Text.ToString(),
                        Gender = selectGender,
                        Email = _Email.Text.ToString(),
                        Address = _Address.Text.ToString(),
                        MobilePhone = _MobilePhone.Text.ToString(),
                    });
                    // Creating table

                    _Email.Text = "";
                    _FirstName.Text = "";
                    _LastName.Text = "";
                    _Address.Text = "";
                    _DOB.Text = "";
                    _MobilePhone.Text = "";

                    Results();
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
                    MessageDialog dialog = new MessageDialog("Personal Details with that Email already exist, Try Different Email", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeletePersonalDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((Personal)PersonalView.SelectedItem).Email;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Personal>();
                    var query1 = conn.Table<Personal>();
                    var query3 = conn.Query<Personal>("DELETE FROM Personal WHERE Email ='" + AccSelection + "'");
                    PersonalView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }


        private async void UpdatePersonalDetails_Click(object sender, RoutedEventArgs e)
        {

            string populatedFirstName;
            populatedFirstName = _FirstName.Text.ToString();

            if (populatedFirstName == "")
            {

                string selEmail = ((Personal)PersonalView.SelectedItem).Email;
                string selFirstName = ((Personal)PersonalView.SelectedItem).FirstName;
                string selLastName = ((Personal)PersonalView.SelectedItem).LastName;
                string selAddress = ((Personal)PersonalView.SelectedItem).Address;
                string selDOB = ((Personal)PersonalView.SelectedItem).DOB;
                string selGender = ((Personal)PersonalView.SelectedItem).Gender;
                string selMobilePhone = ((Personal)PersonalView.SelectedItem).MobilePhone;

                _Email.Text = selEmail;
                _FirstName.Text = selFirstName;
                _LastName.Text = selLastName;
                _Address.Text = selAddress;
                _DOB.Text = selDOB;
                if (selGender == "Male")
                {
                    radioButtonMale.IsChecked = true;
                    radioButtonFemale.IsChecked = false;
                }
                else if (selGender == "Female")
                {
                    radioButtonFemale.IsChecked = true;
                    radioButtonMale.IsChecked = false;
                }
                _MobilePhone.Text = selMobilePhone;

                updateButton.Visibility = Visibility;

            }

            else
            {

               

            }

            /*
            try
            {
                string AccSelection = ((Personal)PersonalView.SelectedItem).Email;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Personal>();
                    var query1 = conn.Table<Personal>();
                    var query3 = conn.Query<Personal>("DELETE FROM Personal WHERE Email ='" + AccSelection + "'");
                    PersonalView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }

    */
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {


            string selectGender = "";
            if (radioButtonMale.IsChecked == true)
            {
                selectGender = "Male";
            }
            else if (radioButtonFemale.IsChecked == true)
            {
                selectGender = "Female";
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Gender Not Selected", "Oops..!");
                await dialog.ShowAsync();
            }

            string selEmail = _Email.Text.ToString();

            conn.CreateTable<Personal>();

            var query1 = conn.Table<Personal>();
            var query3 = conn.Query<Personal>("DELETE FROM Personal WHERE Email ='" + selEmail + "'");
            PersonalView.ItemsSource = query1.ToList();


            conn.Insert(new Personal
            {
                FirstName = _FirstName.Text.ToString(),
                LastName = _LastName.Text.ToString(),
                DOB = _DOB.Text.ToString(),
                Gender = selectGender,
                Email = _Email.Text.ToString(),
                Address = _Address.Text.ToString(),
                MobilePhone = _MobilePhone.Text.ToString()
            });

            _Email.Text = "";
            _FirstName.Text = "";
            _LastName.Text = "";
            _Address.Text = "";
            _DOB.Text = "";
            _MobilePhone.Text = "";

            // Creating table
            Results();


        }
    }
}
