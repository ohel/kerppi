/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Kerppi.ViewModels
{
    sealed class SKUHandler: INotifyPropertyChanged
    {
        private static object syncRoot = new Object();
        private static volatile SKUHandler instance;
        public static SKUHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null) instance = new SKUHandler();
                    }
                }
                return instance;
            }
        }

        private ObservableCollection<DataModel.SKU> _SKUList = new ObservableCollection<DataModel.SKU>();
        private ObservableCollection<DataModel.InvoiceRow> _invoiceables = new ObservableCollection<DataModel.InvoiceRow>();
        public ObservableCollection<DataModel.SKU> SKUList { get { return _SKUList; } set {
            _SKUList = value;
            NotifyPropertyChanged(() => SKUList);
            RefreshInvoiceables();
        } }
        public ObservableCollection<DataModel.InvoiceRow> Invoiceables { get { return _invoiceables; } private set { _invoiceables = value; NotifyPropertyChanged(() => Invoiceables); } }

        private SKUHandler()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                ReloadSKUs();
            }
        }

        private void RefreshInvoiceables()
        {
            List<DataModel.InvoiceRow> i = new List<DataModel.InvoiceRow>(_SKUList);
            Invoiceables = new ObservableCollection<DataModel.InvoiceRow>(i);
        }

        public void ReloadSKUs()
        {
            SKUList = new ObservableCollection<DataModel.SKU>(DataModel.SKU.LoadAll());
        }

        public void RemoveSKU(DataModel.SKU sku)
        {
            sku.Delete();
            SKUList.Remove(SKUList.First(s => s.Id == sku.Id));
            NotifyPropertyChanged(() => SKUList);
        }

        public void Export(string filename)
        {
            DataModel.SKU.SaveAllToFile(filename, SKUList);
        }

        public void Import(string filename)
        {
            var skus = DataModel.SKU.LoadAllFromFile(filename);
            foreach (var sku in skus)
            {
                var match = SKUList.FirstOrDefault(s => s.Code == sku.Code);
                if (match != null) sku.Id = match.Id; // Update existing ones.
                sku.Save();
            }
            SKUList = new ObservableCollection<DataModel.SKU>(skus);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
