using Lab3.Guests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.BookingSystem
{
    public interface IBookingSystem 
        //Bokningsystem som ska kunna implementeras i en rad olika situationer, Biograf, Bowlingbana, hotelrum, bordsbokning osv.
        //Frågan är om man här ska bara lägga upp de listor man vill ha tillgängliga. Eller göra om det här till en abstract?
    {
        List<IBookingObject> BookableObjects { get; set; } //För det är väl bäst ifall den här ej är publik. Så att View bara kan ha tillgång till Observable collections.
        //string[] LoadedFileText { get; set; }
        string[] LoadBookingObjectFile(); //Imports raw textinformation about the state of BookingObjectFiles. 
        List<IBookingObject> ExtractBookingObjectFields(string[] fileText); //Method to extract wanted information from LoadedFileText-array and returnning a list to BookableObjects.

        void WriteBookingObjectFile();
         
    }
}
