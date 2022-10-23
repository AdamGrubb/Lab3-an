using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Tables
{
    public class Table : ITable
    {
        public int TableNumber { get; set; }
        public int NumberOfSeats { get; set; }
        public bool WheelChairAccessable { get; set; }
        public bool LocatedOutside { get; set; }
        public List<DateTimeAndGuestStruct> Booking { get; set; }


        public Table(int tableNumber, int numberOfSeats, bool wheelChairAccessable, bool locatedOutside, List<DateTimeAndGuestStruct> booking)
        {
            TableNumber = tableNumber;
            NumberOfSeats = numberOfSeats;
            WheelChairAccessable = wheelChairAccessable;
            LocatedOutside = locatedOutside;
            Booking = booking;
        }
        public Table(int tableNumber, int numberOfSeats, bool wheelChairAccessable, bool locatedOutside)
        {
            TableNumber = tableNumber;
            NumberOfSeats = numberOfSeats;
            WheelChairAccessable = wheelChairAccessable;
            LocatedOutside = locatedOutside;
        }
        public void BookATime(Guest BookingGuest, DateTime BookedTime)
        {
            Booking.Add(new DateTimeAndGuestStruct(BookedTime, BookingGuest));
        }
    }

}
