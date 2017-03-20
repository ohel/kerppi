/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Windows;
using System.Windows.Input;

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
                Close();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                MessageBox.Show(x.Message, "Poikkeus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
