using System;
using System.CodeDom.Compiler;
using System.Collections;
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
        Restaurants SnuskigaFisken;
        private int _numberOfPerson = 1;
        private bool _listBoxView;
        
        List<TimeSpan> Availabletimes = new List<TimeSpan> { { new TimeSpan ( 16, 00, 00 ) },{ new TimeSpan(17, 00, 00) }, { new TimeSpan(17, 30, 00) },{ new TimeSpan(18, 00, 00) },{ new TimeSpan(18, 30, 00) } }; //Helt enkelt ändra till List<string>.
        public MainWindow()
        {
            SnuskigaFisken = new Restaurants();
            InitializeComponent();
            DataContext = SnuskigaFisken;
            TimePicker.ItemsSource = Availabletimes; //Denna kanske borde läggas mot att uppdatera alla.
            NumberOfPersons.Text = _numberOfPerson.ToString(); //Behöver denna vara ToString?
            
            //Selectchange på Calender, Tid Eller Guest ska uppdatera BordsListan till de bord som är lediga under de förutsättningarna.
        }

        private void SubtractGuest_Click(object sender, RoutedEventArgs e)
        {
            if (_numberOfPerson > 1)
            {
                _numberOfPerson--;
                NumberOfPersons.Text = _numberOfPerson.ToString();
            }

        }

        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (_numberOfPerson < 8)
            {
                _numberOfPerson++;
                NumberOfPersons.Text = _numberOfPerson.ToString();
            }

            else
            {
                MessageBox.Show("For bookings over 8 person, make custom booking.", "Big Booking"); //Om jag orkar göra en custom booking-grej. Antagligen inte. Annars tabort.
            }
        }

        private void BookingButton_Click(object sender, RoutedEventArgs e) //Gör en röd border på det som behöver fixas och skicka upp en messagebox.
        {
            if (UserInputNullCheck())
            {
                int guestNumbers = _numberOfPerson;
                DateTime dateAndTime = ChosenDate.SelectedDate.Value;
                dateAndTime= dateAndTime.Add((TimeSpan)TimePicker.SelectedValue);
                string nameOfGuest = userNameInput.Text;
                int tableID = ChooseTable.SelectedIndex;

                if (ExtraInfoBox.Text != string.Empty) //Frågan är ifall jag bara ska använda en Construktor och låta Commentaren vara null ifall ingen kommentar finns.
                {
                    string ExtraInfo = ExtraInfoBox.Text;
                    SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, ExtraInfo, tableID);
                }
                else SnuskigaFisken.AddBooking(dateAndTime, nameOfGuest, guestNumbers, tableID);
                
                UpdateListBoxes();
                SnuskigaFisken.FillDisplayAllBookings();
                if (_listBoxView == false)
                {
                    DisplayAllBookings();
                }
                else if (_listBoxView == true)
                {
                    GenerateListBoxes();
                }
            }
            else MessageBox.Show("Du har inte fyllt i alla fält");


        }

        private bool UserInputNullCheck()
        {
            if (ChooseTable.SelectedValue ==null || userNameInput.Text == null || ChosenDate.SelectedDate == null || TimePicker.SelectedValue == null)
            {
               return false;
            }
            else return true;
        }

        private void AddTableButton_Click(object sender, RoutedEventArgs e)
        {
            SnuskigaFisken.AddTable(tableName.Text);
        }

        private void GenerateListBoxes()
        {
            int tableIndex = 0; //Ta bort denna?? Förstår inte vart den leder.
            placeForAllBookings.Children.Clear();
            foreach (List<string> tables in SnuskigaFisken.SeperatedTables) //List<List<ObservableCollection<string>>> SeperatedTables
            {
                placeForAllBookings.Children.Add(new StackPanel());

                tableIndex++;
            }
            int by = 0;
            foreach (StackPanel listContainers in placeForAllBookings.Children)
            {
                //Klämma in en If SnuskigaFisken.SeperatedTables[by] is null just addListbox. Då skulle du kunna generera den när som helst. Skulle istället kunna lägga in en "No tables loaded".
                listContainers.Children.Add(new Label() { FontSize = 20, HorizontalAlignment=HorizontalAlignment.Center, Width=250, Content = SnuskigaFisken.ListTableID[by], Background = Brushes.DimGray }); //Kommer denna ge index out of bounds?
 
                listContainers.Children.Add(new ListBox() { ItemsSource = SnuskigaFisken.SeperatedTables[by], Width = 250, Background = Brushes.LightYellow });
                by++;
            }

        }
        private void UpdateListBoxes() //Stoppa in denna i GenereatListBoxes. Och döp om den så dispalyAllBookings och GenreateListboxes låter likandne.
        {
            DateTime? bokadTider = ChosenDate.SelectedDate;
            if (bokadTider != null)
            {
                SnuskigaFisken.FillSeperatedTables((DateTime)bokadTider); //Gör denna privat igen och 
            }
        }
        private void DisplayAllBookings()
        {
            placeForAllBookings.Children.Clear();
            SnuskigaFisken.FillDisplayAllBookings();
            placeForAllBookings.Children.Add(new ListBox() { ItemsSource = SnuskigaFisken.DisplayAllBookings, DisplayMemberPath = "Key", SelectedValuePath="Value" });
        }
        private void deleteBooking_Click(object sender, RoutedEventArgs e)
        { //Varför är det dubbla DisplayAllBookins och Generate ListBoxes??

            if (_listBoxView==false)
            {
                DeleteBookingFromDisplayAll();
                DisplayAllBookings();
            }
            else if (_listBoxView == true) 
            {
                DeleteFromListBoxes();
                GenerateListBoxes();
            }

            /*
             *Hela den här uppdateringsgrejen måste göras om som en metod. Blir för mycket kod igen. 
             * 
             */
            UpdateListBoxes();
            SnuskigaFisken.FillDisplayAllBookings();
            if (_listBoxView == false)
            {
                DisplayAllBookings();
            }
            else if (_listBoxView == true)
            {
                GenerateListBoxes();
            }
        }
        private void DeleteFromListBoxes()
        {
            int indexOfList = 0;
            foreach (StackPanel listContainer in placeForAllBookings.Children)
            {
                foreach (ListBox tabelBookings in listContainer.Children.OfType<ListBox>())
                {
                    if (tabelBookings.SelectedValue != null)
                    {
                        SnuskigaFisken.RemoveBooking(indexOfList, tabelBookings.SelectedIndex);
                    }
                    tabelBookings.UnselectAll();
                    indexOfList++;
                }
            }
        }
        private void DeleteBookingFromDisplayAll()
        {
            int[] tableIndex = new int[] {};
            foreach (ListBox displayBookings in placeForAllBookings.Children)
            {
                    tableIndex = (int[])displayBookings.SelectedValue;
            }
            if (tableIndex != null)
            {
                SnuskigaFisken.RemoveBooking(tableIndex[0], tableIndex[1]); //Här får jag ett exeption i och med att ListBox-är inställd på -1 som default. Lösningen vore att göra ListBox.Select = null. 
            }
        }
        private void ChangeView_Click(object sender, RoutedEventArgs e)
        {
            if (ChosenDate.SelectedDate != null)
            {
                UpdateListBoxes();
                GenerateListBoxes();
                _listBoxView = true;
            }
                
        }
        private void ShowAllBookings_Click(object sender, RoutedEventArgs e)
        {
            DisplayAllBookings();
            _listBoxView = false;
        }

        private void ChosenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listBoxView==true)
            {
                UpdateListBoxes();
                GenerateListBoxes();
            }
        }
        private void UpdateElementPaths()
        {
            UpdateListBoxes(); //Uppdater
        }
    }
}