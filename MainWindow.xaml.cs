﻿using System;
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
        Restaurants SnuskigaFisken = new Restaurants();
        private int _numberOfPerson = 1;
        List<TimeSpan> Availabletimes = new List<TimeSpan> { { new TimeSpan(16, 00, 00) }, { new TimeSpan(17, 00, 00) }, { new TimeSpan(17, 30, 00) }, { new TimeSpan(18, 00, 00) }, { new TimeSpan(18, 30, 00) } }; //Helt enkelt ändra till List<string>.
        List<string> tables = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            TimePicker.ItemsSource = Availabletimes; //Denna kanske borde läggas mot att uppdatera alla, det måste den om man ska ändra på tidsspannet.
            NumberOfPersons.Text = _numberOfPerson.ToString(); //Behöver denna vara ToString?
            DataContext = SnuskigaFisken;
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
        }

        private void BookingButton_Click(object sender, RoutedEventArgs e) //Kolla om du behöver snygga till här.
        {
            if (UserInputNullCheck())
            {
                int guestNumbers = _numberOfPerson;
                DateTime dateAndTime = ChosenDate.SelectedDate.Value;
                dateAndTime = dateAndTime.Add((TimeSpan)TimePicker.SelectedValue);
                string nameOfGuest = userNameInput.Text;
                int tableID = ChooseTable.SelectedIndex;
                string ExtraInfo = ExtraInfoBox.Text;
                SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, ExtraInfo, tableID);
                ResetUserInput();
            }
            else MessageBox.Show("Du har inte fyllt i alla fält");
            UpdateBookingsList();

        }
        private void ResetUserInput()
        {
            userNameInput.Text = "";
            NumberOfPersons.Text = "1";
            ChooseTable.SelectedIndex = -1;
            TimePicker.SelectedIndex = -1;
            ChosenDate.SelectedDate = null;
        }

        private bool UserInputNullCheck()
        {
            if (ChooseTable.SelectedValue == null || userNameInput.Text == null || ChosenDate.SelectedDate == null || TimePicker.SelectedValue == null)
            {
                return false;
            }
            else return true;
        }

        private void AddTableButton_Click(object sender, RoutedEventArgs e)
        {
            SnuskigaFisken.AddTable(tableName.Text);
        }
        private void deleteBooking_Click(object sender, RoutedEventArgs e)
        {

            DeleteBookingFromDisplayAll();
            UpdateBookingsList();
        }
        private void DeleteBookingFromDisplayAll()
        {
            int[] tableIndex = new int[] { };
            if (PlaceForBookings.SelectedValue != null)
            {
                tableIndex = (int[])PlaceForBookings.SelectedValue;
                SnuskigaFisken.RemoveBooking(tableIndex[0], tableIndex[1]);
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
                SnuskigaFisken.RemoveTable(ManagerViewTables.SelectedIndex);
                UpdateBookingsList();
            }

        }
        public async void Update()
        {
            await SnuskigaFisken.UpdateLists();
            UpdateBookingsList();
        }
        private void UpdateBookingsList()
        {
            SnuskigaFisken.FillDisplayAllBookings();
            PlaceForBookings.ItemsSource = SnuskigaFisken.DisplayAllBookings;
            PlaceForBookings.DisplayMemberPath = "Key";
            PlaceForBookings.SelectedValuePath = "Value";
        }

        private async void LoadBookings_Click(object sender, RoutedEventArgs e)
        {
            await SnuskigaFisken.OpenExternalFile();
            PlaceForBookings.ItemsSource = SnuskigaFisken.DisplayAllBookings;
            PlaceForBookings.DisplayMemberPath = "Key";
            PlaceForBookings.SelectedValuePath = "Value";
            
        }

        private void ExportdBookings_Click(object sender, RoutedEventArgs e)
        {
            SnuskigaFisken.SaveExternalFile();
        }
    }
}