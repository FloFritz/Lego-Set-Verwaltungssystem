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
            if (sender is Button btn && btn.CommandParameter is BenutzerSet selected)
            {
                Console.WriteLine($"DEBUG: Speichern für ID={selected.BenutzerSetId}, Preis={selected.GezahlterPreis}");

                if (App.Current.Properties["BenutzerId"] is int benutzerId)
                {
                    using (var db = new AppDbContext())
                    {
                        // BenutzerSet inkl. Set laden
                        var eintrag = db.BenutzerSets
                            .Include(bs => bs.Set)
                            .FirstOrDefault(x => x.BenutzerSetId == selected.BenutzerSetId && x.BenutzerId == benutzerId);

                        if (eintrag != null)
                        {
                            // BenutzerSet-Daten übernehmen
                            eintrag.GezahlterPreis = selected.GezahlterPreis;
                            eintrag.Anzahl = selected.Anzahl;
                            eintrag.Notizen = selected.Notizen;

                            // Set-Daten (global!)
                            eintrag.Set.PreisUVP = selected.Set.PreisUVP;
                            db.Entry(eintrag.Set).State = EntityState.Modified;

                            db.SaveChanges();
                            MessageBox.Show("Änderungen gespeichert.");
                            LadeSammlung();
                        }
                        else
                        {
                            MessageBox.Show("Eintrag nicht gefunden oder gehört nicht dir.");
                        }
                    }
                }
            }
        }









        // Löscht einen Eintrag aus der Sammlung
        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is BenutzerSet selected)
            {
                if (App.Current.Properties["BenutzerId"] is int benutzerId)
                {
                    using (var db = new AppDbContext())
                    {
                        var eintrag = db.BenutzerSets
                            .FirstOrDefault(x => x.BenutzerSetId == selected.BenutzerSetId && x.BenutzerId == benutzerId);

                        if (eintrag != null)
                        {
                            db.BenutzerSets.Remove(eintrag);
                            db.SaveChanges();
                            MessageBox.Show("Set gelöscht.");
                            LadeSammlung();
                        }
                        else
                        {
                            MessageBox.Show("Dieses Set gehört dir nicht.");
                        }
                    }
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
