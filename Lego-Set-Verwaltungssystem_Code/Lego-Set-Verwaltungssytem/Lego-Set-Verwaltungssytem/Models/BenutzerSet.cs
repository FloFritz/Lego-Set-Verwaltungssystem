using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego_Set_Verwaltungssytem.Models
{
    public class BenutzerSet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BenutzerSetId { get; set; }

        public int BenutzerId { get; set; }
        public Benutzer Benutzer { get; set; }

        public int SetId { get; set; }
        public LegoSet Set { get; set; }

        public int Anzahl { get; set; } = 1;
        public DateTime Kaufdatum { get; set; }
        public string Notizen { get; set; }
        public double GezahlterPreis { get; set; }
    }

}

