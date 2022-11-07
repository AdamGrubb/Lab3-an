using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Restaurant;
using Lab3.Tables;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Restaurants SnuskigaFisken = new Restaurants();
        private int _numberOfPerson = 1;
        List<TimeSpan> Availabletimes = new List<TimeSpan> { { new TimeSpan ( 16, 00, 00 ) },{ new TimeSpan(17, 00, 00) }, { new TimeSpan(17, 30, 00) },{ new TimeSpan(18, 00, 00) },{ new TimeSpan(18, 30, 00) } }; //Helt enkelt ändra till List<string>.
        List<string> tables=new List<string> ();
        public MainWindow()
        {
            InitializeComponent();
            TimePicker.ItemsSource = Availabletimes; //Denna kanske borde läggas mot att uppdatera alla, det måste den om man ska ändra på tidsspannet.
            NumberOfPersons.Text = _numberOfPerson.ToString(); //Behöver denna vara ToString?
            Update();
        }

        private void SubtractGuest_Click(object sender, RoutedEventArgs e)
        {
            if (_numberOfPerson > 1)
            {
                _numberOfPerson--;
                NumberOfPersons.Text = _numberOfPerson.ToString();
            }

        }
        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (_numberOfPerson < 8)
            {
                _numberOfPerson++;
                NumberOfPersons.Text = _numberOfPerson.ToString();
            }

            else
            {
                MessageBox.Show("For bookings over 8 person, make custom booking.", "Big Booking"); //Om jag orkar göra en custom booking-grej. Antagligen inte. Annars tabort.
            }
        }

        private async void BookingButton_Click(object sender, RoutedEventArgs e)
        {
            BookingButton.IsEnabled = false;
            if (UserInputNullCheck())
            {
                int guestNumbers = _numberOfPerson;
                DateTime dateAndTime = ChosenDate.SelectedDate.Value;
                dateAndTime = dateAndTime.Add((TimeSpan)TimePicker.SelectedValue);
                string nameOfGuest = userNameInput.Text;
                int tableID = ChooseTable.SelectedIndex;

                if (ExtraInfoBox.Text != string.Empty) //Frågan är ifall jag bara ska använda en Construktor och låta Commentaren vara null ifall ingen kommentar finns.
                {
                    string ExtraInfo = ExtraInfoBox.Text;
                    await SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, ExtraInfo, tableID);
                }
                else await SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, tableID);
            }
            else MessageBox.Show("Du har inte fyllt i alla fält");
            BookingButton.IsEnabled = true;
            Update();

        }

        private bool UserInputNullCheck()
        {
            if (ChooseTable.SelectedValue ==null || userNameInput.Text == null || ChosenDate.SelectedDate == null || TimePicker.SelectedValue == null)
            {
               return false;
            }
            else return true;
        }

        private async void AddTableButton_Click(object sender, RoutedEventArgs e)
        {
            await SnuskigaFisken.AddTable(tableName.Text);
            Update();
        }
        private async void deleteBooking_Click(object sender, RoutedEventArgs e)
        {
            deleteBooking.IsEnabled = false;
            await DeleteBookingFromDisplayAll();
            deleteBooking.IsEnabled = true;
            Update();
        }
        private async Task DeleteBookingFromDisplayAll()
        {
            int[] tableIndex = new int[] {};
            if (PlaceForBookings.SelectedValue != null)
            {
                await SnuskigaFisken.RemoveBooking(tableIndex[0], tableIndex[1]);
            }
            Update();

        }
        private void ShowAllBookings_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private async void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (ManagerViewTables.SelectedItem != null)
            {
                DeleteTable.IsEnabled=false;
                await SnuskigaFisken.RemoveTable(ManagerViewTables.SelectedIndex);
                DeleteTable.IsEnabled = true;
            }
            Update();

        }
        public void format(DateTime date) //Lite allan ballan, foreacha listan och adda till Listboxen, hämta ut Index för table och booking. Om de stämmer överens med datumet som du valt kan du lägga till dem med Bold och lite större. Annars Läggs till med lite mindre.
        {
            int indexOfTable = 0;

            Dictionary<string, int[]> DisplayAllBookings = new Dictionary<string, int[]>();

            foreach (IBookingObject tables in SnuskigaFisken.BookableObjects)
            {
                int indexOfBooking = 0;
                var ärDetta = tables.Booking.ToDictionary(DateAndGuest => $"Table: {SnuskigaFisken.BookableObjects[indexOfTable].NameID} Guest:{DateAndGuest.BookingGuest.Name} Bookedtime: {DateAndGuest.BookedTime}", index => new int[] { indexOfTable, indexOfBooking++ });
                indexOfTable++;

                foreach (KeyValuePair<string, int[]> bookings in ärDetta)
                {
                    DisplayAllBookings.Add(bookings.Key, bookings.Value);
                }
            }
        }
        public async void Update()
        {
            await SnuskigaFisken.UpdateLists();
            PlaceForBookings.ItemsSource = SnuskigaFisken.DisplayAllBookings;
            PlaceForBookings.DisplayMemberPath = "Key";
            PlaceForBookings.SelectedValuePath = "Value";
            ChooseTable.ItemsSource = SnuskigaFisken.ListTableID;
        }


    }
}