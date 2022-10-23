using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Guests
{
    public class Guest : IGuest
    {
        public string Name { get; }
        public int NumberOfGuests { get; }
        public Guest (string name, int numbersOfGuests)
        {
            Name = name;
            NumberOfGuests = numbersOfGuests;

        }
    }

}
