using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kerppi.ViewModels
{
    class Clients : INotifyPropertyChanged
    {
        private DataModel.Client _currentClient = new DataModel.Client();
        private ObservableCollection<DataModel.Client> _clientList = new ObservableCollection<DataModel.Client>();
        public DataModel.Client CurrentClient { get { return _currentClient; } set { _currentClient = value; NotifyPropertyChanged(() => CurrentClient); } }
        public ObservableCollection<DataModel.Client> ClientList { get { return _clientList; } set { _clientList = value; NotifyPropertyChanged(() => ClientList); } }

        public Clients()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                ClientList = new ObservableCollection<DataModel.Client>(DataModel.Client.LoadAll());
                CurrentClient = new DataModel.Client();
            }
        }

        public void SaveCurrentClient()
        {
            DataModel.Client match = ClientList.FirstOrDefault(Client => Client.IdCode == CurrentClient.IdCode);
            // The user is (visually) updating based on client id code, not database id.
            CurrentClient.Id = match != null ? match.Id : null;
            CurrentClient.Save();
            ClientList = new ObservableCollection<DataModel.Client>(DataModel.Client.LoadAll());
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
