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
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Password : Window
    {
        public Password()
        {
            InitializeComponent();
            this.Title = AttributeHelper.GetAttribute<System.Reflection.AssemblyProductAttribute>().Product;
            passwordBox.Focus();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) InitApp();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitApp();
        }

        private void InitApp()
        {
            DBHandler.Password = passwordBox.Password;
            try
            {
                DBHandler.InitDB();
                Console.WriteLine("Opening main view.");
                (new Views.KerppiMain()).Show();
                Console.WriteLine("Password view done, closing.");
                this.Close();
            }
            catch (KerppiException x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.InnerException != null ? x.InnerException.Message : "No inner exception.");
                MessageBox.Show("Virhe avattaessa tietokantaa. Tarkista salasana.", "Virhe", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.InnerException != null ? x.InnerException.Message : "No inner exception.");
                MessageBox.Show(x.Message, "Poikkeus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
