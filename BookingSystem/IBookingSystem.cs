using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    public interface IBookingSystem 
   
    {
        List<IBookingObject> BookableObjects { get; set; }
        void SaveToFile();
        Task LoadFromFile();
        Task OpenExternalFile();
        void SaveExternalFile();

    }
}
