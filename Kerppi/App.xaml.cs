/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kerppi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EventManager.RegisterClassHandler(typeof(DatePicker),
                DatePicker.PreviewKeyDownEvent,
                new KeyEventHandler(DatePicker_PreviewKeyDown));
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dp = sender as DatePicker;
            if (dp == null) return;

            if (e.Key == Key.Return)
            {
                e.Handled = true;
                dp.IsDropDownOpen = !dp.IsDropDownOpen;
                return;
            }

            if (!dp.SelectedDate.HasValue || dp.IsDropDownOpen) return;

            var date = dp.SelectedDate.Value;
            if (e.Key == Key.Up)
            {
                e.Handled = true;
                dp.SetValue(DatePicker.SelectedDateProperty, date.AddDays(1));
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                dp.SetValue(DatePicker.SelectedDateProperty, date.AddDays(-1));
            }
        }
    }
}
