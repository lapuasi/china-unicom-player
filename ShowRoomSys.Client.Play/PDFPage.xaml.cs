using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowRoomSys.Client.Play
{
    /// <summary>
    /// Interaction logic for PDFPage.xaml
    /// </summary>
    public partial class PDFPage : Page
    {
        float m = 1;
        public PDFPage()
        {
            InitializeComponent();
        }
        public PDFPage(string filePath)
            : this()
        {
            this.pdfView.LoadFile(filePath);
            this.pdfView.src = filePath;
            this.pdfView.setShowToolbar(false);
            this.pdfView.setShowScrollbars(false);
            this.pdfView.setLayoutMode("SinglePage");
            this.pdfView.setView("FitV");
            this.pdfView.setPageMode("none");
            this.pdfView.Show();
        }
        public void DecreaseZoom()
        {
            m--;
            this.pdfView.setZoom(10 + m);
        }

        public void IncreaseZoom()
        {
            m++;
            this.pdfView.setZoom(10 + m);
        }
        public void GONextPage()
        {
            this.pdfView.gotoNextPage();
        }
        public void GOPreviousPage()
        {
            this.pdfView.gotoPreviousPage();
        }
        public void Dispose()
        {
            this.pdfView.Dispose();
        }
        public void AdjustView()
        {
            this.pdfView.setLayoutMode("SinglePage");
            this.pdfView.setView("FitV");
            this.pdfView.setPageMode("none");
        }
    }
}
