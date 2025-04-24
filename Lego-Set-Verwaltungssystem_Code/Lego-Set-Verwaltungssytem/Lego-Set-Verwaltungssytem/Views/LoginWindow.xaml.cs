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
using System.Security.Cryptography;
using Lego_Set_Verwaltungssytem.Data;

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string benutzername = txtBenutzername.Text;
            string passwort = txtPasswort.Password;
            string hash = HashPasswort(passwort);

            using (var db = new AppDbContext())
            {
                var user = db.Benutzer.FirstOrDefault(u => u.Benutzername == benutzername && u.PasswortHash == hash);
                if (user != null)
                {
                    App.Current.Properties["BenutzerId"] = user.BenutzerId;
                    App.Current.Properties["BenutzerName"] = user.Benutzername;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Falscher Benutzername oder Passwort.");
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow reg = new RegisterWindow();
            reg.ShowDialog();
        }

        private string HashPasswort(string passwort)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(passwort));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void BtnPasswortVergessen_Click(object sender, RoutedEventArgs e)
        {
            ResetPasswordWindow resetWindow = new ResetPasswordWindow();
            resetWindow.ShowDialog();
        }

    }
}
