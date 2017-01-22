/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Windows;

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
            this.Title = AttributeHelper.GetAttribute<System.Reflection.AssemblyProductAttribute>().Product;

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
