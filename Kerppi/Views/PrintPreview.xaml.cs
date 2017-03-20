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
            DataContext = print;
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
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ((A4Print)DataContext).CheckIfTwoPagesNecessary()), DispatcherPriority.ContextIdle, null);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tulosteen oletusmarginaaleja ja taustalogon näkyvyyttä voi muuttaa päävalikosta lisätoimintojen alta." + Environment.NewLine +
                "Voit määritellä alatunnisteen vakiomerkkijonona ja vaihtaa sitä Alatunniste-napista.",
                "Ohjeita", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FootersButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            button.ContextMenu = new ContextMenu();
            foreach (var footer in ((A4Print)DataContext).AvailableFooters)
            {
                var mi = new MenuItem();
                mi.Header = footer;
                mi.Click += FooterSelected;
                button.ContextMenu.Items.Add(mi);
            }
            button.ContextMenu.IsOpen = true;
            button.ContextMenu.Focus();
        }

        private void FooterSelected(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi == null) return;
            ((A4Print)framePrintableArea.Content).PrintFooter = mi.Header.ToString();
            ((A4Print)framePrintableArea2.Content).PrintFooter = mi.Header.ToString();
        }
    }
}
