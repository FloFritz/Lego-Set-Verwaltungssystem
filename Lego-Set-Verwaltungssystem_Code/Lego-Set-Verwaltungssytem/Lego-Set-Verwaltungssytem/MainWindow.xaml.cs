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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Datenbank erstellen, falls sie noch nicht existiert
            var db = new AppDbContext();
            db.EnsureDatabase();

            InitializeComponent();
            MainFrame.Navigate(new HomePage());

            // Benutzer anzeigen, wenn bereits eingeloggt (z.B. nach erfolgreichem Login)
            if (App.Current.Properties.Contains("BenutzerName"))
            {
                string name = App.Current.Properties["BenutzerName"]?.ToString() ?? "Unbekannt";
                txtBenutzerInfo.Text = $"Angemeldet als: {name}";
                txtBenutzerInfo.Visibility = Visibility.Visible;
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
            }
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                string name = App.Current.Properties["BenutzerName"]?.ToString() ?? "Unbekannt";
                txtBenutzerInfo.Text = $"Angemeldet als: {name}";
                txtBenutzerInfo.Visibility = Visibility.Visible;
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;

                MessageBox.Show($"Willkommen, {name}!");
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Benutzer abmelden
            App.Current.Properties.Remove("BenutzerId");
            App.Current.Properties.Remove("BenutzerName");

            txtBenutzerInfo.Text = "";
            txtBenutzerInfo.Visibility = Visibility.Collapsed;

            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Collapsed;

            MessageBox.Show("Du wurdest erfolgreich abgemeldet.");
        }
    }
}
