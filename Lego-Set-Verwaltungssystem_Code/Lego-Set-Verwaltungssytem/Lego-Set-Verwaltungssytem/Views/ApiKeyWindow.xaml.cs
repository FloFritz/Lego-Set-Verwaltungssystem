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
using System.IO;
using System.Windows;


namespace Lego_Set_Verwaltungssytem.Views
{
    /// <summary>
    /// Interaktionslogik für ApiKeyWindow.xaml
    /// </summary>
    public partial class ApiKeyWindow : Window
    {

        private const string ApiKeyPfad = "api.txt";


        public ApiKeyWindow()
        {
            InitializeComponent();

            // Falls Datei schon existiert, schlage Key vor
            if (File.Exists(ApiKeyPfad))
                txtApiKey.Text = File.ReadAllText(ApiKeyPfad);
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            var key = txtApiKey.Text.Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("API-Key darf nicht leer sein.");
                return;
            }

            File.WriteAllText(ApiKeyPfad, key);
            MessageBox.Show("API-Key gespeichert.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
