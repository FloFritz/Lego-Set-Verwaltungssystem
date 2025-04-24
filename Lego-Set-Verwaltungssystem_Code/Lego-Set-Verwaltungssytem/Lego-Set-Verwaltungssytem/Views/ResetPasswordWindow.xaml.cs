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
using System.Linq;
using System.Security.Cryptography;
using Lego_Set_Verwaltungssytem.Data;

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        private void BtnZuruecksetzen_Click(object sender, RoutedEventArgs e)
        {
            string benutzername = txtBenutzername.Text;
            string sicherheitscode = txtSicherheitscode.Text;
            string neuesPasswort = txtNeuesPasswort.Password;
            string neuesHash = HashPasswort(neuesPasswort);

            using (var db = new AppDbContext())
            {
                var user = db.Benutzer.FirstOrDefault(b => b.Benutzername == benutzername && b.Sicherheitscode == sicherheitscode);
                if (user != null)
                {
                    user.PasswortHash = neuesHash;
                    db.SaveChanges();

                    MessageBox.Show("Passwort erfolgreich zurückgesetzt!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Benutzername oder Sicherheitscode ist falsch.");
                }
            }
        }

        private string HashPasswort(string passwort)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(passwort));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
