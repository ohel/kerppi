/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Kerppi.ViewModels
{
    class SKUs : INotifyPropertyChanged
    {
        private DataModel.SKU _currentSKU = new DataModel.SKU();
        public DataModel.SKU CurrentSKU { get { return _currentSKU; } set { _currentSKU = value; NotifyPropertyChanged(() => CurrentSKU); } }
        public SKUHandler SKUHandlerInstance { get { return SKUHandler.Instance; } }

        public SKUs()
        {
            CurrentSKU = new DataModel.SKU();
        }

        public void SaveCurrentSKU()
        {
            DataModel.SKU match = SKUHandlerInstance.SKUList.FirstOrDefault(sku => sku.Code == CurrentSKU.Code);
            // The user is (visually) updating based on SKU code, not database id.
            CurrentSKU.Id = match != null ? match.Id : null;
            CurrentSKU.Save();
            SKUHandlerInstance.SKUList = new ObservableCollection<DataModel.SKU>(DataModel.SKU.LoadAll());
        }

        public void RemoveSKU(DataModel.SKU sku)
        {
            SKUHandlerInstance.RemoveSKU(sku);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
