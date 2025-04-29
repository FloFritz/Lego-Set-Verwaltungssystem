using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Lego_Set_Verwaltungssytem.Services;

// Diese Seite bietet die Oberfläche für die API-gestützte Set-Suche und das Thema-Vorladen
namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class SetSuchePage : Page
    {
        // Liste für aktuelle Suchergebnisse (wird in der ListView angezeigt)
        private List<LegoSet> suchErgebnisse = new List<LegoSet>();

        public SetSuchePage()
        {
            InitializeComponent();
            LadeThemenInDropdown(); // Themen beim Laden der Seite in das Dropdown einfügen
        }

        // Lädt alle Themen aus der API und befüllt das ComboBox-Dropdown
        private async void LadeThemenInDropdown()
        {
            cmbVorladeThema.Items.Clear();

            var themenListe = await RebrickableService.LadeAlleThemenNamenAsync();

            foreach (var thema in themenListe)
            {
                cmbVorladeThema.Items.Add(thema);
            }
        }

        // Wird aufgerufen, wenn der Benutzer manuell nach einem Set sucht
        private async void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            string suchbegriff = txtSuche.Text.Trim();
            if (string.IsNullOrEmpty(suchbegriff))
            {
                MessageBox.Show("Bitte einen Suchbegriff eingeben.");
                return;
            }

            string suchtyp = (cmbSuchtyp.SelectedItem as ComboBoxItem)?.Content?.ToString();
            suchErgebnisse = new List<LegoSet>();

            using (var db = new AppDbContext())
            {
                // Lokale Suche je nach Suchtyp
                if (suchtyp == "Setnummer")
                {
                    suchErgebnisse = db.Sets
                        .Where(s => s.Nummer.ToLower().Contains(suchbegriff.ToLower()))
                        .ToList();
                }
                else if (suchtyp == "Name")
                {
                    suchErgebnisse = db.Sets
                        .Where(s => s.Name.ToLower().Contains(suchbegriff.ToLower()))
                        .ToList();
                }
                else if (suchtyp == "Thema")
                {
                    suchErgebnisse = db.Sets
                        .Where(s => s.Thema.ToLower().Contains(suchbegriff.ToLower()))
                        .ToList();
                }
            }

            // Wenn nichts gefunden wurde → online in der API suchen
            if (suchErgebnisse.Count == 0)
            {
                suchErgebnisse = await RebrickableService.SucheSetsAsync(suchbegriff, suchtyp);
            }

            lvErgebnisse.ItemsSource = suchErgebnisse;
        }


        // Lädt alle Sets eines bestimmten Themas aus der API und speichert sie lokal
        private async void BtnThemaVorladen_Click(object sender, RoutedEventArgs e)
        {
            if (cmbVorladeThema.SelectedItem == null)
            {
                MessageBox.Show("Bitte ein Thema auswählen.");
                return;
            }

            string thema = cmbVorladeThema.SelectedItem as string;

            var sets = await RebrickableService.SucheAlleSetsNachThemaAsync(thema);

            if (sets == null || sets.Count == 0)
            {
                MessageBox.Show("Keine Sets gefunden.");
                return;
            }

            // Fortschrittsbalken konfigurieren
            progressBar.Visibility = Visibility.Visible;
            progressBar.Minimum = 0;
            progressBar.Maximum = sets.Count;
            progressBar.Value = 0;

            int neueSets = 0;

            // Sets lokal in der Datenbank speichern, wenn sie noch nicht existieren
            using (var db = new AppDbContext())
            {
                foreach (var set in sets)
                {
                    bool existiert = db.Sets.Any(s => s.Nummer == set.Nummer);
                    if (!existiert)
                    {
                        db.Sets.Add(set);
                        neueSets++;
                    }

                    progressBar.Value += 1;
                    await Task.Delay(5); // kleine Pause für optisches Feedback
                }
                db.SaveChanges();
            }

            progressBar.Visibility = Visibility.Collapsed;

            MessageBox.Show($"{sets.Count} Sets gefunden.\nDavon {neueSets} neue Sets gespeichert.");
        }

        // Wird aufgerufen, wenn der Benutzer auf "Hinzufügen" klickt
        private async void BtnHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LegoSet selectedSet)
            {
                using (var db = new AppDbContext())
                {
                    var setInDb = db.Sets.FirstOrDefault(s => s.Nummer == selectedSet.Nummer);

                    // Set noch nicht in DB → Preis laden + speichern
                    if (setInDb == null)
                    {
                        double preis = await RebrickableService.LadePreisVonSetAsync(selectedSet.Nummer);
                        selectedSet.PreisUVP = preis;

                        db.Sets.Add(selectedSet);
                        db.SaveChanges();
                        setInDb = selectedSet;
                    }

                    // Benutzer-ID aus globalem Speicher holen
                    if (App.Current.Properties["BenutzerId"] is int benutzerId)
                    {
                        // Anzahl aus der TextBox auslesen (pro Zeile)
                        int anzahl = 1;
                        foreach (var item in lvErgebnisse.Items)
                        {
                            if (item == selectedSet)
                            {
                                var container = lvErgebnisse.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                                if (container != null)
                                {
                                    var textBoxes = FindVisualChildren<TextBox>(container);
                                    foreach (var tb in textBoxes)
                                    {
                                        if (int.TryParse(tb.Text, out int parsedAnzahl) && parsedAnzahl > 0)
                                        {
                                            anzahl = parsedAnzahl;
                                        }
                                    }
                                }
                            }
                        }

                        // BenutzerSet erstellen oder erhöhen
                        var vorhandenesBenutzerSet = db.BenutzerSets.FirstOrDefault(bs => bs.BenutzerId == benutzerId && bs.SetId == setInDb.SetId);

                        if (vorhandenesBenutzerSet == null)
                        {
                            var neuesBenutzerSet = new BenutzerSet
                            {
                                BenutzerId = benutzerId,
                                SetId = setInDb.SetId,
                                Kaufdatum = DateTime.Now,
                                Notizen = "",
                                GezahlterPreis = setInDb.PreisUVP,
                                Anzahl = anzahl
                            };
                            db.BenutzerSets.Add(neuesBenutzerSet);
                        }
                        else
                        {
                            vorhandenesBenutzerSet.Anzahl += anzahl;
                        }

                        db.SaveChanges();
                        MessageBox.Show($"Set {setInDb.Name} wurde deiner Sammlung hinzugefügt!");
                    }
                    else
                    {
                        MessageBox.Show("Bitte zuerst einloggen, um Sets zu speichern.");
                    }
                }
            }
        }

        // Hilfsfunktion um verschachtelte TextBoxen in ListView zu finden
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        // Speichert Anzahl-Werte pro Set, falls außerhalb der Datenbank verwendet wird
        private Dictionary<LegoSet, int> setAnzahlen = new Dictionary<LegoSet, int>();

        // Wird aufgerufen, wenn der Benutzer das Eingabefeld für Anzahl verlässt
        private void TxtAnzahl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is LegoSet set)
            {
                if (int.TryParse(txt.Text, out int anzahl) && anzahl > 0)
                {
                    setAnzahlen[set] = anzahl;
                }
                else
                {
                    setAnzahlen[set] = 1;
                }
            }
        }
    }
}

