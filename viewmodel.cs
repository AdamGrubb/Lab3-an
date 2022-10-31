using Lab3.BookingSystem;
using Lab3.Restaurant;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class viewmodel
    {
        public List<IBookingObject> Tables;
        private Restaurants Aztuka = new Restaurants();

        public viewmodel()
        {
            LoadResturant();
        }
        private void LoadResturant()
        {
            Tables = Aztuka.BookableObjects;
        }
        private void LoadTables()
        {

        }
    }
}
