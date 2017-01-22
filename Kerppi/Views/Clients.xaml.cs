/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : UserControl
    {
        public Clients()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var client = e.AddedItems[0] as DataModel.Client ?? new DataModel.Client();
                ((ViewModels.Clients)this.DataContext).CurrentClient = client.Copy();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var client = s.Tag as DataModel.Client;
                if (!ViewModels.Clients.CanRemoveClient(client))
                {
                    MessageBox.Show("Asiakkaaseen liittyy jo töitä. Asiakasta ei voi poistaa.");
                }
                else if (MessageBox.Show(
                    String.Format("Seuraava asiakas poistetaan:{0}Tunniste: {1}{2}Nimi: {3}", Environment.NewLine, client.IdCode, Environment.NewLine, client.Name),
                    "Vahvista asiakkaan poisto",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    ((ViewModels.Clients)this.DataContext).RemoveClient(client);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in VisualTreeWalker.FindVisualChildren<TextBox>(this))
            {
                var be = tb.GetBindingExpression(TextBox.TextProperty);
                if (be != null) be.UpdateSource();
            }

            if (String.IsNullOrEmpty(((ViewModels.Clients)this.DataContext).CurrentClient.IdCode))
            {
                MessageBox.Show("Tunniste ei voi olla tyhjä.", "Puuttuvia tietoja", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (String.IsNullOrEmpty(((ViewModels.Clients)this.DataContext).CurrentClient.Name))
            {
                MessageBox.Show("Nimi ei voi olla tyhjä.", "Puuttuvia tietoja", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            try
            {
                ((ViewModels.Clients)this.DataContext).SaveCurrentClient();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                MessageBox.Show("Tallennus epäonnistui." + Environment.NewLine + "Virhe:" + x.Message);
            }
        }

        /// <summary>
        /// Note: this needs an interactive element (Button, TextBox etc.) in the view to fire.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (this.DataContext == null) this.DataContext = new ViewModels.Clients();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dataGridClients.SelectedItem = null;
            ((ViewModels.Clients)this.DataContext).CurrentClient = new DataModel.Client();
        }
    }
}
