/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kerppi.ViewModels
{
    public class DeeplyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public DeeplyObservableCollection()
            : base()
        {
            CollectionChanged += new NotifyCollectionChangedEventHandler(DeeplyObservableCollection_CollectionChanged);
        }

        void DeeplyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(DeeplyObservableCollection_ItemPropertyChanged);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(DeeplyObservableCollection_ItemPropertyChanged);
                }
            }
        }

        void DeeplyObservableCollection_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(a);
        }
    }
}
