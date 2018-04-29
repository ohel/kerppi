/*
    Copyright 2015, 2017 Olli Helin / GainIT
*/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for SKUCollections.xaml
    /// </summary>
    public partial class SKUCollections : UserControl
    {
        public SKUCollections()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var skuCollection = new DataModel.SKUCollection();
            if (e.AddedItems.Count > 0)
            {
                skuCollection = e.AddedItems[0] as DataModel.SKUCollection ?? skuCollection;
            }

            ((ViewModels.SKUCollections)DataContext).CurrentSKUCollection = skuCollection.Copy();
        }

        private void RemoveSKUButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var sku = s.Tag as DataModel.SKU;
                ((ViewModels.SKUCollections)DataContext).RemoveFromCurrentSKUCollection(sku);
            }
        }

        private void RemoveSKUCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var skuCollection = s.Tag as DataModel.SKUCollection;
                if (MessageBox.Show(
                    string.Format("Seuraava nimikekokoelma poistetaan:{0}Koodi: {1}{2}Kuvaus: {3}", Environment.NewLine, skuCollection.Code, Environment.NewLine, skuCollection.Description),
                    "Vahvista nimikekokoelman poisto",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    ((ViewModels.SKUCollections)DataContext).RemoveSKUCollection(skuCollection);
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = dataGridAvailableSKUs.SelectedItems.Cast<DataModel.SKU>().ToList();
            ((ViewModels.SKUCollections)DataContext).AddToCurrentSKUCollection(selected);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ViewModels.SKUCollections)DataContext).SaveCurrentSKUCollection();
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
            if (DataContext == null) DataContext = new ViewModels.SKUCollections();
        }
    }
}
