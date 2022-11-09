using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
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
using Microsoft.Win32;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Restaurants Vesuvio = new Restaurants();
        private List<TimeSpan> Availabletimes;
        private List<string> tables = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            Availabletimes = FillTimeSpan();
            TimePicker.ItemsSource = Availabletimes;
            ChosenDate.DisplayDateStart = DateTime.Today;
            DataContext = Vesuvio;
            NumberOfSeats.Text = "2";
            NumberOfPersons.Text = "1";
            Update();
        }
        private List<TimeSpan> FillTimeSpan() //Fyller på combobox med valbara tider.
        {
            List<TimeSpan> fillTimeSpan = new List<TimeSpan>();
            TimeSpan StartTime = new TimeSpan(16, 00, 00);
            for (int i = 0; i < 9; i++)
            {
                fillTimeSpan.Add(StartTime);
                StartTime = StartTime.Add(new TimeSpan(00, 30, 00));
            }
            return fillTimeSpan;

        }
        private void SubtractGuest_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(NumberOfPersons.Text, out int SubtracGuests) && SubtracGuests > 1)
            {
                SubtracGuests--;
                NumberOfPersons.Text = SubtracGuests.ToString();
            }
        }
        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(NumberOfPersons.Text, out int AddGuests) && AddGuests < 8)
            {
                AddGuests++;
                NumberOfPersons.Text = AddGuests.ToString();
            }

        }
        private void SubtractSeats_Click(object sender, RoutedEventArgs e)
        {

            if (Int32.TryParse(NumberOfSeats.Text, out int SubtractSeat) && SubtractSeat > 1)
            {
                SubtractSeat--;
                NumberOfSeats.Text = SubtractSeat.ToString();
            }
        }

        private void AddSeat_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(NumberOfSeats.Text, out int AddSeat) && AddSeat < 8)
            {
                AddSeat++;
                NumberOfSeats.Text = AddSeat.ToString();
            }
        }

        private void BookingButton_Click(object sender, RoutedEventArgs e) 
        {
            if (UserInputNullCheck() && Int32.TryParse(NumberOfPersons.Text, out int guestNumbers)) //Kolla av via UserInputNullCheck att ingen av fälten som stoppas in i Add.Booking är tomma eller null. Samt gör ytterligare en Int32.TryParse av guestNumbers innan de stoppas in som parameter.
            {
                DateTime dateAndTime = ChosenDate.SelectedDate.Value;
                dateAndTime = dateAndTime.Add((TimeSpan)TimePicker.SelectedValue); //Lägger ihop det valda datumet med den valda tiden för bokningen.
                string nameOfGuest = userNameInput.Text;
                int tableIndex = ChooseTable.SelectedIndex;
                string ExtraInfo = ExtraInfoBox.Text;
                if (Vesuvio.AddBooking(dateAndTime, nameOfGuest, guestNumbers, ExtraInfo, tableIndex)) //AddBooking kollar av om bokningen kan genomföras och skickar true om det funkar. Ifall t ex man dubbelbokar så skickar den false. På det sättet så rensas fälten för inmatning endast om bokningen gick igenom.
                {
                    ResetUserInput();
                }
            }
            else MessageBox.Show("Empty fields, please try again");
            UpdateBookingsList();

        }
        private bool UserInputNullCheck()
        {
            if (ChooseTable.SelectedValue == null || String.IsNullOrWhiteSpace(userNameInput.Text) || ChosenDate.SelectedDate == null || TimePicker.SelectedValue == null)
            {
                return false;
            }
            else return true;
        }
        private void ResetUserInput()
        {
            userNameInput.Text = "";
            NumberOfPersons.Text = "1";
            ChooseTable.SelectedIndex = -1;
            TimePicker.SelectedIndex = -1;
            ChosenDate.SelectedDate = null;
            ExtraInfoBox.Text = "";
        }

        private void AddTableButton_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(tableName.Text) == false)
            {
                if (Int32.TryParse(NumberOfSeats.Text, out int intSeats))
                {
                    Vesuvio.AddTable(tableName.Text, intSeats);
                    NumberOfSeats.Text = "2";
                    tableName.Text = "";
                }
            }
            else
            {
                MessageBox.Show("You must set a name for the table");
            }


        }
        private void deleteBooking_Click(object sender, RoutedEventArgs e)
        {

            DeleteBookingFromDisplayAll();
            UpdateBookingsList();

        }
        private void DeleteBookingFromDisplayAll() //Jag har valt att ha bokningen i Listboxen som ett dictonary där Key är en string med bokningsinfo och value är en int-array där Index[0] är index för IBookingObject (table) och Index[1] är index för själva bokningen.
        {
            int[] tableIndex = new int[] { };
            if (PlaceForBookings.SelectedValue != null)
            {
                tableIndex = (int[])PlaceForBookings.SelectedValue;
                Vesuvio.RemoveBooking(tableIndex[0], tableIndex[1]);
            }

        }
        private void ShowAllBookings_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (ManagerViewTables.SelectedItem != null)
            {
                Vesuvio.RemoveTable(ManagerViewTables.SelectedIndex);
                UpdateBookingsList();
            }
        }
        public async void Update() //Gör en await här så att det laddar in all information innan den börja göra nya listor av bokningsdata.
        {
            await Vesuvio.UpdateLists();
            UpdateBookingsList();
        }
        private void UpdateBookingsList()
        {
            Vesuvio.FillDisplayAllBookings();
            BookingInfo.Text = "";
            PlaceForBookings.ItemsSource = Vesuvio.DisplayAllBookings;
            PlaceForBookings.DisplayMemberPath = "Key";
            PlaceForBookings.SelectedValuePath = "Value";
        }

        private async void LoadBookings_Click(object sender, RoutedEventArgs e)
        {
            await Vesuvio.OpenExternalFile();
            PlaceForBookings.ItemsSource = Vesuvio.DisplayAllBookings;
            PlaceForBookings.DisplayMemberPath = "Key";
            PlaceForBookings.SelectedValuePath = "Value";
        }
        private void ExportdBookings_Click(object sender, RoutedEventArgs e)
        {
            Vesuvio.SaveExternalFile();
        }
        private void PlaceForBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] tableAndBookingIndex = new int[] { };
            if (PlaceForBookings.SelectedValue != null)
            {
                tableAndBookingIndex = (int[])PlaceForBookings.SelectedValue;
                BookingInfo.Text = $"{Vesuvio.BookableObjects[tableAndBookingIndex[0]].NameID}\nName of guest: {Vesuvio.BookableObjects[tableAndBookingIndex[0]].Booking[tableAndBookingIndex[1]].BookingGuest.Name}\nNumber of guest: {Vesuvio.BookableObjects[tableAndBookingIndex[0]].Booking[tableAndBookingIndex[1]].BookingGuest.NumbersOfGuests}\nComments:\n{Vesuvio.BookableObjects[tableAndBookingIndex[0]].Booking[tableAndBookingIndex[1]].BookingGuest.Comments}";
            }
        }
    }
}