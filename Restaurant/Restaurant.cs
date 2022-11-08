using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Tables;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Lab3.Restaurant
{
    internal class Restaurants : IBookingSystem, INotifyPropertyChanged //Kanske lägga alla Lists här som observable collections? Så att alla Listor, Fixa en export och Load knapp som gör att man kan ladda bokningar och sedan skriver över default-filen.
    {
        public List<IBookingObject> BookableObjects { get; set; } = new List<IBookingObject>();//Declare as nullabel

        private List<string> listTableID = new List<string>(); //Declare as nullabel?
        public List<string> ListTableID
        {
            get
            {
                return listTableID;
            }
            set
            {
                listTableID = value;
                OnPropertyChanged(nameof(ListTableID));
            }
        }
        public Dictionary<string, int[]> DisplayAllBookings = new Dictionary<string, int[]>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string intName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(intName));
            }
        }
        public async Task UpdateLists() //Den här måste felhanteras för att kunna ta emot en tom fil. Temporär lösning.
        {
            await LoadFromFile();
            if (BookableObjects == null)
            {
                BookableObjects = new List<IBookingObject>();
                ListTableID = new List<string>();
            }
            else
            {
                FillTableID();
                FillDisplayAllBookings();
            }
        }
        private void FillTableID()
        {
            ListTableID = new List<string>(BookableObjects.Select(tableID => tableID.NameID).ToList());
        }
        public void FillDisplayAllBookings() // fixa null-hantering här
        {
            int indexOfTable = 0;

            DisplayAllBookings = new Dictionary<string, int[]>();

            foreach (IBookingObject tables in BookableObjects)
            {
                int indexOfBooking = 0;
                var ärDetta = tables.Booking.ToDictionary(DateAndGuest => $"Table: {BookableObjects[indexOfTable].NameID}\nGuest: {DateAndGuest.BookingGuest.Name}\nBookedtime: {DateAndGuest.BookedTime.ToString("f")}", index => new int[] { indexOfTable, indexOfBooking++ });
                indexOfTable++;

                foreach (KeyValuePair<string, int[]> bookings in ärDetta)
                {
                    DisplayAllBookings.Add(bookings.Key, bookings.Value);
                }
            }
        }

        private async void SaveToFile() //Kanske sa ha en UppdateList på SaveToFile??
        {
            //await Task.Delay(5000);
            List<Table> BordsBokningar = new List<Table>();
            foreach (IBookingObject table in BookableObjects) //Här kanske du kan spara ner det som IBookingobjekt från början.
            {
                BordsBokningar.Add((Table)table);
            }
            string fileName = "BookingData.json";
            using FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream,
            BordsBokningar);
            await createStream.DisposeAsync();
        }
        private async Task LoadFromFile()
        {
            //await Task.Delay(5000);
            string fileName = "BookingData.json";
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists) //Hur felhanterar man att det försöker ladda in en tom fil??
            {
                using FileStream openStream = File.OpenRead(fileName);
                List<Table>? BordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(openStream);
                BookableObjects = new List<IBookingObject>();
                if (BordsBokningar != null)
                {
                    foreach (Table table in BordsBokningar)
                    {
                        BookableObjects.Add(table);
                    }
                }
            }
        }
        public void AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment, int IndexOfTable) //om du ska implementera tid för sittning så kan du sätta in 
        {
            if (BookableObjects[IndexOfTable].Booking.Select(date => date.BookedTime).Any(dateAndTime => dateAndTime==Chosentime|| Chosentime > dateAndTime && Chosentime < dateAndTime + new TimeSpan(1, 59, 0) || Chosentime < dateAndTime && dateAndTime < Chosentime + new TimeSpan(1, 59, 0))) //Ta bort Timespan och lägg till lenght of sitting och lägg alltid till minus en minut. Och kolla av så att den fungerar.
            //if (BookableObjects[IndexOfTable].Booking.Select(date=> date.BookedTime).Any(dateAndTime => dateAndTime.Equals(Chosentime)))
            {
                MessageBox.Show("The time is not free or within another bookings sitting", "Cannot Book");
            }
            else
            {
                BookableObjects[IndexOfTable].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(guestName, numberOfGuests, comment)));
                foreach (IBookingObject table in BookableObjects)
                {
                    table.Booking = table.Booking.OrderBy(date => date.BookedTime).ToList(); //Är tänkt att sortera listan innan den läggs över i IBookableObjects.
                }
                SaveToFile();
                MessageBox.Show("Added Booking");
            }
        }
        //private bool IfWithinTimeSpan(DateTime addbooking, DateTime existingBooking) bättre att lägga den som en metod?
        //{
        //    if ()
        //}
        public void RemoveBooking(int IndexOfTable, int IndexOfBooking)
        {
            BookableObjects[IndexOfTable].Booking.RemoveAt(IndexOfBooking);
            SaveToFile();
        }
        public void AddTable(string name)
        {
            if (BookableObjects.Count < 5)
            {
                BookableObjects.Add(new Table(name, 4, new List<DateTimeAndGuestStruct>()));
                FillTableID();
                MessageBox.Show($"{name} added", "Table Added");
                SaveToFile();
            }
            else
            {
                MessageBox.Show("You have reached max number of tables", "Cannot add more tables");
            }




        }
        public void RemoveTable(int tableIndex) //Här kan du lägga in så att man kan ställa in hur bordet ska vara. Den verkar inte uppdatera Table. Varför inte?
        {
            if (BookableObjects[tableIndex].Booking.Count > 0)
            {
                MessageBoxResult removalOfTable = MessageBox.Show("The table has bookings.\nA removal will loose bookings.\nContinue?", "Warning", MessageBoxButton.YesNo);
                if (removalOfTable == MessageBoxResult.Yes)
                {
                    BookableObjects.RemoveAt(tableIndex);

                }
            }
            else
            {
                BookableObjects.RemoveAt(tableIndex);
            }
            MessageBox.Show($"Successfully removed table", "Completed");
            FillTableID();
            SaveToFile();

        }
        public async Task OpenExternalFile() //Kolla upp mer om den här grejen
        {

            OpenFileDialog LoadingFile = new OpenFileDialog();
            LoadingFile.DefaultExt = ".json";
            LoadingFile.Filter = "json files (.json)|*.json";

            var result = LoadingFile.ShowDialog();

            if (result == true)
            {
                await Task.Delay(4000);
                using (FileStream fs = (FileStream)LoadingFile.OpenFile())
                {
                    List<Table>? BordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(fs);
                    BookableObjects = new List<IBookingObject>();
                    if (BordsBokningar != null)
                    {
                        foreach (Table table in BordsBokningar)
                        {
                            table.Booking = table.Booking.OrderBy(date => date.BookedTime).ToList(); //Är tänkt att sortera listan innan den läggs över i IBookableObjects.
                            BookableObjects.Add(table);
                        }


                    }
                }
                FillTableID();
                FillDisplayAllBookings();
            }

        }
        public async void SaveExternalFile() //kolla upp mer om den här grejen
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "json Files|*.json";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                    List<Table> BordsBokningar = new List<Table>();
                    foreach (IBookingObject table in BookableObjects) //Här kanske du kan spara ner det som IBookingobjekt från början.
                    {
                        BordsBokningar.Add((Table)table);
                    }
                    using FileStream createStream = File.Create(saveDialog.FileName);
                    await JsonSerializer.SerializeAsync(createStream,
                    BordsBokningar);
                    await createStream.DisposeAsync();
             
            }
        }
    }
}