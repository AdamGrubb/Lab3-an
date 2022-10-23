using Lab3.BookingSystem;
using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Tables
{
    public class Table : IBookingObject
    {
        public string NameID { get; set; }
        public int MaxNumberOfGuests { get; set; }
        public bool WheelChairAccessable { get; set; }
        public bool LocatedOutside { get; set; } //Här har vi ju ett nytt fält. Ska det vara kvar??
        public List<DateTimeAndGuestStruct> Booking { get; set; }


        public Table(string tableNumber, int numberOfSeats, bool wheelChairAccessable, bool locatedOutside, List<DateTimeAndGuestStruct> booking)
        {
            NameID = tableNumber;
            MaxNumberOfGuests = numberOfSeats;
            WheelChairAccessable = wheelChairAccessable;
            LocatedOutside = locatedOutside;//Här har vi ju ett nytt fält. Ska det vara kvar??
            Booking = booking;
        }
        public Table(string tableNumber, int numberOfSeats, bool wheelChairAccessable, bool locatedOutside)
        {
            NameID = tableNumber;
            MaxNumberOfGuests = numberOfSeats;
            WheelChairAccessable = wheelChairAccessable;
            LocatedOutside = locatedOutside;//Här har vi ju ett nytt fält. Ska det vara kvar??
        }
        public void BookATime(Guest BookingGuest, DateTime BookedTime)
        {
            Booking.Add(new DateTimeAndGuestStruct(BookedTime, BookingGuest));
        }
    }

}
