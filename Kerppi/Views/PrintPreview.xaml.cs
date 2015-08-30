using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
