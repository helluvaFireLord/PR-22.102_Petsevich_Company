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
using PR_22._102_Petsevich_Company.Pages;

namespace PR_22._102_Petsevich_Company
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new MaterialsPage());
        }

        private void Materials_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MaterialsPage());
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        private void Purchases_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PurchasesPage());
        }

        private void Stock_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StockPage());
        }
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductionOrdersPage());
        }
        private void EmployeesEquipment_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesEquipmentPage());
        }
    }
}