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

        protected void OnPropertyChanged(string intName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(intName));
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
                //FillDisplayAllBookings(); Testa ännu mer, Verkar som den här inte behövs
            }
        }
        private void FillTableID()
        {
            ListTableID = new List<string>(BookableObjects.Select(table => $"{table.NameID.PadRight(15)}{table.MaxNumberOfGuests} Seats ").ToList());
        }
        public void FillDisplayAllBookings()
        {
            int indexOfTable = 0;
            DisplayAllBookings = new Dictionary<string, int[]>();
            Dictionary<int[], DateTime> ExtractBooking = new Dictionary<int[], DateTime>();

            foreach (IBookingObject tables in BookableObjects)
            {

                int indexOfBooking = 0;
                foreach (DateTimeAndGuestStruct date in tables.Booking)
                {
                    ExtractBooking.Add(new int[] { indexOfTable, indexOfBooking++}, date.BookedTime);
                }
                indexOfTable++;
            }
            foreach (KeyValuePair<int[], DateTime> bookings in ExtractBooking.OrderBy(date=> date.Value))//Här gör jag om en till en dictonary när den är sorterad, För att göra det hela lite tydligare så tilldelar jag variablar för varje del i strängen.
            {
                string tableID = BookableObjects[bookings.Key[0]].NameID;
                string guestName = BookableObjects[bookings.Key[0]].Booking[bookings.Key[1]].BookingGuest.Name;
                string startSitting = bookings.Value.ToString("g");
                string endSitting = bookings.Value.Add(new TimeSpan(02, 00, 00)).ToString("H:mm");
                                
                DisplayAllBookings.Add($"{tableID} Guest: {guestName} {startSitting}-{endSitting}", bookings.Key);
            }
        }
        public async void SaveToFile() //Kanske sa ha en UppdateList på SaveToFile??
        {
            try
            {
                List<Table> BordsBokningar = new List<Table>();
                foreach (IBookingObject table in BookableObjects)
                {
                    BordsBokningar.Add((Table)table);
                }
                string fileName = "BookingData.json";
                using FileStream createStream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(createStream,
                BordsBokningar);
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
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("You do not have permission to read BookingData.json file.\nCould not load data", "Unauthorized Access");
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("BookingData.json is empty", "Failed to load");
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
        public bool AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment, int IndexOfTable) //Börjar med att jämföra datetime på den nya bokningen med befintliga bokningars datetime. Sen ifall antalet gäster överskrider målobjektets. Om båda är false så går bokningen igenom.
        {
            bool succsessfullBooking = false;
            if (BookableObjects[IndexOfTable].Booking.Select(date => date.BookedTime).Any(dateAndTime => dateAndTime==Chosentime|| Chosentime > dateAndTime && Chosentime < dateAndTime + new TimeSpan(1, 59, 0) || Chosentime < dateAndTime && dateAndTime < Chosentime + new TimeSpan(1, 59, 0))) //Ta bort Timespan och lägg till lenght of sitting och lägg alltid till minus en minut. Och kolla av så att den fungerar.
            {
                MessageBox.Show("The time is not free or within another bookings sitting", "Cannot Book");
            }
            else if (numberOfGuests >BookableObjects[IndexOfTable].MaxNumberOfGuests)
            {
                MessageBox.Show("Number of guests exceed max seats of table", "Cannot Book");
            }
            else
            {
                BookableObjects[IndexOfTable].Booking.Add(new DateTimeAndGuestStruct(Chosentime, new Guest(guestName, numberOfGuests, comment)));
                SaveToFile();
                MessageBox.Show("Added Booking");
                succsessfullBooking=true;
            }
            return succsessfullBooking;
        }
        public void RemoveBooking(int IndexOfTable, int IndexOfBooking)
        {
            BookableObjects[IndexOfTable].Booking.RemoveAt(IndexOfBooking);
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
            MessageBoxResult removalOfTable = MessageBox.Show("Will overwrite booking data\nContinue?", "Warning", MessageBoxButton.YesNo);
            if (removalOfTable == MessageBoxResult.Yes)
            {
                OpenFileDialog LoadingFile = new OpenFileDialog();
                LoadingFile.DefaultExt = ".json";
                LoadingFile.Filter = "json files (.json)|*.json";

                var result = LoadingFile.ShowDialog();

                if (result == true)
                {
                    try 
                    {
                        using (FileStream fs = (FileStream)LoadingFile.OpenFile())
                        {
                            List<Table>? BordsBokningar = await JsonSerializer.DeserializeAsync<List<Table>>(fs);
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
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You do not have permission to read file.\nCould not load data", "Unauthorized Access");
                    }
                    catch (ArgumentNullException)
                    {
                        MessageBox.Show("Booking data file is empty", "Failed to load");
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
        public async void SaveExternalFile() //kolla upp mer om den här grejen
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "BookingData" + DateTime.Now.ToString("d");
            saveDialog.Filter = "json Files|*.json";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                List<Table> BordsBokningar = new List<Table>();
                foreach (IBookingObject table in BookableObjects)
                {
                    BordsBokningar.Add((Table)table);
                }
                try
                {
                    using FileStream createStream = File.Create(saveDialog.FileName);
                    await JsonSerializer.SerializeAsync(createStream,
                    BordsBokningar);
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