using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    //Lägg till en Inotify och kör det istället för massa ObservableCollections.
    {
        public List<IBookingObject> BookableObjects { get; set; }
        public List<List<string>> SeperatedTables { get; set; } //Den här är tänkt att användas i en dynamisk Listbox-grej. Då varje List<ObservableCollection<string>> är ett "Table" och string är bokningarna.

        //public List<string> _listTableIB;
        public ObservableCollection<string> ListTableID { get; set; }
        public Dictionary<string, int[]> DisplayAllBookings { get; set; }

        public ObservableCollection<string> DisplayAllBookins { get; set; } //Testar lista alla bokningar
        private string[] LoadedFileText { get; set; }
        public Restaurants()
        {
            UpdateLists();
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void UpdateLists() //Den här måste felhanteras för att kunna ta emot en tom fil. Temporär lösning.
        {
            await LoadFromFile();
            if (BookableObjects==null)
            {
                BookableObjects = new List<IBookingObject>();
                ListTableID = new ObservableCollection<string>();
            }
            else
            {
                //FillSeperatedTables();
                FillTableID();
                FillDisplayAllBookings();
            }

        }
        public void FillTableID()
        {
            ListTableID = new ObservableCollection<string> (BookableObjects.Select(tableID => tableID.NameID).ToList());

        }
        public void FillDisplayAllBookings() // fixa null-hantering här
        {

            DisplayAllBookins = new ObservableCollection<string>(BookableObjects.SelectMany(table => table.Booking).Select(booking => $"Guest: {booking.BookingGuest.Name}    Date:{booking.BookedTime.ToString("f")}\n").ToList());
            int indexOfTable = 0;

            DisplayAllBookings = new Dictionary<string, int[]>();
            //DisplayAllBookings = BookableObjects.SelectMany(table=>table.Booking).ToDictionary(bookings=> bookings.BookedTime)

            //BookableObjects.SelectMany(table => table.Booking).ToDictionary(booking => $"Guest:{booking.BookingGuest} Bookedtime: {booking.BookedTime}", index => new int[] { table.ID, indexOfBooking++ });

            foreach (IBookingObject tables in BookableObjects)
            {
                int indexOfBooking = 0;
                var ärDetta= tables.Booking.ToDictionary(DateAndGuest => $"Table: {BookableObjects[indexOfTable].NameID} Guest:{DateAndGuest.BookingGuest.Name} Bookedtime: {DateAndGuest.BookedTime}", index => new int[] { indexOfTable, indexOfBooking++ });
                indexOfTable++;

                foreach (KeyValuePair<string, int[]> bookings in ärDetta)
                {
                    DisplayAllBookings.Add(bookings.Key, bookings.Value);
                }
            }
            for (int x =0; x<DisplayAllBookings.Count;x++)
            {

            }
            //DisplayAllBookings = BookableObjects.SelectMany(table => $"Table {table.NameID} {table.Booking)"


        }
        public void FillSeperatedTables(DateTime SelectedDay)
        {
            List<List<string>> listOfTables = new List<List<string>>();
            int indexOfTables = 0;
            foreach (IBookingObject table in BookableObjects)
            {
                listOfTables.Add(BookableObjects[indexOfTables].Booking.Where(booking =>booking.BookedTime.Date.Equals(SelectedDay.Date)).Select(tables => $"Guest: {tables.BookingGuest.Name} Date: {tables.BookedTime.ToString("dd/MM/yyyy H:mm")}").ToList());
                //listOfTables.Add(new ObservableCollection<string>(BookableObjects[indexOfTables].Booking.Select(d => $"Guest: {d.BookingGuest.Name} Date: {d.BookedTime.ToString("dd/MM/yyyy H:mm")}").ToList()));
                indexOfTables++;
            }

            SeperatedTables = listOfTables;
        }
        private async Task SaveToFile() //Kanske sa ha en UppdateList på SaveToFile??
        {
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
        public async Task LoadFromFile()
        {
            string fileName = "BookingData.json";
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists) //Hur felhanterar man att det försöker ladda in en tom fil??
            {
                using FileStream openStream = File.OpenRead(fileName);
                List<Table>? BordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(openStream);
                BookableObjects = new List<IBookingObject>();
                foreach (Table table in BordsBokningar)
                {
                    table.Booking=table.Booking.OrderBy(date => date.BookedTime).ToList(); //Är tänkt att sortera listan innan den läggs över i IBookableObjects.
                    BookableObjects.Add(table);
                }
                
            }
        }
        public void AddBooking(DateTime Chosentime, string GuestName, int numberOfGuests, int tableID) //(DateTime bookedTime, Guest bookingGuest (string name, int numbersOfGuests)) 
        {
            if (BookableObjects[tableID].Booking.Any(dateAndTime => dateAndTime.BookedTime.Equals(Chosentime)))
            {
                MessageBox.Show("The time is not available", "Cannot Book");
            }
            else
            {
                BookableObjects[tableID].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(GuestName, numberOfGuests)));
                SaveToFile();
                MessageBox.Show("Added Booking"); //Egentligen här också. Async förstör
                //FillSeperatedTables();
            }


        }
        public void AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment, int tableID) //Om man väljer att lägga in en kommentar. Här är det ju inga problem att lägga till kommentar efter json serialisering.
        {
            if (BookableObjects[tableID].Booking.Any(dateAndTime => dateAndTime.Equals(Chosentime)))
            {
                MessageBox.Show("The time is not available", "Cannot Book");
            }
            else
            {
                BookableObjects[tableID].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(guestName, numberOfGuests, comment)));
                SaveToFile();
                MessageBox.Show("Added Booking, with comment"); //Egentligen här också. Async förstör
                //FillSeperatedTables();
            }

        }
        public async void RemoveBooking(int IndexOfTable, int IndexOfBooking)
        {
            
            BookableObjects[IndexOfTable].Booking.RemoveAt(IndexOfBooking);
            await SaveToFile();
            UpdateLists(); //Du måste fixa UpdateLists så att den inte disconnectar TableChoise.
        }
        public async void AddTable(string name) //Här kan du lägga in så att man kan ställa in hur bordet ska vara. Den verkar inte uppdatera Table. Varför inte?
        {
            BookableObjects.Add(new Table(name, 4, false));
            await SaveToFile();
            MessageBox.Show($"{name} added", "Table Added");
            ListTableID.Add(name);
            //FillSeperatedTables();
        }
    }
}
