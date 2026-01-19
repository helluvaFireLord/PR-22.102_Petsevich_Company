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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using PR_22._102_Petsevich_Company.Model;

namespace PR_22._102_Petsevich_Company.Pages
{
    public partial class ProductsPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();

        public ProductsPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsGrid.ItemsSource = db.Products.ToList();
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var product = ProductsGrid.SelectedItem as Products;

            if (product != null)
            {
                var bom = db.BillOfMaterials
                            .Where(b => b.ProductID == product.ProductID)
                            .Include("Materials")
                            .ToList();

                BOMGrid.ItemsSource = bom;
            }
        }
    }
}