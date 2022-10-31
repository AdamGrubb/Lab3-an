using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    public struct DateTimeAndGuestStruct //Is the information that is going to be saved to database/textfile with information of the guest and the booked datetime.
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
