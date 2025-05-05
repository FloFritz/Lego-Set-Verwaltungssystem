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

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class StatistikenPage : Page
    {
        public StatistikenPage()
        {
            InitializeComponent();
            LadeStatistiken();
        }

        private void LadeStatistiken()
        {
            if (App.Current.Properties["BenutzerId"] is int benutzerId)
            {
                using (var db = new AppDbContext())
                {
                    var sets = db.BenutzerSets
                        .Where(bs => bs.BenutzerId == benutzerId)
                        .Select(bs => new
                        {
                            bs.Anzahl,
                            bs.GezahlterPreis,
                            UVP = bs.Set.PreisUVP
                        })
                        .ToList();

                    int gesamtAnzahl = sets.Sum(s => s.Anzahl);
                    double gesamtBezahlt = sets.Sum(s => s.Anzahl * s.GezahlterPreis);
                    double gesamtUVP = sets.Sum(s => s.Anzahl * s.UVP);
                    double differenz = gesamtUVP - gesamtBezahlt;
                    


                    txtGesamtAnzahl.Text = gesamtAnzahl.ToString();
                    txtGesamtWertBezahlt.Text = $"{gesamtBezahlt:F2} €";
                    txtGesamtWertUVP.Text = $"{gesamtUVP:F2} €";
                    txtDifferenz.Text = $"{differenz:F2} €";
                }
            }
        }
    }
}
