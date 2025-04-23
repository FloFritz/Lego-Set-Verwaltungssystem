using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lego_Set_Verwaltungssytem.Data;
using Lego_Set_Verwaltungssytem.Views;

namespace Lego_Set_Verwaltungssytem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Datenbank automatisch erstellen, falls sie nicht existiert
            var db = new AppDbContext();
            db.EnsureDatabase();


            InitializeComponent();
            // Starte mit HomePage
            MainFrame.Navigate(new HomePage());
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }

        private void SammlungButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SammlungPage());
        }

        private void NavigateAlbumSuche(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SetSuchePage());
        }
    }
}