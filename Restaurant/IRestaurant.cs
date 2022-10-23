using Lab3.Guests;
using Lab3.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Restaurant
{
    internal interface IRestaurants
    {
        List<ITable> Tables { get; set; }

        string writeOutTable();

    }
}
