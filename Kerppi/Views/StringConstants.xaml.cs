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
            if (this.DataContext == null) this.DataContext = new ViewModels.StringConstants();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if (s != null && s.Tag != null)
            {
                var sc = s.Tag as DataModel.StringConstant;
                {
                    ((ViewModels.StringConstants)this.DataContext).RemoveStringConstantFromList(sc);
                    ((ViewModels.StringConstants)this.DataContext).IsEdited = true;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModels.StringConstants)this.DataContext).SaveStringConstants();
            ((ViewModels.StringConstants)this.DataContext).IsEdited = false;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit) ((ViewModels.StringConstants)this.DataContext).IsEdited = true;
        }
    }
}
