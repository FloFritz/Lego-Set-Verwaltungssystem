using System;
using System.Collections.Generic;
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
using Lego_Set_Verwaltungssytem.Models;

namespace Lego_Set_Verwaltungssytem.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegistrieren_Click(object sender, RoutedEventArgs e)
        {
            string benutzername = txtBenutzername.Text;
            string email = txtEmail.Text;
            string passwort = txtPasswort.Password;
            string passwortHash = HashPasswort(passwort);

            string sicherheitscode = GenerateSicherheitscode(); 

            using (var db = new AppDbContext())
            {
                bool existiert = db.Benutzer.Any(b => b.Benutzername == benutzername || b.Email == email);
                if (existiert)
                {
                    MessageBox.Show("Benutzername oder E-Mail existiert bereits.");
                    return;
                }

                var neuerBenutzer = new Benutzer
                {
                    Benutzername = benutzername,
                    Email = email,
                    PasswortHash = passwortHash,
                    Sicherheitscode = sicherheitscode
                };

                db.Benutzer.Add(neuerBenutzer);
                db.SaveChanges();

                MessageBox.Show(
                                $"✅ Registrierung erfolgreich!\n\n" +
                                $"🔐 Dein persönlicher Sicherheitscode lautet:\n\n" +
                                $"{sicherheitscode}\n\n" +
                                $"❗Diesen Code brauchst du, um dein Passwort zurückzusetzen!\n" +
                                $"Bitte notiere ihn sicher.",
                                "Wichtiger Hinweis",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                this.Close();
                new LoginWindow().ShowDialog();
               
            }
        }


        private string GenerateSicherheitscode()
        {
            //Random Code Erstellen
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rand = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }


        private string HashPasswort(string passwort)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(passwort));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void BtnZurueck_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new LoginWindow().ShowDialog(); 
        }
    }
}

