using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Tables;
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
    internal class Restaurants : IBookingSystem //Kanske lägga alla Lists här som observable collections? Så att alla Listor, Fixa en export och Load knapp som gör att man kan ladda bokningar och sedan skriver över default-filen.
    {
        public List<IBookingObject> BookableObjects { get; set; }

        public List<string> ListTableID;
        public Dictionary<string, int[]> DisplayAllBookings = new Dictionary<string, int[]>();


        //public ObservableCollection<string> DisplayAllBookins { get; set; } //Testar lista alla bokningar
        public Restaurants()
        {}

        
        

        public async Task UpdateLists() //Den här måste felhanteras för att kunna ta emot en tom fil. Temporär lösning.
        {
            await LoadFromFile();
            //await Task.Delay(4000);
            if (BookableObjects==null)
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
            ListTableID = new List<string> (BookableObjects.Select(tableID => tableID.NameID).ToList());
        }
        private void FillDisplayAllBookings() // fixa null-hantering här
        {
            int indexOfTable = 0;

            DisplayAllBookings = new Dictionary<string, int[]>();

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
        public async Task AddBooking(DateTime Chosentime, string GuestName, int numberOfGuests, int tableID) //(DateTime bookedTime, Guest bookingGuest (string name, int numbersOfGuests)) 
        {
            if (BookableObjects[tableID].Booking.Any(dateAndTime => dateAndTime.BookedTime.Equals(Chosentime)))
            {
                MessageBox.Show("The time is not available", "Cannot Book");
            }
            else
            {
                BookableObjects[tableID].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(GuestName, numberOfGuests)));
                await SaveToFile();
                //await Task.Delay(3000);
                MessageBox.Show("Added Booking"); //Egentligen här också. Async förstör
            }


        }
        public async Task AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment, int tableID) //Om man väljer att lägga in en kommentar. Här är det ju inga problem att lägga till kommentar efter json serialisering.
        {
            if (BookableObjects[tableID].Booking.Any(dateAndTime => dateAndTime.Equals(Chosentime)))
            {
                MessageBox.Show("The time is not available", "Cannot Book");
            }
            else
            {
                BookableObjects[tableID].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(guestName, numberOfGuests, comment)));
                await SaveToFile();
                MessageBox.Show("Added Booking, with comment"); //Egentligen här också. Async förstör
                await UpdateLists();
                //FillSeperatedTables();
            }

        }
        public async Task RemoveBooking(int IndexOfTable, int IndexOfBooking)
        {
            BookableObjects[IndexOfTable].Booking.RemoveAt(IndexOfBooking);
            await SaveToFile();
        }
        public async Task AddTable(string name) //Här kan du lägga in så att man kan ställa in hur bordet ska vara. Den verkar inte uppdatera Table. Varför inte?
        {
            BookableObjects.Add(new Table(name, 4, false));
            await SaveToFile();
            MessageBox.Show($"{name} added", "Table Added");

        }
        public async Task RemoveTable(int tableIndex) //Här kan du lägga in så att man kan ställa in hur bordet ska vara. Den verkar inte uppdatera Table. Varför inte?
        {
            if (BookableObjects[tableIndex].Booking.Count >0)
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
            await SaveToFile();

        }
    }
}
