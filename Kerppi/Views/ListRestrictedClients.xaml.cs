/*
    Copyright 2018 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Linq;
using System.Windows;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for ListRestrictedClients.xaml
    /// </summary>
    public partial class ListRestrictedClients: Window
    {
        private class ClientListItem
        {
            public string Name { get; set; }
            public string IdCode { get; set; }
            public long? Id { get; set; }
            public override string ToString()
            {
                return $"{Name} ({IdCode})";
            }
        }

        private Action _callback = null;

        public ListRestrictedClients(Action callback)
        {
            InitializeComponent();
            Title = AttributeHelper.GetAttribute<System.Reflection.AssemblyProductAttribute>().Product;
            DataContext = DataModel.Client.LoadAllRestricted()
                .Select(c => new ClientListItem { Name = c.Name, IdCode = c.IdCode, Id = c.Id })
                .ToList();
            _callback = callback;
        }

        private void ButtonUnrestrict_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxClients.SelectedItem is ClientListItem selected)
            {
                if (MessageBox.Show(
                    "Poistetaanko tietorajoitus asiakkaalta?", "Vahvista tietorajoituksen poisto",
                    MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    DataModel.Client.LoadWithId(selected.Id).ToggleRestricted();
                    Close();
                    _callback?.Invoke();
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
