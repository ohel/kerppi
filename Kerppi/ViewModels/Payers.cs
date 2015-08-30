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
    class Payers : INotifyPropertyChanged
    {
        private List<DataModel.Payer> initialPayerList = new List<DataModel.Payer>();
        private bool _isEdited = false;
        private ObservableCollection<DataModel.Payer> _payerList = new ObservableCollection<DataModel.Payer>();
        public ObservableCollection<DataModel.Payer> PayerList { get { return _payerList; } set { _payerList = value; NotifyPropertyChanged(() => PayerList); } }
        public bool IsEdited { get { return _isEdited; } set { _isEdited = value; NotifyPropertyChanged(() => IsEdited); } }

        public Payers()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject())) Refresh();
        }

        public void RemovePayerFromList(DataModel.Payer Payer)
        {
            PayerList.Remove(Payer);
        }

        public void SavePayers()
        {
            var toBeDeleted = new List<DataModel.Payer>();

            foreach (var payer in initialPayerList)
            {
                var match = PayerList.FirstOrDefault(p => p.Id == payer.Id);
                if (match == null) toBeDeleted.Add(payer);
            }
            
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    toBeDeleted.ForEach(p => p.Delete(conn, t));
                    foreach (var payer in PayerList)
                    {
                        // Do not save empty names.
                        if (String.IsNullOrWhiteSpace(payer.Name))
                        {
                            if (payer.Id != null) payer.Delete(conn, t); // Also remove those existing with empty names.
                        }
                        else
                        {
                            payer.Save(conn, t);
                        }
                    }
                    t.Commit();
                }
            }

            Refresh();
        }

        private void Refresh()
        {
            initialPayerList = DataModel.Payer.LoadAll().ToList();
            PayerList = new ObservableCollection<DataModel.Payer>(initialPayerList.Select(p => p.Copy()));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
