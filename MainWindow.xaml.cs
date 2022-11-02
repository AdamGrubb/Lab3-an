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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _numberOfPerson = 1;
        public int NumberOfPerson
        {
            get
            {
                return _numberOfPerson;
            }
            set
            {
                _numberOfPerson = value;
                OnPropertyChanged(nameof(NumberOfPerson));
            }
        }
        private Restaurants SnuskigaFisken = new Restaurants();
        List<TimeSpan> Availabletimes = new List<TimeSpan> { { new TimeSpan ( 16, 00, 00 ) },{ new TimeSpan(17, 00, 00) }, { new TimeSpan(17, 30, 00) },{ new TimeSpan(18, 00, 00) },{ new TimeSpan(18, 30, 00) } }; //Helt enkelt ändra till List<string>.
        public event PropertyChangedEventHandler? PropertyChanged;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            TimePicker.ItemsSource = Availabletimes;
            ChooseTable.ItemsSource = SnuskigaFisken.ListTableID; //Selectchange på Calender, Tid Eller Guest ska uppdatera BordsListan till de bord som är lediga under de förutsättningarna.
            fillTestField();
        }





        //public List<string> GetTimeIntervals()
        //{
        //    List<string> timeIntervals = new List<string>();
        //    TimeSpan startTime = new TimeSpan(0, 0, 0);
        //    DateTime startDate = new DateTime(DateTime.MinValue.Ticks); // Date to be used to get shortTime format.
        //    for (int i = 0; i < 48; i++)
        //    {
        //        int minutesToBeAdded = 30 * i;      // Increasing minutes by 30 minutes interval
        //        TimeSpan timeToBeAdded = new TimeSpan(0, minutesToBeAdded, 0);
        //        TimeSpan t = startTime.Add(timeToBeAdded);
        //        DateTime result = startDate + t;
        //        timeIntervals.Add(result.ToShortTimeString());      // Use Date.ToShortTimeString() method to get the desired format                
        //    }
        //    return timeIntervals;
        //}




        //--------------------------------
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SubtractGuest_Click(object sender, RoutedEventArgs e)
        {
            if (NumberOfPerson > 1) NumberOfPerson--;
        }

        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (NumberOfPerson < 8) NumberOfPerson++;
            else
            {
                MessageBox.Show("For bookings over 8 person, make custom booking.", "Big Booking"); //Om jag orkar göra en custom booking-grej. Antagligen inte. Annars tabort.
            }
        }

        private void BookingButton_Click(object sender, RoutedEventArgs e) //Gör en röd border på det som behöver fixas och skicka upp en messagebox.
        {
            if (UserInputNullCheck())
            {
                int guestNumbers = NumberOfPerson;
                DateTime dateAndTime = ChosenDate.SelectedDate.Value;
                dateAndTime= dateAndTime.Add((TimeSpan)TimePicker.SelectedValue); //Kolla om det här fungerar.
                string nameOfGuest = userNameInput.Text;
                int tableID = ChooseTable.SelectedIndex;

                if (ExtraInfoBox.Text != string.Empty)
                {
                    string ExtraInfo = ExtraInfoBox.Text;
                    SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, ExtraInfo, tableID); //AddBooking(DateTime Chosentime, string guestName, int numberOfGuests, string comment)
                }
                else SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, tableID); //(DateTime Chosentime, string GuestName, int numberOfGuests)
            }
            else MessageBox.Show("Du har inte fyllt i alla fält idiot.");

        }

        private bool UserInputNullCheck()
        {
            if (ChooseTable.SelectedValue ==null || userNameInput.Text == null || ChosenDate.SelectedDate == null || TimePicker.SelectedValue == null)
            {
               return false;
            }
            else return true;
        }
        private void fillTestField()
        {
            //List<List<ObservableCollection<string>>>
            //HärSkadetTestas
            foreach (List<ObservableCollection<string>> Test in SnuskigaFisken.SeperatedTables)
            {
                foreach (ObservableCollection<string> TestLager2 in Test)
                {
                    foreach (string TestLager3 in TestLager2)
                    {
                        HärSkadetTestas.Text += TestLager3+"\n";
                    }
                }
            }

        }

        //private void CreateTimeSpans() //Här kan du skapa vilka tider du vill ha möjliga att välja bland. Samt sätta itemsource på comboboxen
        //{
        //    for (int i = 0; i < 10; i++)
        //    {

        //    }
        //}
    }
}