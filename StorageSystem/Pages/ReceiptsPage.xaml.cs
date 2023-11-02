using Microsoft.Reporting.WinForms;
using StorageSystem.Common;
using StorageSystem.MVVM;
using System.Data;

namespace StorageSystem.Pages
{
    /// <summary>
    /// Interaction logic for ReceiptsPage.xaml
    /// </summary>
    public partial class ReceiptsPage
    {
        public ReportViewer _reportViewer;

        public ReceiptsPage()
        {
            InitializeComponent();
            _reportViewer = new()
            {
                ProcessingMode = ProcessingMode.Local
            };

            winformsHost.Child = _reportViewer;

            var viewmodel = DataContext as ReceiptsPageViewModel;
            viewmodel.PageChanged += Viewmodel_PageChanged;
            viewmodel.SelectedTabIndex = 0;
        }

        private void Viewmodel_PageChanged(object? sender, UpdateViewEvent e)
        {
            DataTable dataTable;
            ReportDataSource reportDataSource;

            if (e.Page == ReportsPage.Products)
            {
                _reportViewer.LocalReport.ReportEmbeddedResource = "StorageSystem.Reports.ReportProducts.rdlc";
                dataTable = DatabaseController.LoadProductsReport();
                reportDataSource = new("ProductsDetailsDataset", dataTable);
            }
            else if (e.Page == ReportsPage.Shops)
            {
                _reportViewer.LocalReport.ReportEmbeddedResource = "StorageSystem.Reports.ReportShops.rdlc";
                dataTable = DatabaseController.LoadShopsReport(e.Parameters);
                reportDataSource = new("ShopInventoryDataset", dataTable);
            }
            else
            {
                _reportViewer.LocalReport.ReportEmbeddedResource = "StorageSystem.Reports.ReportStorages.rdlc";
                dataTable = DatabaseController.LoadStoragesReport(e.Parameters);
                reportDataSource = new("StorageInventoryDataset", dataTable);
            }

            _reportViewer.LocalReport.DataSources.Clear();
            _reportViewer.LocalReport.DataSources.Add(reportDataSource);

            _reportViewer.RefreshReport();
        }

    }
}
