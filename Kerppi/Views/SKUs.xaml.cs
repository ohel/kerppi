/*
    Copyright 2015, 2017, 2018 Olli Helin / GainIT
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
            if (e.AddedItems.Count == 0)
                return;

            UpdateTextBoxSources();

            var dc = (ViewModels.SKUs)DataContext;

            var oldSku = dc.SKUHandlerInstance.SKUList.Where(item => item.Code == dc.CurrentSKU.Code).FirstOrDefault();
            bool skuExists = oldSku != null;
            if (!skuExists) {
                oldSku = new DataModel.SKU
                {
                    // This prevents unsaved changes detection with two empty SKU objects.
                    Timestamp = dc.CurrentSKU.Timestamp
                };
            }

            if (!dc.CurrentSKU.Equals(oldSku))
            {
                if (MessageBox.Show(
                    string.Format("Sinulla on tallentamattomia muutoksia nimikkeelle koodilla: {0}.{1}{2}{3}Tallennetaanko muutokset?",
                        dc.CurrentSKU.Code, Environment.NewLine + Environment.NewLine,
                        skuExists ? "Muutokset ylikirjoittavat olemassa olevan nimikkeen tiedot." :
                            "Tallennus ei ylikirjoita olemassa olevaa nimikettä, vaan kyseessä on uusi nimike.",
                        Environment.NewLine + Environment.NewLine),
                    "Tallentamattomia muutoksia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    SaveCurrentSKU(false);
                }
            }

            dc.CurrentSKU = (e.AddedItems[0] as DataModel.SKU)?.Copy() ?? new DataModel.SKU();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button s && s.Tag != null)
            {
                var sku = s.Tag as DataModel.SKU;
                if (MessageBox.Show(
                    string.Format("Seuraava nimike poistetaan:{0}Koodi: {1}{2}Kuvaus: {3}", Environment.NewLine, sku.Code, Environment.NewLine, sku.Description),
                    "Vahvista nimikkeen poisto",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    var dc = (ViewModels.SKUs)DataContext;

                    // This prevents unsaved changes detection from triggering.
                    dc.CurrentSKU = new DataModel.SKU();
                    dc.RemoveSKU(sku);
                }
            }
        }

        private void UpdateTextBoxSources()
        {
            // Using PropertyChanged as UpdateSourceTrigger makes typing decimal numbers hard, so we update sources here.
            // For some very strange reason this refused to work in some cases, so have to revert to the old way.
            //foreach (TextBox tb in VisualTreeWalker.FindVisualChildren<TextBox>(this))
            //{
            //    var be = tb.GetBindingExpression(TextBox.TextProperty);
            //    if (be != null) be.UpdateSource();
            //}

            var dc = (ViewModels.SKUs)DataContext;
            dc.CurrentSKU.Code = textBoxCode.Text;
            dc.CurrentSKU.Description = textBoxDescription.Text;
            decimal converted = Decimal.Zero;
            Decimal.TryParse(textBoxBuyPrice.Text, out converted);
            dc.CurrentSKU.BuyPrice = converted;
            Decimal.TryParse(textBoxSellPriceFactor.Text, out converted);
            dc.CurrentSKU.SellPriceFactor = converted;
        }

        private void SaveCurrentSKU(bool selectSaved = true)
        {
            UpdateTextBoxSources();

            var dc = (ViewModels.SKUs)DataContext;
            var sku = dc.CurrentSKU;
            if (sku.Code.Contains('|') || sku.Description.Contains('|'))
            {
                MessageBox.Show("Nimikkeen koodi ja kuvaus eivät saa sisältää piippumerkkiä |.", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var savedSkuCode = dc.CurrentSKU.Code;
                dc.SaveCurrentSKU();

                if (selectSaved)
                {
                    var savedSku = dc.SKUHandlerInstance.SKUList.Where(item => item.Code == savedSkuCode).FirstOrDefault();

                    // There should always be at least one item after save.
                    // Scrolling to the bottom first makes the saved SKU be on top of the list after scrolling.
                    dataGridSKUs.ScrollIntoView(dataGridSKUs.Items[dataGridSKUs.Items.Count - 1]);
                    dataGridSKUs.UpdateLayout();
                    dataGridSKUs.ScrollIntoView(savedSku);

                    // This prevents unsaved changes detection from triggering.
                    dc.CurrentSKU = savedSku.Copy();
                    dataGridSKUs.SelectedItem = savedSku;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                MessageBox.Show("Tallennus epäonnistui." + Environment.NewLine + "Virhe:" + x.Message);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentSKU();
        }

        /// <summary>
        /// Note: this needs an interactive element (Button, TextBox etc.) in the view to fire.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (DataContext == null) DataContext = new ViewModels.SKUs();
        }

        private void TextBox_GotFocus_SelectAll(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox tb)) return;
            tb.SelectAll();
        }

        private void TextBox_GotFocus_CaretIndex(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox tb)) return;
            tb.CaretIndex = tb.Text.Length;
        }
    }
}
