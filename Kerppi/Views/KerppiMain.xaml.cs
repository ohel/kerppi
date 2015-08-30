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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for KerppiMain.xaml
    /// </summary>
    public partial class KerppiMain : Window
    {
        public KerppiMain()
        {
            InitializeComponent();
            Application.Current.DispatcherUnhandledException += KerppiExceptionHandler;
        }

        private void KerppiExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is KerppiException)
            {
                e.Handled = true;
                Console.WriteLine(((KerppiException)e.Exception).Message);
                MessageBox.Show("Tapahtui virhe: " + ((KerppiException)e.Exception).Message, "Poikkeus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Console.WriteLine(((Exception)e.Exception).Message);
                MessageBox.Show("Tapahtui vakava virhe. Tallenna kuvakaappaus tästä ilmoituksesta, kiitos. Sovellus suljetaan." + Environment.NewLine + Environment.NewLine +
                    ((Exception)e.Exception).Message, "Poikkeus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EndProgram(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            (new Views.NewPassword()).Show();
        }

        private void ExportUnencrypted(object sender, RoutedEventArgs e)
        {
            string filename = DBHandler.ExportUnencrypted();
            MessageBox.Show("Tietokanta tallennettiin salaamattomana tiedostoon:" + Environment.NewLine + filename, "Valmis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// This makes sure that whether a tab is changed with keyboard or mouse, the view model is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var icg = ((TabControl)e.Source).ItemContainerGenerator;
                var chosen = icg.Items.Select(i => i as TabItem).FirstOrDefault(t => t != null && t.IsSelected);
                if (chosen != null)
                {
                    var view = chosen.Content as FrameworkElement;
                    if (view != null && view.DataContext != null)
                    {
                        var refreshable = view.DataContext as ViewModels.Refreshable;
                        if (refreshable != null)
                        {
                            refreshable.Refresh();
                        }
                    }
                }
            }
        }

        private void About(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            MessageBox.Show(
                "Kerppi" + Environment.NewLine +
                "Versio: " + version.ToString() + Environment.NewLine +
                "© 2015 Olli Helin (olli.helin@iki.fi)" + Environment.NewLine +
                "Lisenssi- ja loppukäyttäjäehdot löytyvät tiedostosta License.txt",
                "Tietoa sovelluksesta", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangeVAT(object sender, RoutedEventArgs e)
        {
            (new MiscSetting("VAT", "Syötä ALV-kanta prosentteina (pelkät numerot, esimerkiksi 24), vaihto vaatii ohjelman uudelleenkäynnistyksen:")).Show();
        }

        private void SetPrintFooter(object sender, RoutedEventArgs e)
        {
            (new MiscSetting("PrintFooter", "Tulosteiden alatunniste:")).Show();
        }

        private void SetPrintMargin(object sender, RoutedEventArgs e)
        {
            (new MiscSetting("PrintMargin", "Tulosteiden marginaali:")).Show();
        }
    }
}
