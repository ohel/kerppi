/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Kerppi.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Kerppi.Views
{
    public class A4Print : Page
    {
        private string _printFooter;
        private double _printLogoOpacity;
        public string PrintFooter { get { return _printFooter; } set { _printFooter = value; NotifyPropertyChanged(() => PrintFooter); } }
        public double PrintLogoOpacity { get { return _printLogoOpacity; } set { _printLogoOpacity = value; NotifyPropertyChanged(() => PrintLogoOpacity); } }

        public A4Print(object dataContext)
        {
            this.DataContext = dataContext;
            PrintFooter = DBHandler.QueryMisc("PrintFooter");
            double opacity = 1.0;
            Double.TryParse(DBHandler.QueryMisc("PrintLogoOpacity"), out opacity);
            PrintLogoOpacity = opacity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyHelper.GetPropertyName(property)));
        }
    }
}
