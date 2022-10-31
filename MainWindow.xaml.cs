using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab3.BookingSystem;
using Lab3.Guests;
using Lab3.Restaurant;
using Lab3.Tables;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectionChangedEventArgs chosen = null;
        public List<string> tableOneBookings { get; set; }
        //public List<DateTimeAndGuestStruct> Table1 = new List<DateTimeAndGuestStruct>();

        ListBox EttBordTider;
        Restaurants Azteka = new Restaurants();
        public MainWindow()
        {
            InitializeComponent();
            Restaurants Azteka = new Restaurants();
            viewmodel Aztuka = new viewmodel();
            //var Tabbles = Azteka.BookableObjects.Select(tabless=>tabless.

            //StartListen.DisplayMemberPath = Table1.BookedTime;
            //GenerateTableBookings();
            GenerateDatesAnd();
        }
        public void GenerateBookings()
        {
            tableOneBookings = Azteka.BookableObjects.SelectMany(table => table.Booking).Select(booking => booking.BookedTime).Select(date => "Booked Times:" + date.ToString("HH:mm")).ToList();
        }

        private void GenerateDatesAnd()
        {
            List<List<string>> SamlaDettaILista = new List<List<string>>();
            List<string> tableName = new List<string>();
            int tableNamezz = 0;

            foreach (IBookingObject table in Azteka.BookableObjects)
            {
                SamlaDettaILista.Add(table.Booking.Select(x=> $"Guest: {x.BookingGuest.Name}.  Booked Time: {x.BookedTime.ToString("HH:mm")}.").ToList());
                tableName.Add(table.NameID);
            }

            //------------------------------------------------------------------------------
            //List<List<string>> allaBokadeTider=new List<List<string>>();
            //foreach (List<string> tableBookings in SamlaDettaILista)
            //{
            //    placeForAllBookings.Children.Add(new Label()
            //    {
            //     Background = Brushes.LightGray,
            //     Content = tableName[tableNamezz],
            //     //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
            //     BorderBrush = Brushes.LightSlateGray,
            //     BorderThickness = new Thickness(5),
            //     Margin=new Thickness(0,10,0,0)
            //    });
            //    placeForAllBookings.Children.Add(new ListBox() {ItemsSource=tableBookings, Margin=new Thickness(0,0,0,10), Background = Brushes.LightYellow });
            //    tableNamezz++;
            //}
            //------------------------------------------------------------------
            List<Label> LabelList = new List<Label>();
            List<ListBox> ListboxList = new List<ListBox>();

            foreach (List<string> tableBookings in SamlaDettaILista)
            {
                placeForAllBookings.Children.Add(new StackPanel());
                LabelList.Add(new Label()
                {
                    Background = Brushes.LightGray,
                    Content = tableName[tableNamezz],
                    //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
                    BorderBrush = Brushes.LightSlateGray,
                    BorderThickness = new Thickness(5)
                });
                ListboxList.Add(new ListBox() { ItemsSource = tableBookings, Background = Brushes.LightYellow, Margin = new Thickness(0, 0, 0, 20), });
                tableNamezz++;
            }
            int test = 0;
            foreach (StackPanel tablePresentation in placeForAllBookings.Children)
            {
                tablePresentation.Children.Add(LabelList[test]);
                tablePresentation.Children.Add(ListboxList[test]);
                test++;
            }
            //---------------------------------------------------------------------



            //StartListen1.ItemsSource = Tempirrär1;
            //StartListen2.ItemsSource = Tempirrär2;
            //StartListen3.ItemsSource = Tempirrär3;  
            //StartListen4.ItemsSource = Tempirrär4;
            //StartListen5.ItemsSource = Tempirrär5;
            //StartListen6.ItemsSource = Tempirrär6;
            //StartListen7.ItemsSource = Tempirrär7;
        }

        private void whichOneIsSelected(object sender, SelectionChangedEventArgs e)
        {
            e.
        }

        private void deleteBooking_Click(object sender, RoutedEventArgs e, SelectionChangedEventArgs chosen)
        {
            chosen.
            Azteka.RemoveBooking
        }


        //private void GenerateTableBookings() //Den här genererar en översyn på vilka bokningar vilka bord har. Går det att fixa en delete därifrån? Antagligen inte. Kan man få ut index härifrån?? Ja via regex, testa!
        //{

        //    List<List<Dictionary<string, string>>> SecondTableList = new List<List<Dictionary<string, string>>>(); //Jag kanske ska ha en Dictonary här?? där Nyckeln är index för vilket bord det är och Value är en List<string>. Går det då att få ut nyckeln?
        //    int påFyllning = 0;
        //    StackPanel BordsBokning = new StackPanel();

        //    foreach (IBookingObject table in Azteka.BookableObjects)
        //    {
        //        SecondTableList.Add(new List<Dictionary<string, string>>());
        //        {
        //            for (int x = 0; x < table.Booking.Count; x++)
        //            {

        //                new Dictionary<string, string>() { { table.NameID, $"{table.Booking[x].BookingGuest.Name} has booked {table.Booking[x].BookedTime.ToString("F")}" } };
        //            }
        //        }
        //    }
        //    foreach (List<Dictionary<string, string>> table in SecondTableList)
        //    {

        //            table.Add(new Dictionary<string, string> () { { SecondTableList[påFyllning], $"{table.Booking[x].BookingGuest.Name} has booked {table.Booking[x].BookedTime.ToString("F")}" } });
        //        }
        //    påFyllning++;
        //    }
        //    foreach (List<Dictionary<string, string>> bookings in SecondTableList)
        //    {
        //        placeForAllBookings.Children.Add(new StackPanel());
        //    }

        //    foreach (StackPanel stackpanelll in placeForAllBookings.Children)
        //    {
        //        int y = 0;
        //        stackpanelll.Children.Add(new Label()
        //        {
        //            Background = Brushes.LightGray,
        //            //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
        //            BorderBrush = Brushes.LightSlateGray,
        //            BorderThickness = new Thickness(5)

        //        });

        //        stackpanelll.Children.Add(new ListBox()
        //        {
        //            ItemsSource = SecondTableList[y]
        //            //DisplayMemberPath= Hur får man dit att den ska använda nyckeln=??
        //        }

        //            );

        //    }
        //}


        //foreach (StackPanel stackpanelll in placeForAllBookings.Children)
        //                    {
        //                        stackpanelll.Children.Add(new Label()
        //                        {
        //                            Background = Brushes.LightGray,
        //                            Content = "TablePlaceholder",
        //                            //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
        //                            BorderBrush = Brushes.LightSlateGray,
        //                            BorderThickness = new Thickness(5)

        //                        });


        //        foreach (IBookingObject table in Azteka.BookableObjects)
        //{
        //    SecondTableList.Add(new List<Dictionary<string, string>>()
        //{

        //                foreach (DateTimeAndGuestStruct date in table.Booking)
        //            {
        //                new Dictionary<string, string>()
        //                                        {
        //                                            {table.NameID,date.BookedTime.ToString("f") }
        //                                        };
        //            }
        //        }




        //    }
        //};
    }
}

