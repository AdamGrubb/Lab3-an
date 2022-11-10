using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    public struct DateTimeAndGuestStruct
    {
        public DateTime BookedTime { get; set; }
        public Guest BookingGuest { get; set; }
        public DateTimeAndGuestStruct(DateTime bookedTime, Guest bookingGuest)
        {
            BookedTime = bookedTime;
            BookingGuest = bookingGuest;
        }

    }
}
