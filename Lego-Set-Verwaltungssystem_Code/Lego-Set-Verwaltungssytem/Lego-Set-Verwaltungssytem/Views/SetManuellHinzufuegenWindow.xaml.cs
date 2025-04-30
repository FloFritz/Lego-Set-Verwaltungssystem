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
using System.Windows.Shapes;
using Lego_Set_Verwaltungssytem.Data;
using Lego_Set_Verwaltungssytem.Models;

namespace Lego_Set_Verwaltungssytem.Views
{
    // Fenster zum manuellen Hinzufügen eines neuen Sets
    public partial class SetManuellHinzufuegenWindow : Window
    {
        public SetManuellHinzufuegenWindow()
        {
            InitializeComponent();
        }

        // Wird aufgerufen, wenn der Benutzer auf "Speichern" klickt
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            // Benutzer-ID prüfen (nur eingeloggte Benutzer dürfen speichern)
            if (App.Current.Properties["BenutzerId"] is not int benutzerId)
            {
                MessageBox.Show("Bitte zuerst einloggen.");
                return;
            }

            // Validierung von Jahr und Anzahl (müssen Zahlen sein)
            if (!int.TryParse(txtJahr.Text, out int jahr) ||
                !int.TryParse(txtAnzahl.Text, out int anzahl))
            {
                MessageBox.Show("Bitte gültige Zahlen für Jahr und Anzahl angeben.");
                return;
            }

            // Preis optional, wird 0 wenn leer oder ungültig
            double.TryParse(txtPreis.Text, out double preis);

            // Neues Set-Objekt vorbereiten
            var set = new LegoSet
            {
                Nummer = txtNummer.Text.Trim(),
                Name = txtName.Text.Trim(),
                Thema = txtThema.Text.Trim(),
                Jahr = jahr,
                PreisUVP = preis
            };

            using (var db = new AppDbContext())
            {
                // Prüfen, ob das Set bereits existiert
                var existing = db.Sets.FirstOrDefault(s => s.Nummer == set.Nummer);
                if (existing == null)
                {
                    // Neues Set speichern
                    db.Sets.Add(set);
                    db.SaveChanges();
                }
                else
                {
                    // Existierendes Set verwenden
                    set = existing;
                }

                // Neues BenutzerSet anlegen (Verknüpfung zwischen Benutzer und Set)
                var benutzerSet = new BenutzerSet
                {
                    BenutzerId = benutzerId,
                    SetId = set.SetId,
                    Anzahl = anzahl,
                    Kaufdatum = DateTime.Now,
                    GezahlterPreis = preis,
                    Notizen = txtNotiz.Text
                };

                db.BenutzerSets.Add(benutzerSet);
                db.SaveChanges();
            }

            MessageBox.Show("Set erfolgreich hinzugefügt.");
            this.Close(); // Fenster schließen
        }
    }
}