//        }      //table.Booking.Select(date => $"{date.BookingGuest.Name} has booked {date.BookedTime.ToString("f")}")

//            foreach (List<Dictionary<string, string>> table in SecondTableList)
//                    {
//                        table.Add()
//}
//                    foreach (List<Dictionary<string, string>> bookings in SecondTableList)
//                    {
//                        placeForAllBookings.Children.Add(new StackPanel());
//                    }
//                    foreach (StackPanel stackpanelll in placeForAllBookings.Children)
//                    {
//                        stackpanelll.Children.Add(new Label()
//                        {
//                            Background = Brushes.LightGray,
//                            Content = "TablePlaceholder",
//                            //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
//                            BorderBrush = Brushes.LightSlateGray,
//                            BorderThickness = new Thickness(5)

//                        });
//                        foreach (List<Dictionary<string, string>> bookings in SecondTableList)
//                        {
//                            stackpanelll.Children.Add(new Label()
//                            {
//                                Background = Brushes.LightGray,
//                                Content = bookings.ToString(),
//                                //rubrik.Content = "Table: " + table.NameID; //Den här kan du ju köra efteråt nu har du ju strukturen. Kanske en linq?
//                                BorderBrush = Brushes.LightSlateGray,
//                                BorderThickness = new Thickness(5)
//                            });
//                            stackpanelll.Children.Add(new ListBox() { ItemsSource = bookings });
//                        }
//                    }
//                }
            

