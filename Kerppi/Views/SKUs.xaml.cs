/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for SKUs.xaml
    /// </summary>
    public partial class SKUs : UserControl
    {
        public SKUs()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sku = new DataModel.SKU();
            if (e.AddedItems.Count > 0) sku = e.AddedItems[0] as DataModel.SKU ?? sku;

            ((ViewModels.SKUs)this.DataContext).CurrentSKU = sku.Copy();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var sku = s.Tag as DataModel.SKU;
                if (MessageBox.Show(
                    String.Format("Seuraava nimike poistetaan:{0}Koodi: {1}{2}Kuvaus: {3}", Environment.NewLine, sku.Code, Environment.NewLine, sku.Description),
                    "Vahvista nimikkeen poisto",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    ((ViewModels.SKUs)this.DataContext).RemoveSKU(sku);
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

            var sku = ((ViewModels.SKUs)this.DataContext).CurrentSKU;
            if (sku.Code.Contains('|') || sku.Description.Contains('|'))
            {
                MessageBox.Show("Nimikkeen koodi ja kuvaus eivät saa sisältää piippumerkkiä |.", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                ((ViewModels.SKUs)this.DataContext).SaveCurrentSKU();
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
            if (this.DataContext == null) this.DataContext = new ViewModels.SKUs();
        }

        private void TextBox_GotFocus_SelectAll(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            tb.SelectAll();
        }

        private void TextBox_GotFocus_CaretIndex(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            tb.CaretIndex = tb.Text.Length;
        }
    }
}
