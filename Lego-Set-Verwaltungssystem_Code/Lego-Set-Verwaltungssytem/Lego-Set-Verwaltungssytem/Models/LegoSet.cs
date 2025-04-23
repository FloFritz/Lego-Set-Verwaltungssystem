using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego_Set_Verwaltungssytem.Models
{
    public class LegoSet
    {
        public int SetId { get; set; }  // <- Primärschlüssel

        public string Name { get; set; } = string.Empty;
        public string Nummer { get; set; } = string.Empty;
        public string Thema { get; set; } = string.Empty;
        public int Jahr { get; set; }
        public double PreisUVP { get; set; }

        public ICollection<BenutzerSet> BenutzerSets { get; set; } = new List<BenutzerSet>();
    }
}

