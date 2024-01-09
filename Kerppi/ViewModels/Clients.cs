/*
    Copyright 2015, 2017, 2018, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Kerppi.ViewModels
{
    class Clients : INotifyPropertyChanged, IKerppiRefreshable
    {
        private DataModel.Client _currentClient = new DataModel.Client();
        private ObservableCollection<DataModel.Client> _clientList = new ObservableCollection<DataModel.Client>();
        private ObservableCollection<DataModel.Contact> _payerList = new ObservableCollection<DataModel.Contact>();
        private bool _saveAsNew;
        public DataModel.Client CurrentClient { get { return _currentClient; } set { _currentClient = value; NotifyPropertyChanged(() => CurrentClient); } }
        public ObservableCollection<DataModel.Client> ClientList { get { return _clientList; } set { _clientList = value; NotifyPropertyChanged(() => ClientList); } }
        public ObservableCollection<DataModel.Contact> PayerList { get { return _payerList; } set { _payerList = value; NotifyPropertyChanged(() => PayerList); } }
        public bool SaveAsNew { get { return _saveAsNew; } set { _saveAsNew = value; NotifyPropertyChanged(() => SaveAsNew); } }

        public Clients()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                ClientList = new ObservableCollection<DataModel.Client>(DataModel.Client.LoadAll());
                CurrentClient = new DataModel.Client();
                SaveAsNew = true;
                Refresh();
            }
        }

        public void Refresh()
        {
            PayerList = new ObservableCollection<DataModel.Contact>(DataModel.Contact.LoadAllPayers());
        }

        public void Reset()
        {
            ClientList = new ObservableCollection<DataModel.Client>(DataModel.Client.LoadAll());
            CurrentClient = new DataModel.Client();
            Refresh();
        }

        public void SaveCurrentClient()
        {
            DataModel.Client matchByCode = ClientList.FirstOrDefault(client => client.IdCode == CurrentClient.IdCode);

            // An IdCode collision occurred and it's not the same client.
            if (matchByCode != null && matchByCode.Id != CurrentClient.Id)
            {
                throw new Exception($"Asiakas tunnisteella {CurrentClient.IdCode} on jo olemassa.");
            }
            if (SaveAsNew && matchByCode == null) CurrentClient.Id = null;

            CurrentClient.Save();
            ClientList = new ObservableCollection<DataModel.Client>(DataModel.Client.LoadAll());

            // If the user changes the IdCode and saves the client as new, this updates the Id to the view model.
            CurrentClient = ClientList.First(client => client.IdCode == CurrentClient.IdCode).Copy();
        }

        public void RemoveClient(DataModel.Client client)
        {
            client.Delete();
            ClientList.Remove(client);
        }

        public static bool CanRemoveClient(DataModel.Client client)
        {
            return (DataModel.Task.LoadAllFor(client).Count() == 0);
        }

        public static bool CanSaveForClient(DataModel.Client client)
        {
            return (DataModel.Client.Exists(client.Id));
        }

        public void SetCurrentClientDefaultPayerObject(object payer)
        {
            CurrentClient.DefaultPayer = payer as DataModel.Contact;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