//                    //List<string> SecondTableList = table.Booking.OrderBy(x=>x.BookedTime).Select(date => date.BookingGuest.Name+ " has booked: " + date.BookedTime).ToList();
//                    //List<string> SecondTableList = table.Booking.Select(date => date.BookingGuest.Name + " has booked: " + date.BookedTime).ToList();



//                    //private void deleteBooking_Click(object sender, RoutedEventArgs e)
//                    //{
//                    //    List<string> vadÄrDetta = new List<string>();
//                    //    vadÄrDetta.Add(EttBordTider.SelectedValue.ToString());
//                    //    vadÄrDetta.Add(EttBordTider.SelectedValuePath.ToString());
//                    //    vadÄrDetta.Add(EttBordTider.SelectedIndex.ToString());
//                    //    vadÄrDetta.Add(EttBordTider.SelectedItem.ToString());
//                    //    displayResults.ItemsSource = vadÄrDetta;


//                    //}

//                    //private void DeleteButton_Click(object sender, RoutedEventArgs e) //Göra en select
//                    //{
//                    //    if (StartListen1.SelectedIndex != -1) Azteka.BookableObjects[0].Booking.RemoveAt(StartListen1.SelectedIndex);
//                    //    else if(StartListen2.SelectedIndex != -1) Azteka.BookableObjects[1].Booking.RemoveAt(StartListen2.SelectedIndex);
//                    //    else if (StartListen3.SelectedIndex != -1) Azteka.BookableObjects[2].Booking.RemoveAt(StartListen3.SelectedIndex);
//                    //    else if (StartListen4.SelectedIndex != -1) Azteka.BookableObjects[3].Booking.RemoveAt(StartListen4.SelectedIndex);
//                    //    else if (StartListen5.SelectedIndex != -1) Azteka.BookableObjects[4].Booking.RemoveAt(StartListen5.SelectedIndex);
//                    //    else if (StartListen6.SelectedIndex != -1) Azteka.BookableObjects[5].Booking.RemoveAt(StartListen6.SelectedIndex);
//                    //    else if (StartListen7.SelectedIndex != -1) Azteka.BookableObjects[6].Booking.RemoveAt(StartListen7.SelectedIndex);
//                    //    GenerateDatesAnd();
//                    //    Azteka.WriteBookingObjectFile();
//                    //}



//                    //private List<DateTimeAndGuestStruct> CheckIfTimeIsFree(DateTime AvailableDate)
//                    //{
//                    //    /*
//                    //     * Kolla här hur man gör för att aggregate när man har ett bool-statement. Kolla också hur det fungerar att jämföra två DateTime-objects.
//                    //     * Problemet nu är inte bara att man ska jämföra date time objekt utan också att du ska jämföra en timespann. Ta t ex en bokad tid och 2 timmar framåt i jämförelse.
//                    //     * Kan vara så att du måste öppna upp Aggregate så att den jämför before, i och after.
//                    //     * Du kan ju också lägga in alla andra olika jämförelser, typ handikapp, platser osv osv
//                    //     */

//                    //    //Foreacha varje bojekt i listan, och där gör du ett urval om just det läggs till i listan! Får man då rätt index i Listbox??

//                    //    foreach (IBookingObject in )

//                    //    var freeTables = Azteka.BookableObjects.Select(table => table.Booking).Select(booking => booking.); 

//                    //        foreach (IBookingObject table in Azteka.BookableObjects)
//                    //        {
//                    //            foreach (DateTimeAndGuestStruct booking in table.Booking)
//                    //            {
//                    //                if (booking.BookedTime.Equals(AvailableDate)==false)
//                    //                {
//                    //                ListBox16.ItemsSource.Cast //Hur lägger man till något till listbox
//                    //                } 
//                    //                //Eller så har jag Restaurant som har olika funktioner för att ge ifrån sig listor.
//                    //                // Samt även en metod för att lägga till en bokning eller ta bort en bokning.



//                    //            }
//                    //        }

//                    //    //.SelectMany(x => x.Booking).Aggregate(0, (AMatch, datum) => datum.BookedTime.Equals(AvailableDate) ? AMatch + 1 : AMatch); ; //Problemet är ifall det ite finns ett datum, då blir det null. Sortera sort nullarna.

//                    //    //return freeTables; //Azteka.BookableObjects.Count() - occupiedTables;


//                    //}
//                }
//            }
//        }