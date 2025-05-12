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
using Lego_Set_Verwaltungssytem.Models;
using Microsoft.EntityFrameworkCore;


namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class SammlungPage : Page
    {
        private List<BenutzerSet> benutzerSets = new();

        public SammlungPage()
        {
            InitializeComponent();
            LadeSammlung();
        }

        // Lädt alle Sets des aktuell eingeloggten Benutzers
        private void LadeSammlung()
        {
            if (App.Current.Properties["BenutzerId"] is int benutzerId)
            {
                using (var db = new AppDbContext())
                {
                    // Lade alle BenutzerSets für den eingeloggten Benutzer inklusive zugehörigem Set (mit PreisUVP etc.)
                    benutzerSets = db.BenutzerSets
                        .Where(bs => bs.BenutzerId == benutzerId)
                        .Include(bs => bs.Set)
                        .ToList();
                }

                // Setze die Sammlung in die ListView
                lvSammlung.ItemsSource = null;
                lvSammlung.ItemsSource = benutzerSets;
            }
            else
            {
                MessageBox.Show("Bitte zuerst einloggen.");
            }
        }


        // Speichert Änderungen an Anzahl, Preis oder Notizen
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is BenutzerSet selected)
            {
                if (App.Current.Properties["BenutzerId"] is int benutzerId)
                {
                    using (var db = new AppDbContext())
                    {
                        var eintrag = db.BenutzerSets
                            .Include(bs => bs.Set)
                            .FirstOrDefault(b => b.BenutzerSetId == selected.BenutzerSetId && b.BenutzerId == benutzerId);

                        if (eintrag != null)
                        {
                            eintrag.Anzahl = selected.Anzahl;
                            eintrag.GezahlterPreis = selected.GezahlterPreis;
                            eintrag.Notizen = selected.Notizen;

                            if (eintrag.Set != null && selected.Set != null)
                            {
                                eintrag.Set.PreisUVP = selected.Set.PreisUVP;
                            }

                            db.SaveChanges();
                            MessageBox.Show("Änderungen gespeichert.");
                            LadeSammlung();
                        }
                    }
                }
            }
        }



        // Löscht einen Eintrag aus der Sammlung
        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is BenutzerSet selected)
            {
                var result = MessageBox.Show("Willst du dieses Set wirklich aus deiner Sammlung löschen?", "Löschen bestätigen", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var eintrag = db.BenutzerSets.FirstOrDefault(b => b.BenutzerSetId == selected.BenutzerSetId);
                        if (eintrag != null)
                        {
                            db.BenutzerSets.Remove(eintrag);
                            db.SaveChanges();
                            MessageBox.Show("Set wurde gelöscht.");
                        }
                    }

                    // Ansicht aktualisieren
                    LadeSammlung();
                }
            }
        }


        private void BtnSetManuellHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            var win = new SetManuellHinzufuegenWindow();
            win.ShowDialog();
            LadeSammlung(); // Liste nach Hinzufügen aktualisieren
        }

    }
}
