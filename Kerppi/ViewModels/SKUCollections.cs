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
    class SKUCollections : INotifyPropertyChanged, Refreshable
    {
        private DataModel.SKUCollection _currentSKUCollection = new DataModel.SKUCollection();
        public DataModel.SKUCollection CurrentSKUCollection { get { return _currentSKUCollection; } set { _currentSKUCollection = value; NotifyPropertyChanged(() => CurrentSKUCollection); } }
        public SKUHandler SKUHandlerInstance { get { return SKUHandler.Instance; } }

        public SKUCollections()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                CurrentSKUCollection = new DataModel.SKUCollection();
                Refresh();
            }
        }

        public void SaveCurrentSKUCollection()
        {
            DataModel.SKUCollection match = SKUHandlerInstance.SKUCollectionList.FirstOrDefault(c => c.Code == CurrentSKUCollection.Code);
            // The user is (visually) updating based on SKUCollection code, not database id.
            CurrentSKUCollection.Id = match == null ? null : match.Id;
            bool noNewLinks = (CurrentSKUCollection != null && match != null) &&
                (CurrentSKUCollection.SKUs.Count == match.SKUs.Count) &&
                (CurrentSKUCollection.SKUs.Zip(match.SKUs, (a, b) => a.Id == b.Id).All(boolVal => boolVal));
            CurrentSKUCollection.Save(noNewLinks);
            SKUHandlerInstance.SKUCollectionList = new ObservableCollection<DataModel.SKUCollection>(DataModel.SKUCollection.LoadAll());
        }

        public void RemoveSKUCollection(DataModel.SKUCollection SKUCollection)
        {
            SKUCollection.Delete();
            SKUHandlerInstance.SKUCollectionList.Remove(SKUCollection);
        }

        public void AddToCurrentSKUCollection(List<DataModel.SKU> skus)
        {
            skus.AddRange(CurrentSKUCollection.SKUs);
            var skusNew = new List<DataModel.SKU>();
            foreach (var sku in skus)
            {
                if (skusNew.FirstOrDefault(f => f.Id == sku.Id) == null) skusNew.Add(sku);
            }

            var newSKUCollection = CurrentSKUCollection.Copy();
            newSKUCollection.SKUs = skusNew;
            CurrentSKUCollection = newSKUCollection;
        }

        public void RemoveFromCurrentSKUCollection(DataModel.SKU sku)
        {
            var newSKUCollection = CurrentSKUCollection.Copy();
            int matchIndex = newSKUCollection.SKUs.FindIndex(s => s.Id == sku.Id);
            newSKUCollection.SKUs.RemoveAt(newSKUCollection.SKUs.FindIndex(s => s.Id == sku.Id));
            CurrentSKUCollection = newSKUCollection;
        }

        public void Refresh()
        {
            // If SKUs have been removed, list needs updating.
            SKUHandlerInstance.ReloadSKUCollections();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
