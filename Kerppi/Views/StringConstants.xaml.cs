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
    /// Interaction logic for StringConstants.xaml
    /// </summary>
    public partial class StringConstants : UserControl
    {
        public StringConstants()
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
            if (DataContext == null) DataContext = new ViewModels.StringConstants();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var sc = s.Tag as DataModel.StringConstant;
                {
                    ((ViewModels.StringConstants)DataContext).RemoveStringConstantFromList(sc);
                    ((ViewModels.StringConstants)DataContext).IsEdited = true;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModels.StringConstants)DataContext).SaveStringConstants();
            ((ViewModels.StringConstants)DataContext).IsEdited = false;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit) ((ViewModels.StringConstants)DataContext).IsEdited = true;
        }
    }
}
