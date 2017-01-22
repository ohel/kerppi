/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kerppi.Views
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        public PrintPreview(A4Print print)
        {
            InitializeComponent();

            double margin = 20;
            Double.TryParse(DBHandler.QueryMisc("PrintMargin"), out margin);
            framePrintableArea.Margin = new Thickness(margin);
            framePrintableArea.Content = print;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                using (new WaitCursor())
                {
                    System.Printing.PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / viewBoxPrintPreview.ActualWidth, capabilities.PageImageableArea.ExtentHeight / viewBoxPrintPreview.ActualHeight);
                    viewBoxPrintPreview.LayoutTransform = new ScaleTransform(scale, scale); // This fills the whole printable area on the print.

                    Size printAreaSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                    viewBoxPrintPreview.Measure(printAreaSize);
                    viewBoxPrintPreview.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), printAreaSize));

                    printDialog.PrintVisual(viewBoxPrintPreview, "Kerppi-tuloste");
                    viewBoxPrintPreview.LayoutTransform = null;
                }
                this.Close();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tulosteen alatunnistetta, oletusmarginaaleja ja taustalogon näkyvyyttä voi muuttaa päävalikosta lisätoimintojen alta." + Environment.NewLine +
                "Voit muokata asiakkaan henkilötunnusta esikatseluikkunassa sitä klikkaamalla. Muokkaus vaikuttaa vain tulosteeseen.",
                "Ohjeita", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
