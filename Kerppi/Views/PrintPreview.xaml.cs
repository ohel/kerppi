/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

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
            framePrintableArea2.Margin = framePrintableArea.Margin;
            framePrintableArea.Content = print;
            framePrintableArea2.Content = print.GetSecondPage();
            this.DataContext = print;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private PageContent CreateNewPage(UIElement uie, Size printAreaSize)
        {
            var fixedPage = new FixedPage();
            fixedPage.Width = printAreaSize.Width;
            fixedPage.Height = printAreaSize.Height;
            fixedPage.Children.Add(uie);
            fixedPage.Measure(printAreaSize);
            fixedPage.Arrange(new Rect(new Point(), printAreaSize));
            fixedPage.UpdateLayout();

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);
            return pageContent;
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                using (new WaitCursor())
                {
                    System.Printing.PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                    Size printAreaSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    var document = new FixedDocument();
                    document.DocumentPaginator.PageSize = printAreaSize;

                    // This fills the whole printable area on the print.
                    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / viewBoxPrintPreview.ActualWidth, capabilities.PageImageableArea.ExtentHeight / viewBoxPrintPreview.ActualHeight);
                    viewBoxPrintPreview.LayoutTransform = new ScaleTransform(scale, scale);
                    borderFirstPage.Child = null;
                    document.Pages.Add(CreateNewPage(viewBoxPrintPreview, printAreaSize));

                    if (((A4Print)DataContext).TwoPagePrint)
                    {
                        viewBoxPrintPreview2.LayoutTransform = new ScaleTransform(scale, scale);
                        borderSecondPage.Child = null;
                        document.Pages.Add(CreateNewPage(viewBoxPrintPreview2, printAreaSize));
                    }

                    printDialog.PrintDocument(document.DocumentPaginator, "Kerppi-tuloste");
                }
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ((A4Print)DataContext).CheckIfTwoPagesNecessary()), DispatcherPriority.ContextIdle, null);
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
