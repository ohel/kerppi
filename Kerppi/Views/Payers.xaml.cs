/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Windows;
using System.Windows.Controls;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for Payers.xaml
    /// </summary>
    public partial class Payers : UserControl
    {
        public Payers()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Note: this needs an interactive element (Button, TextBox etc.) in the view to fire.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (DataContext == null) DataContext = new ViewModels.Payers();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var payer = s.Tag as DataModel.Payer;
                {
                    ((ViewModels.Payers)DataContext).RemovePayerFromList(payer);
                    ((ViewModels.Payers)DataContext).IsEdited = true;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModels.Payers)DataContext).SavePayers();
            ((ViewModels.Payers)DataContext).IsEdited = false;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit) ((ViewModels.Payers)DataContext).IsEdited = true;
        }
    }
}
