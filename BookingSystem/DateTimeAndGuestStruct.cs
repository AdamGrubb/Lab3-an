using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    public class DateTimeAndGuestStruct //Se om du smärtfritt kan byta om den till DateTimeAndGuest - Eller om du smärtfritt kan göra om den till struct igen.
    {
        public DateTime BookedTime { get; set; }
        public Guest BookingGuest { get; set; }
        public DateTimeAndGuestStruct(DateTime bookedTime, Guest bookingGuest)
        {
            BookedTime = bookedTime;
            BookingGuest = bookingGuest;
        }
        public DateTimeAndGuestStruct()
        { }
    }
}
