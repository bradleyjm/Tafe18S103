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
    public sealed partial class ShoppingListPage : Page
    {
      
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
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
            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList();
        }

        private async void AddShopItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_Shopname.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    double TempMoney = Convert.ToDouble(_PriceQuoted.Text);
                    conn.CreateTable<ShoppingList>();
                    conn.Insert(new ShoppingList
                    {
                        ShopName = _Shopname.Text.ToString(),
                        NameOfItem = _Shopitem.Text.ToString(),
                        ShoppingDate = _Shopdate.Text.ToString(),
                        PriceQuoted = TempMoney
                    });
                    // Creating table
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
                    MessageDialog dialog = new MessageDialog("Shopping List Item already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeleteShopItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    var query3 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE NameOfItem ='" + AccSelection + "'");
                    ShoppingListView.ItemsSource = query1.ToList();
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


        private async void UpdateShoppingList_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                string selNameOfItem = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                if (selNameOfItem == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    string selShopName = ((ShoppingList)ShoppingListView.SelectedItem).ShopName;
                    string selShoppingDate = ((ShoppingList)ShoppingListView.SelectedItem).ShoppingDate;
                    double selPriceQuoted = ((ShoppingList)ShoppingListView.SelectedItem).PriceQuoted;

                    _Shopname.Text = selShopName;
                    _Shopitem.Text = selNameOfItem;
                    _Shopdate.Text = selShoppingDate;
                    _PriceQuoted.Text = selPriceQuoted.ToString();

                    updateButton.Visibility = Visibility;
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }

        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {

            // get selected shop name and product to update
            string AccSelectionShop = ((ShoppingList)ShoppingListView.SelectedItem).ShopName;
            string AccSelectionItem = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;

            string updateShopName = _Shopname.Text.ToString();
            string updateShopItem = _Shopitem.Text.ToString();
            string updateShopDate = _Shopdate.Text.ToString();
            double updatePriceQuoted = Convert.ToDouble(_PriceQuoted.Text);

            var query3 = conn.Query<ShoppingList>("UPDATE ShoppingList SET ShopName =  '" + updateShopName + "', NameOfItem = '" + updateShopItem + "', ShoppingDate = '" + updateShopDate + "', PriceQuoted = '" + updatePriceQuoted + "' WHERE ShopName ='" + AccSelectionShop + "' AND NameOfItem ='" + AccSelectionItem + "'");

            _Shopname.Text = "";
            _Shopitem.Text = "";
            _Shopdate.Text = "";
            _PriceQuoted.Text = "";

            updateButton.Visibility = Visibility.Collapsed;

            // Updating table and make update button collapse
            Results();
        }


    }
}
