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
        public int NumbersOfGuests { get; set; }
        public string? Comments { get; set; }
        public Guest (string name, int numbersOfGuests, string comments)
        {
            Name = name;
            NumbersOfGuests = numbersOfGuests;
            Comments = comments;
        }
    }

}
