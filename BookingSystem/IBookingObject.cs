using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    internal interface IBookingObject //Kan vara bord på restaurang, hotellrum, bowlingbana osv osv. 
    {
        string NameID { get; set; }
        int MaxNumberOfGuests { get; set; }
        bool WheelChairAccessable { get; set; }
        public List<DateTimeAndGuestStruct> Booking { get; set; } //Struct 
    }
}
