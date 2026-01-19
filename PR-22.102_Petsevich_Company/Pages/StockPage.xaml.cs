using PR_22._102_Petsevich_Company.Model;
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

namespace PR_22._102_Petsevich_Company.Pages
{
    public partial class StockPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();

        public StockPage()
        {
            InitializeComponent();
            LoadStock();
        }

        private void LoadStock()
        {
            StockGrid.ItemsSource = db.MaterialStocks
                .Include(s => s.Materials)
                .Include(s => s.Materials.Units)
                .ToList();
        }
    }
}