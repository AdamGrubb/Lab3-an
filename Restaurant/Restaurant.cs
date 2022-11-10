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
using System.Windows.Automation;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Lab3.Restaurant
{
    internal class Restaurants : IBookingSystem, INotifyPropertyChanged
    {
        public List<IBookingObject> BookableObjects { get; set; } = new List<IBookingObject>();

        private List<string> listTableID = new List<string>();
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

        protected void OnPropertyChanged(string propertytName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertytName));
            }
        }
        public async Task UpdateLists()
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
            }
        }
        private void FillTableID() //Gör en lista över vilka bord som finns samt hur många sitplatser de har. Används i en combobox för att välja bord.
        {
            ListTableID = new List<string>(BookableObjects.Select(table => $"{table.NameID.PadRight(15)}{table.MaxNumberOfGuests} Seats ").ToList());
        }
        public void FillDisplayAllBookings()
        {
            int indexOfTable = 0;
            DisplayAllBookings = new Dictionary<string, int[]>();
            Dictionary<int[], DateTime> extractBooking = new Dictionary<int[], DateTime>();

            foreach (IBookingObject tables in BookableObjects)
            {

                int indexOfBooking = 0;
                foreach (DateTimeAndGuestStruct date in tables.Booking)
                {
                    extractBooking.Add(new int[] { indexOfTable, indexOfBooking++}, date.BookedTime); //indexOfTable sätter Indexvärde för vilket table bokningen har inom List<IBookingObject> och indexOfBooking vilket index bokningen har inom IBookingObject Table.Booking. 
                }
                indexOfTable++;
            }
            foreach (KeyValuePair<int[], DateTime> bookings in extractBooking.OrderBy(date=> date.Value))//Här gör jag om en till en dictonary där datumet sorteras och sedan görs om till en string. 
            {
                string tableID = BookableObjects[bookings.Key[0]].NameID;
                string guestName = BookableObjects[bookings.Key[0]].Booking[bookings.Key[1]].BookingGuest.Name;
                string startSitting = bookings.Value.ToString("g");
                string endSitting = bookings.Value.Add(new TimeSpan(02, 00, 00)).ToString("H:mm");
                                
                DisplayAllBookings.Add($"{tableID} Guest: {guestName} {startSitting}-{endSitting}", bookings.Key);
            }
        }
        public async void SaveToFile()
        {
            try
            {
                List<Table> bordsBokningar = new List<Table>();
                foreach (IBookingObject table in BookableObjects)
                {
                    bordsBokningar.Add((Table)table);
                }
                string fileName = "BookingData.json";
                using FileStream createStream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(createStream,
                bordsBokningar);
                await createStream.DisposeAsync();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You do not have permission to overwrite BookingData.json file.", "Unauthorized Access");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Save to file failed, {e}", "Failed to save");
            }

        }
        public async Task LoadFromFile()
        {
            string fileName = "BookingData.json";
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                try
                {
                    using FileStream openStream = File.OpenRead(fileName);
                    List<Table>? bordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(openStream);
                    BookableObjects = new List<IBookingObject>();
                    if (bordsBokningar != null)
                    {
                        foreach (Table table in bordsBokningar)
                        {
                            BookableObjects.Add(table);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("You do not have permission to read BookingData.json file.\nCould not load data", "Unauthorized Access");
                }
                catch (JsonException)
                {
                    MessageBox.Show($"The BookingData is corrupt\nCould not load data", "Failed to load");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Load from file failed, {e}", "Failed to load");
                }

            }
            else
            {
                MessageBox.Show("Bookingdata not found", "Missing bookingdata");
            }
        }
        public bool AddBooking(DateTime chosentime, string guestName, int numberOfGuests, string comment, int indexOfTable) //Börjar med att jämföra datetime på den nya bokningen med befintliga bokningars datetime. Sen ifall antalet gäster överskrider målobjektets. Om båda är false så går bokningen igenom.
        {
            bool succsessfullBooking = false;
            if (BookableObjects[indexOfTable].Booking.Select(date => date.BookedTime).Any(dateAndTime => dateAndTime==chosentime|| chosentime > dateAndTime && chosentime < dateAndTime + new TimeSpan(2, 00, 0) || chosentime < dateAndTime && dateAndTime < chosentime + new TimeSpan(2, 00, 0))) //Här kollar den av så att inte tiden för hela sittningen inte krockar med befintlig bokning och sittning. Minus en minut så man fortfarande kan boka heltimmar.
            {
                MessageBox.Show("The time is not free or within another bookings sitting", "Cannot Book");
            }
            else if (numberOfGuests >BookableObjects[indexOfTable].MaxNumberOfGuests) //Kollar av antalet gäster som ska bokas med hur många platser det är på det bordet.
            {
                MessageBox.Show("Number of guests exceed max seats of table", "Cannot Book");
            }
            else
            {
                BookableObjects[indexOfTable].Booking.Add(new DateTimeAndGuestStruct(chosentime, new Guest(guestName, numberOfGuests, comment)));
                SaveToFile();
                MessageBox.Show("Added Booking");
                succsessfullBooking=true;
            }
            return succsessfullBooking;
        }
        public void RemoveBooking(int indexOfTable, int indexOfBooking)
        {
            BookableObjects[indexOfTable].Booking.RemoveAt(indexOfBooking);
            SaveToFile();
        }
        public void AddTable(string name, int seats) //Här har jag i enlighet med uppgiftens krav satt en gräns på att det inte kan vara mer än 5 bord som kan bli bokade samtidigt.
        {
            if (BookableObjects.Any(table => table.NameID.Equals(name)))
            {
                MessageBox.Show("A table with that name already exists", "Fail to add");
            }
            else if (BookableObjects.Count < 5)
            {
                BookableObjects.Add(new Table(name, seats, new List<DateTimeAndGuestStruct>()));
                FillTableID();
                MessageBox.Show($"{name} added", "Table Added");
                SaveToFile();
            }
            else
            {
                MessageBox.Show("You have reached max number of tables", "Cannot add more tables");
            }
        }
        public void RemoveTable(int tableIndex) //Kollar först av ifall det finns bokningar knutna till objektet Table och ger en varning att bokningar som tillhör table kommer försvinna.
        {
            if (BookableObjects[tableIndex].Booking.Count > 0)
            {
                MessageBoxResult removalOfTable = MessageBox.Show("The table has bookings.\nA removal will loose bookings.\nContinue?", "Warning", MessageBoxButton.YesNo);
                if (removalOfTable == MessageBoxResult.Yes)
                {
                    BookableObjects.RemoveAt(tableIndex);
                    MessageBox.Show($"Successfully removed table", "Completed");

                }
            }
            else
            {
                BookableObjects.RemoveAt(tableIndex);
                MessageBox.Show($"Successfully removed table", "Completed");
            }
            
            FillTableID();
            SaveToFile();

        }
        public async Task OpenExternalFile()
        {
            MessageBoxResult warningOfOverwrite = MessageBox.Show("Will overwrite booking data\nContinue?", "Warning", MessageBoxButton.YesNo);
            if (warningOfOverwrite == MessageBoxResult.Yes)
            {
                OpenFileDialog loadingFile = new OpenFileDialog();
                loadingFile.DefaultExt = ".json";
                loadingFile.Filter = "json files (.json)|*.json";

                var result = loadingFile.ShowDialog();

                if (result == true)
                {
                    try 
                    {
                        using (FileStream fs = (FileStream)loadingFile.OpenFile())
                        {
                            List<Table>? bordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(fs);
                            BookableObjects = new List<IBookingObject>();
                            if (bordsBokningar != null)
                            {
                                foreach (Table table in bordsBokningar)
                                {
                                    BookableObjects.Add(table);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to read file.\nCould not load data", "Unauthorized Access");
                    }
                    catch (JsonException)
                    {
                        MessageBox.Show($"The BookingData is corrupt\nCould not load data", "Failed to load");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Load from file failed, {e}", "Failed to load");
                    }
                    SaveToFile();
                    FillTableID();
                    FillDisplayAllBookings();
                }
            }
        }
        public async void SaveExternalFile()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "BookingData" + DateTime.Now.ToString("d");
            saveDialog.Filter = "json Files|*.json";
            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                List<Table> bordsBokningar = new List<Table>();
                foreach (IBookingObject table in BookableObjects)
                {
                    bordsBokningar.Add((Table)table);
                }
                try
                {
                    using FileStream createStream = File.Create(saveDialog.FileName);
                    await JsonSerializer.SerializeAsync(createStream,
                    bordsBokningar);
                    await createStream.DisposeAsync();
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show("Filename and/or filepath is to long", "Failed to save");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Save to file failed, {e}", "Failed to save");
                }
             
            }
        }
    }
}