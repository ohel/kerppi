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
    /// Interaction logic for MiscSetting.xaml
    /// </summary>
    public partial class MiscSetting : Window
    {
        private string key = "";

        public MiscSetting(string miscTableKey, string description)
        {
            InitializeComponent();

            key = miscTableKey;
            textBlockDescription.Text = description;
            textBoxValue.Text = DBHandler.QueryMisc(miscTableKey);
            textBoxValue.Focus();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DBHandler.SaveMisc(key, textBoxValue.Text);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
