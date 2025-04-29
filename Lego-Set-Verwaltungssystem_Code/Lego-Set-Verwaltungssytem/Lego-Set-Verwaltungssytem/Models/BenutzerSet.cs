using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego_Set_Verwaltungssytem.Models
{
    public class BenutzerSet
    {
        public int BenutzerId { get; set; }
        public Benutzer Benutzer { get; set; }
        public int Anzahl { get; set; } = 1;
        public int SetId { get; set; }
        public LegoSet Set { get; set; }

        public DateTime Kaufdatum { get; set; }
        public string Notizen { get; set; }
        public double GezahlterPreis { get; set; }
    }
}
