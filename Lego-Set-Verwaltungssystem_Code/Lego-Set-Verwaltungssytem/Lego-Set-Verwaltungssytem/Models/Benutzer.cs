using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lego_Set_Verwaltungssytem.Models
{
    public class Benutzer
    {
        public int BenutzerId { get; set; }
        public string Benutzername { get; set; }
        public string Email { get; set; }
        public string PasswortHash { get; set; }
        public string Sicherheitscode { get; set; } = string.Empty;

        public ICollection<BenutzerSet> BenutzerSets { get; set; } = new List<BenutzerSet>();
    }
}
