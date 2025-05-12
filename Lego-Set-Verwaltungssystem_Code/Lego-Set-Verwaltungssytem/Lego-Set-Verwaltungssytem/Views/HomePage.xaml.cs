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
using Lego_Set_Verwaltungssytem.Data;
using Lego_Set_Verwaltungssytem.Services;
using Microsoft.EntityFrameworkCore;

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            PruefeApiZugang();
            LadeInfos();
        }

        private void PruefeApiZugang()
        {
            bool gueltig = !string.IsNullOrWhiteSpace(RebrickableService.ApiKey);

            btnSammlung.IsEnabled = gueltig;
            btnSuche.IsEnabled = gueltig;
            btnStatistik.IsEnabled = gueltig;
        }

        private void LadeInfos()
        {
            string name = App.Current.Properties["BenutzerName"]?.ToString() ?? "Gast";
            txtBegruessung.Text = $"Hallo {name}!";

            if (name != "Gast")
            {
                using var db = new AppDbContext();
                var benutzer = db.Benutzer.FirstOrDefault(b => b.Benutzername == name);

                if (benutzer != null)
                {
                    int anzahl = db.BenutzerSets.Count(bs => bs.BenutzerId == benutzer.BenutzerId);
                    txtSammlungsInfo.Text = $"Du hast aktuell {anzahl} Sets in deiner Sammlung.";

                    // 🔄 Letzte Sets laden
                    var letzte = db.BenutzerSets
                            .Include(bs => bs.Set)
                            .Where(bs => bs.BenutzerId == benutzer.BenutzerId)
                            .OrderByDescending(bs => bs.Kaufdatum)
                            .Take(5)
                            .ToList();


                    lstLetzteSets.ItemsSource = letzte;
                }
            }

            txtLizenzInfo.Text = string.IsNullOrWhiteSpace(RebrickableService.ApiKey)
                ? "⚠ Kein API-Key gefunden – Suche und Import gesperrt."
                : "✔ API-Key ist eingegeben";
        }

        private void BtnSammlung_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new SammlungPage());

        private void BtnSuche_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new SetSuchePage());

        private void BtnStatistik_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new StatistikenPage());

        private void BtnApiKeyAendern_Click(object sender, RoutedEventArgs e)
        {
            var keyWindow = new ApiKeyWindow();
            var result = keyWindow.ShowDialog();

            if (result == true)
            {
                MessageBox.Show("API-Key wurde erfolgreich geändert.");

                // Eigene Buttons aktualisieren
                PruefeApiZugang();

                // Zugriff auf MainWindow und dort Buttons aktivieren
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.AktualisiereApiZugriffButtons();
                }
            }
        }



    }

}
