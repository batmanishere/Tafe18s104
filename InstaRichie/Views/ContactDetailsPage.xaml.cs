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
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;
using System.Text.RegularExpressions;

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
            // Creating table
            conn.CreateTable<Contacts>();
            var query = conn.Table<Contacts>();
            ContactList.ItemsSource = query.ToList();
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Null Checks
                if (ContactIDTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Contact ID field must not be left blank.", "Please enter your Contact ID.");
                    await dialog.ShowAsync();
                }
                else if (FirstNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name field must not be left blank.", "Please enter your First Name.");
                    await dialog.ShowAsync();
                }
                else if (LastNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name field must not be left blank.", "Please enter your Last Name.");
                    await dialog.ShowAsync();
                }
                else if (CompanyNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name field must not be left blank.", "Please enter your Company Name.");
                    await dialog.ShowAsync();
                }
                else if (MobilePhoneTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone Number field must not be left blank.", "Please enter your Mobile Phone Number.");
                    await dialog.ShowAsync();
                }

                //Input Validation 
                else if (IsDigitsOnly(ContactIDTextBox.Text.ToString()) == false)
                {
                    MessageDialog dialog = new MessageDialog("Contact ID field invalid.", "Please re-enter your Contact ID.");
                    await dialog.ShowAsync();
                }
                else if (IsDigitsOnly(MobilePhoneTextBox.Text.ToString()) == false)
                {
                    MessageDialog dialog = new MessageDialog("Contact ID field invalid.", "Please re-enter your Contact ID.");
                    await dialog.ShowAsync();
                }

                else
                {   // Inserts the data
                    conn.Insert(new Contacts()
                    {
                        ContactID = Int32.Parse(ContactIDTextBox.Text),
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        CompanyName = CompanyNameTextBox.Text,
                        MobilePhoneNumber = MobilePhoneTextBox.Text,

                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Invalid Data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Contact ID already exists, Try Different Contact ID", "Oops..!");
                    await dialog.ShowAsync();
                }

            }
        }


        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Null Checks
                if (ContactIDTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Contact ID field must not be left blank.", "Please enter your Contact ID.");
                    await dialog.ShowAsync();
                }
                else if (FirstNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name field must not be left blank.", "Please enter your First Name.");
                    await dialog.ShowAsync();
                }
                else if (LastNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name field must not be left blank.", "Please enter your Last Name.");
                    await dialog.ShowAsync();
                }
                else if (CompanyNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name field must not be left blank.", "Please enter your Company Name.");
                    await dialog.ShowAsync();
                }
                else if (MobilePhoneTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone Number field must not be left blank.", "Please enter your Mobile Phone Number.");
                    await dialog.ShowAsync();
                }
                else
                {
                    int ContactsLabel = ((Contacts)ContactList.SelectedItem).ContactID;


                    var queryEdit = conn.Query<Contacts>(
                        "UPDATE Contacts SET ContactID = '" +
                        ContactIDTextBox.Text + "', FirstName = '" +
                        FirstNameTextBox.Text + "', LastName = '" +
                        LastNameTextBox.Text + "', CompanyName = '" +
                        CompanyNameTextBox.Text + "', MobilePhoneNumber = '" +
                        MobilePhoneTextBox.Text + "' WHERE ContactID = " + ContactsLabel);
                    Results();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to edit!", "Oops..!");
                await ClearDialog.ShowAsync();
            }

        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Doing this will delete the contact permanently. Continue?", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {

                try
                {
                    int ContactsLabel = ((Contacts)ContactList.SelectedItem).ContactID;
                    var querydel = conn.Query<Contacts>("DELETE FROM Contacts WHERE ContactID=" + ContactsLabel);
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }

        }

        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactList.SelectedIndex != -1)
            {
                ContactIDTextBox.Text = Convert.ToString(((Contacts)ContactList.SelectedItem).ContactID);
                FirstNameTextBox.Text = ((Contacts)ContactList.SelectedItem).FirstName;
                LastNameTextBox.Text = ((Contacts)ContactList.SelectedItem).LastName;
                CompanyNameTextBox.Text = ((Contacts)ContactList.SelectedItem).CompanyName;
                MobilePhoneTextBox.Text = ((Contacts)ContactList.SelectedItem).MobilePhoneNumber;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
