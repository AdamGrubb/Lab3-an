using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Guests
{
    internal interface IGuest
    {
        string Name { get; }
        int NumberOfGuests { get; }

    }
}
