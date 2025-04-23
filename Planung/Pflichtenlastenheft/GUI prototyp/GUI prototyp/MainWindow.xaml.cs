using System.Windows;

namespace LegoSetManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InventarButton_Click(object sender, RoutedEventArgs e)
        {
            // Logik für den Wechsel zum Inventar-Tab
            MainTabControl.SelectedIndex = 1; // Tab "Inventar" aktivieren
        }

        private void AddSetButton_Click(object sender, RoutedEventArgs e)
        {
            // Logik für den Wechsel zum "Set Hinzufügen"-Tab
            MainTabControl.SelectedIndex = 2; // Tab "Set Hinzufügen" aktivieren
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // Logik für den Wechsel zum Export-Tab
            MainTabControl.SelectedIndex = 3; // Tab "Export" aktivieren
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Popup-Dialog für Login/Registrierung anzeigen
            var result = MessageBox.Show("Möchten Sie sich anmelden oder registrieren?", "Login/Registrierung", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Logik für Anmeldung
                MessageBox.Show("Anmeldung erfolgreich!", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (result == MessageBoxResult.No)
            {
                // Logik für Registrierung
                MessageBox.Show("Registrierung erfolgreich!", "Registrieren", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
