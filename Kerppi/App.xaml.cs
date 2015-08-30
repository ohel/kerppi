using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
