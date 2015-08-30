using Kerppi.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
