/*
    Copyright 2018 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Microsoft.Win32;
using System.Windows;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for PrintClientData.xaml
    /// </summary>
    public partial class PrintClientData : Window
    {
        public PrintClientData(string data)
        {
            InitializeComponent();
            Title = AttributeHelper.GetAttribute<System.Reflection.AssemblyProductAttribute>().Product;
            textBoxData.Text = data;
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "asiakastiedot",
                Filter = "Tekstitiedostot (*.txt)|*.txt|Kaikki tiedostot (*.*)|*.*"
            };
            if (sfd.ShowDialog() != true) return;
            var file = new System.IO.StreamWriter(sfd.FileName);
            file.Write(textBoxData.Text);
            file.Close();
            Close();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
