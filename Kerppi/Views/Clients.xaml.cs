/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                var dc = (ViewModels.Clients)DataContext;
                dc.CurrentClient = client.Copy();
                comboBoxPayers.SelectedItem = dc.PayerList.FirstOrDefault(p => p.Id == client.DefaultPayerContactId);
                if (comboBoxPayers.SelectedItem == null) comboBoxPayers.Text = comboBoxPayers.Tag.ToString();
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
                    $"Seuraava asiakas poistetaan:{Environment.NewLine}Tunniste: {client.IdCode}{Environment.NewLine}Nimi: {client.Name}",
                    "Vahvista asiakkaan poisto",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    ((ViewModels.Clients)DataContext).RemoveClient(client);
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

            if (String.IsNullOrEmpty(((ViewModels.Clients)DataContext).CurrentClient.IdCode))
            {
                MessageBox.Show("Tunniste ei voi olla tyhjä.", "Puuttuvia tietoja", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (String.IsNullOrEmpty(((ViewModels.Clients)DataContext).CurrentClient.Name))
            {
                MessageBox.Show("Nimi ei voi olla tyhjä.", "Puuttuvia tietoja", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            try
            {
                ((ViewModels.Clients)DataContext).SaveCurrentClient();
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
            if (DataContext == null) DataContext = new ViewModels.Clients();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dataGridClients.SelectedItem = null;
            ((ViewModels.Clients)DataContext).CurrentClient = new DataModel.Client();
        }

        private void ComboBoxPayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            ((ViewModels.Clients)DataContext).SetCurrentClientDefaultPayerObject(c.SelectedItem);
        }

        private void ComboBoxPayers_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            if (c.SelectedItem == null) {
                ((ViewModels.Clients)DataContext).SetCurrentClientDefaultPayerObject(null);
                c.Text = c.Tag.ToString();
            }
        }
    }
}
