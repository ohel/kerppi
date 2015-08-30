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
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Password : Window
    {
        public Password()
        {
            InitializeComponent();
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
