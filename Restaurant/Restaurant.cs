using Lab3.Guests;
using Lab3.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Lab3.Restaurant
{
    internal class Restaurants : IRestaurants
    {
        public List<ITable> Tables { get; set; }


        public Restaurants ()
        {
            LoadTables();
        }
        private void LoadTables() //Lägg på ett async här och en delay på 4000 för att testa async funktionen.
        {
            string[] loadTextFromFile;

            string filepath= "Tables.txt";
            //await Task.Delay(4000);
            FileInfo fi = new FileInfo(filepath);
            if (fi.Exists)
            {
                loadTextFromFile = File.ReadAllLines(filepath).ToArray();
                this.Tables = ReadTableFile(loadTextFromFile);
            }
        }
        public string writeOutTable() //Fixa den här metoden, den skriver inte ut något vettigt alls.
        {
            string tempString = "d";
            //    var bookedTables = Tables.Where(table => table.Booking.Count < 0);
            //  foreach (ITable table in bookedTables)
            //    {
            //        t
            //        tempString+=$"Det finns en bokning den: {table.Booking}"
            //    }
            return tempString;
    }
    private List<ITable> ReadTableFile(string[] fileText)
        {
            List<ITable> tables = new List<ITable>();
            Regex tableInfo = new Regex(@"TableNumber=(\d+)\.NumberOfSeats=(\d+)\.WheelChairAccessable=(True|False)\.LocatedOutside=(True|False)\.");
            Regex booking = new Regex(@"Booking=(\w+),(\d),(\d{2}-\d{2}-\d{4} \d{2}:\d{2}).");

            foreach (string Line in fileText)
            {
                List<DateTimeAndGuestStruct> Bookings = importBookings(Line);
                int tableNumber;
                int numberOfSeats;
                bool wheelChairAccessable;
                bool locatedOutside;
                bool ifWorks= Int32.TryParse(tableInfo.Match(Line).Groups[1].Value, out tableNumber);
                if (ifWorks == false) continue;
                ifWorks = Int32.TryParse(tableInfo.Match(Line).Groups[2].Value, out numberOfSeats);
                if (ifWorks == false) continue;
                ifWorks = Boolean.TryParse(tableInfo.Match(Line).Groups[3].Value, out wheelChairAccessable);
                if (ifWorks == false) continue;
                ifWorks = Boolean.TryParse(tableInfo.Match(Line).Groups[4].Value, out locatedOutside);
                if (ifWorks == false) continue;

                if (Bookings.Count < 1) tables.Add(new Table(tableNumber, numberOfSeats, wheelChairAccessable, locatedOutside));
                else tables.Add(new Table(tableNumber, numberOfSeats, wheelChairAccessable, locatedOutside, Bookings));
            }
            return tables;


        }
       
        public async void WriteTableFile(List<ITable> fileText) //Gör asynk här också så att det kan ladda från en plats istället.
        {
            string filepath = "Tables.txt";
            List<string> SaveToFile = new List<string>();
            foreach (ITable table in fileText)
            {
                if (table.Booking ==null)
                {
                    SaveToFile.Add($"TableNumber={table.TableNumber}.NumberOfSeats={table.NumberOfSeats}.WheelChairAccessable={table.WheelChairAccessable}.LocatedOutside={table.LocatedOutside}.");
                }
                else
                {
                    string bookings = ConvertBookingToString(table);
                    SaveToFile.Add($"TableNumber={table.TableNumber}.NumberOfSeats={table.NumberOfSeats}.WheelChairAccessable={table.WheelChairAccessable}.LocatedOutside={table.LocatedOutside}.{bookings}.");
                }
                    
                
            }
            File.WriteAllLines(filepath, SaveToFile); 

        }
        private string ConvertBookingToString(ITable table)
        {
            string Bookings = "";
            foreach (DateTimeAndGuestStruct booking in table.Booking)
            {
               Bookings +=$"Booking={booking.BookingGuest.Name},{booking.BookingGuest.NumberOfGuests},{booking.BookedTime.ToString("MM/dd/yyyy h:mm")}";
            }
            return Bookings;
        }
        private List<DateTimeAndGuestStruct> importBookings(string fileText) //Den här lägger in bokninga på alla bord. Gör om den till string.
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
    }
}
