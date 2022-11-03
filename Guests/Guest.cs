using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Guests
{
    public class Guest : IGuest
    {
        public string Name { get; set; }
        public int NumberOfGuests { get; set; }
        public string Comments { get; set; } //Om kommentaren är null så skapas en vanligt Guest. Om det finns en kommentar kan ju den presenteras i listan med en rödmarkerad plutt.
        public Guest (string name, int numbersOfGuests, string comments)
        {
            Name = name;
            NumberOfGuests = numbersOfGuests;
            Comments = comments;

        }
        public Guest(string name, int numbersOfGuests)
        {
            Name = name;
            NumberOfGuests = numbersOfGuests;

        }
        public Guest()
        {

        }
    }

}
