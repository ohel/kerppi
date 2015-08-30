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
    class StringConstants : INotifyPropertyChanged
    {
        private List<DataModel.StringConstant> initialStringConstantList = new List<DataModel.StringConstant>();
        private bool _isEdited = false;
        private ObservableCollection<DataModel.StringConstant> _stringConstantList = new ObservableCollection<DataModel.StringConstant>();
        public ObservableCollection<DataModel.StringConstant> StringConstantList { get { return _stringConstantList; } set { _stringConstantList = value; NotifyPropertyChanged(() => StringConstantList); } }
        public bool IsEdited { get { return _isEdited; } set { _isEdited = value; NotifyPropertyChanged(() => IsEdited); } }

        public StringConstants()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject())) Refresh();
        }

        public void RemoveStringConstantFromList(DataModel.StringConstant StringConstant)
        {
            StringConstantList.Remove(StringConstant);
        }

        public void SaveStringConstants()
        {
            var toBeDeleted = new List<DataModel.StringConstant>();

            foreach (var StringConstant in initialStringConstantList)
            {
                var match = StringConstantList.FirstOrDefault(p => p.Id == StringConstant.Id);
                if (match == null) toBeDeleted.Add(StringConstant);
            }
            
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    toBeDeleted.ForEach(p => p.Delete(conn, t));
                    foreach (var StringConstant in StringConstantList)
                    {
                        // Do not save empty text.
                        if (String.IsNullOrWhiteSpace(StringConstant.Text))
                        {
                            if (StringConstant.Id != null) StringConstant.Delete(conn, t); // Also remove those existing and empty.
                        }
                        else
                        {
                            StringConstant.Save(conn, t);
                        }
                    }
                    t.Commit();
                }
            }

            Refresh();
        }

        private void Refresh()
        {
            initialStringConstantList = DataModel.StringConstant.LoadAll().ToList();
            StringConstantList = new ObservableCollection<DataModel.StringConstant>(initialStringConstantList.Select(p => p.Copy()));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
