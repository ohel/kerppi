/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Globalization;
using System.Windows.Data;

namespace Kerppi.ViewModels
{
    class SingleLineStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int p = 0;
            int.TryParse((string)parameter, out p);
            string unbroken = ((string)value).Replace(Environment.NewLine, " ");
            return p == 0 ? unbroken : (unbroken.Substring(0, Math.Min(unbroken.Length, p)) + (unbroken.Length < p + 1 ? "" : "…"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
