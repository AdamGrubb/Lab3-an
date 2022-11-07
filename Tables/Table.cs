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
        public List<DateTimeAndGuestStruct> Booking { get; set; }

        public Table(string nameID, int maxNumberOfGuests, List<DateTimeAndGuestStruct> booking)
        {
            NameID = nameID;
            MaxNumberOfGuests = maxNumberOfGuests;
            Booking = booking;
        }
    }

}
