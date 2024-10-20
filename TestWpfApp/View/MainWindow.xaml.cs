using GrandSmetaReader.Services;
using GrandSmetaReader.ViewModel;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //для чтения кодировки win1251
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {         
                DataContext = new SmetaViewModel("smeta.xml");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "smetaXLSX.xlsx";
            SmetaExporter.ExportToExcel((DataContext as SmetaViewModel).Chapters, fileName);
            MessageBox.Show(string.Format("Смета выгружена {0}/{1}", Directory.GetCurrentDirectory(), fileName));
        }
    }
}
