using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab3.Restaurant;
using Lab3.Tables;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Restaurants Azteka = new Restaurants();
        public MainWindow()
        {
            InitializeComponent();
            Restaurants Azteka = new Restaurants();
            foreach (ITable table in Azteka.Tables)
            {
                Testar.Text += table.TableNumber + "\n";
            }
        }

        private void TestKnapp_Click(object sender, RoutedEventArgs e)
        {
            Azteka.WriteTableFile(Azteka.Tables);
        }
    }
}
