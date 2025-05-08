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
using System.Windows.Controls;
using Lego_Set_Verwaltungssytem.Data;
using Lego_Set_Verwaltungssytem.Services;

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            LadeInfos();
        }

        private void LadeInfos()
        {
            string name = App.Current.Properties["BenutzerName"]?.ToString() ?? "Gast";
            txtBegruessung.Text = $"Hallo {name}!";

            if (name != "Gast")
            {
                using var db = new AppDbContext();
                int anzahl = db.BenutzerSets.Count(bs => bs.Benutzer.Benutzername == name);
                txtSammlungsInfo.Text = $"Du hast aktuell {anzahl} Sets in deiner Sammlung.";
            }

            txtLizenzInfo.Text = string.IsNullOrWhiteSpace(RebrickableService.ApiKey)
                ? "⚠ Kein API-Key gefunden – Suche und Import gesperrt."
                : "✔ API-Key ist aktiv – alle Funktionen verfügbar.";
        }

        private void BtnSammlung_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new SammlungPage());

        private void BtnSuche_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new SetSuchePage());

        private void BtnStatistik_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new StatistikenPage());
    }

}
