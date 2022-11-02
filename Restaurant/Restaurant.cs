using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Lab3.Restaurant
{
    internal class Restaurants : IBookingSystem //Kanske lägga alla Lists här som observable collections? Så att alla Listor, Fixa en export och Load knapp som gör att man kan ladda bokningar och sedan skriver över default-filen.
    {
        public List<IBookingObject> BookableObjects { get; set; }
        public List<List<ObservableCollection<string>>> SeperatedTables { get; set; } //Här har du en List där varje List<Dictionary<string, string>> motsvarar ett table. Där Key märks med bokningarna och Value blir bordets märkning.

        public List<string> ListTableID = new List<string>();
        private string[] LoadedFileText { get; set; }
        public Restaurants()
        {
            LoadedFileText = LoadBookingObjectFile();
            //if (!LoadedFileText == null); //Vad händer då? Skapa en felruta i programmet som säger att det inte kunde laddas. 
            BookableObjects = ExtractBookingObjectFields(LoadedFileText);
            FillSeperatedTables();
            FillTableID();

        }
        public string[] LoadBookingObjectFile() //Hur ska jag lösa det här? En varning kan ploppa upp att det inte existerar några bord, ge möjlighet att skapa? Antagligen inte. En try-catch här. Göra denna till async?
        {
            string[] loadTextFromFile;
            string filepath = "Tables.txt";
            //await Task.Delay(4000);
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                return loadTextFromFile = File.ReadAllLines(filepath).ToArray();
            }
            else
            {
                fi.Create();
                return null;
            }


        }
        private void FillTableID()
        {
            ListTableID = BookableObjects.Select(tableID => tableID.NameID).ToList();
        }
        private void FillSeperatedTables() //Den här infon kan användas för att göra en Lista över borden. 
        {
            List<List<ObservableCollection<string>>> listOfTables = new List<List<ObservableCollection<string>>>();

            foreach (IBookingObject table in BookableObjects)
            {

                listOfTables.Add(new List<ObservableCollection<string>>() { });
            }
            for (int i = 0; i < BookableObjects.Count; i++)
            {
                var vadÄr = BookableObjects[i].Booking.Select(d => $"{d.BookingGuest.Name} has booked {d.BookedTime.ToString("f")}").ToList();

                listOfTables[i].Add(new ObservableCollection<string>(BookableObjects[i].Booking.Select(d => $"{d.BookingGuest.Name} has booked {d.BookedTime.ToString("f")}").ToList()));
            }
            SeperatedTables = listOfTables;
        }

        public List<IBookingObject> ExtractBookingObjectFields(string[] fileText) //Här har du ett eventuellt fel om det inte blir nån matchning. Gör en If Match>0 {} sen foreachen, annars return null.
        {
            List<IBookingObject> tables = new List<IBookingObject>();
            Regex tableInfo = new Regex(@"TableNumber=(\d+)\.NumberOfSeats=(\d+)\.WheelChairAccessable=(True|False)\.");
            Regex booking = new Regex(@"Booking=(\w+),(\d),(\d{2}-\d{2}-\d{4} \d{2}:\d{2})."); //Återanvända den här??

            foreach (string Line in fileText)
            {
                List<DateTimeAndGuestStruct> Bookings = ImportBookings(Line);
                string tableNumber = tableInfo.Match(Line).Groups[1].Value;
                int numberOfSeats;
                bool wheelChairAccessable;
                bool locatedOutside; //Här har vi ju ett nytt fält. Ska det vara kvar??
                bool ifWorks = Int32.TryParse(tableInfo.Match(Line).Groups[2].Value, out numberOfSeats);
                if (ifWorks == false) continue;
                ifWorks = Boolean.TryParse(tableInfo.Match(Line).Groups[3].Value, out wheelChairAccessable);
                if (ifWorks == false) continue;

                if (Bookings.Count < 1) tables.Add(new Table(tableNumber, numberOfSeats, wheelChairAccessable)); //Här har vi ju ett nytt fält. Ska det vara kvar??
                else tables.Add(new Table(tableNumber, numberOfSeats, wheelChairAccessable, Bookings));
            }
            return tables;


        }

        /*  //Skapa fil
            string fileName = "WeatherForecast.json";

            //Skapa en filström
            using FileStream createStream = File.Create(fileName);

            // Serialisera datan asynkront
            await JsonSerializer.SerializeAsync(createStream,
            weatherForecast);

            // Skriv datan asynkront
            await createStream.DisposeAsync();
            Console.WriteLine(File.ReadAllText(fileName));
        */
        private List<DateTimeAndGuestStruct> ImportBookings(string fileText) //Den här lägger in bokninga på alla bord. Gör om den till string.
        {
            Regex booking = new Regex(@"Booking=(\w+),(\d),(\d{2}-\d{2}-\d{4} \d{2}:\d{2}).");

            List<DateTimeAndGuestStruct> DateAndTime = new List<DateTimeAndGuestStruct>() { };

            var result = booking.Matches(fileText).ToList();

            foreach (Match match in result)
            {
                DateTime tempDateD;
                int tempPersons;
                string name = match.Groups[1].Value;
                bool successfullParse = DateTime.TryParse(match.Groups[3].Value, out tempDateD);
                successfullParse = Int32.TryParse(match.Groups[2].Value, out tempPersons);

                if (successfullParse)
                {
                    DateAndTime.Add(new DateTimeAndGuestStruct(tempDateD, new Guest(name, tempPersons)));
                }


            }
            return DateAndTime;
        }

        public void WriteBookingObjectFile() //Gör asynk här också så att det kan ladda från en plats istället.
        {
            string filepath = "Tables.txt";
            List<string> SaveToFile = new List<string>();
            foreach (IBookingObject table in BookableObjects)
            {
                if (table.Booking == null)
                {
                    SaveToFile.Add($"TableNumber={table.NameID}.NumberOfSeats={table.MaxNumberOfGuests}.WheelChairAccessable={table.WheelChairAccessable}.");
                }
                else
                {
                    string bookings = table.Booking.Aggregate("", (current, s) => current + ($"Booking={s.BookingGuest.Name},{s.BookingGuest.NumberOfGuests},{s.BookedTime.ToString("MM/dd/yyyy H:mm")}."));
                    SaveToFile.Add($"TableNumber={table.NameID}.NumberOfSeats={table.MaxNumberOfGuests}.WheelChairAccessable={table.WheelChairAccessable}.{bookings}");
                }


            }
            File.WriteAllLines(filepath, SaveToFile);

        }
        public void AddBooking(DateTime Chosentime, string GuestName, int numberOfGuests, int tableID) //(DateTime bookedTime, Guest bookingGuest (string name, int numbersOfGuests))
        {
            BookableObjects[tableID].Booking.Add(new DateTimeAndGuestStruct (Chosentime,new Guest(GuestName, numberOfGuests)));
            MessageBox.Show("Added Booking");
            WriteBookingObjectFile();
        }
        public void AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment, int tableID) //Om man väljer att lägga in en kommentar. Här är det ju inga problem att lägga till kommentar efter json serialisering.
        {
            MessageBox.Show("Added Booking, with comment");
        }
        public void RemoveBooking(int IndexOfTable, int IndexOfBooking)
        {
            BookableObjects[IndexOfTable].Booking.RemoveAt(IndexOfBooking);
        }
    }
}
