/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Kerppi.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Kerppi.Views
{
    public abstract class A4Print : Page, INotifyPropertyChanged
    {
        private string _printFooter;
        private double _printLogoOpacity;
        private bool _twoPagePrint;
        private bool _isSecondPage;
        public string PrintFooter { get { return _printFooter; } set { _printFooter = value; NotifyPropertyChanged(() => PrintFooter); } }
        public double PrintLogoOpacity { get { return _printLogoOpacity; } set { _printLogoOpacity = value; NotifyPropertyChanged(() => PrintLogoOpacity); } }
        public bool TwoPagePrint { get { return _twoPagePrint; } set { _twoPagePrint = value; NotifyPropertyChanged(() => TwoPagePrint); NotifyPropertyChanged(() => ShowSecondPageContent); } }
        public bool IsSecondPage { get { return _isSecondPage; } private set { _isSecondPage = value; TwoPagePrint = value || TwoPagePrint; NotifyPropertyChanged(() => IsSecondPage); } }
        public bool ShowSecondPageContent { get { return IsSecondPage || !(IsSecondPage || TwoPagePrint); } }
        public IEnumerable<String> AvailableFooters { get; private set; }

        public A4Print() { }

        public A4Print(object dataContext, bool isSecondPage = false)
        {
            this.DataContext = dataContext;
            double opacity = 1.0;
            Double.TryParse(DBHandler.QueryMisc("PrintLogoOpacity"), out opacity);
            PrintLogoOpacity = opacity;
            IsSecondPage = isSecondPage;
            AvailableFooters = DataModel.StringConstant.LoadFooterStrings();
            PrintFooter = AvailableFooters.FirstOrDefault();
        }

        public abstract A4Print GetSecondPage();

        public virtual void CheckIfTwoPagesNecessary() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
