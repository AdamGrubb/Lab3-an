using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Tables
{
    internal interface ITable
    {
        int TableNumber { get; set; }
        int NumberOfSeats { get; set; }
        bool WheelChairAccessable { get; set; }
        bool LocatedOutside { get; set; }
        public List<DateTimeAndGuestStruct> Booking { get; set; }
    }
}
