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

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for NewPassword.xaml
    /// </summary>
    public partial class NewPassword : Window
    {
        public NewPassword()
        {
            InitializeComponent();
            passwordBox.Focus();
        }

        private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) passwordBox2.Focus();
        }

        private void PasswordBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) ChangePassword();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword();
        }

        private void ChangePassword()
        {
            if (passwordBox.Password != passwordBox2.Password)
            {
                MessageBox.Show("Salasanat eivät täsmää.", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                DBHandler.ChangePassword(passwordBox.Password);
                if (String.IsNullOrEmpty(passwordBox.Password))
                {
                    MessageBox.Show("Salasana poistettu.");
                }
                else
                {
                    MessageBox.Show("Salasana vaihdettu. Tietokanta tallennetaan vielä salaamattomana siltä varalta, että unohdat salasanan.");
                    string filename = DBHandler.ExportUnencrypted();
                    MessageBox.Show("Tietokanta tallennettiin salaamattomana tiedostoon:" + Environment.NewLine + filename, "Valmis", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                this.Close();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                MessageBox.Show(x.Message, "Poikkeus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
