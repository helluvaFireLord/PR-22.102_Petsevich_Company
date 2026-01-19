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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace PR_22._102_Petsevich_Company.Pages
{
    public partial class PurchasesPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();
        private Purchase selectedPurchase = null;

        public PurchasesPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                SupplierBox.ItemsSource = db.Suppliers.ToList();
                MaterialBox.ItemsSource = db.Materials.ToList();

                PurchasesGrid.ItemsSource = db.Purchase
                    .Include(p => p.Materials)
                    .Include(p => p.Suppliers)
                    .OrderByDescending(p => p.PurchaseDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PurchasesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPurchase = PurchasesGrid.SelectedItem as Purchase;

            if (selectedPurchase != null)
            {
                PurchaseGroup.Header = "Редактирование поставки";
                DeletePurchase.IsEnabled = true;

                SupplierBox.SelectedItem = db.Suppliers.Find(selectedPurchase.SupplierID);
                MaterialBox.SelectedItem = db.Materials.Find(selectedPurchase.MaterialID);
                QuantityBox.Text = selectedPurchase.Quantity.ToString();
                PriceBox.Text = selectedPurchase.Price.ToString();
            }
            else
            {
                PurchaseGroup.Header = "Добавление новой поставки";
                DeletePurchase.IsEnabled = false;

                SupplierBox.SelectedIndex = -1;
                MaterialBox.SelectedIndex = -1;
                QuantityBox.Text = "";
                PriceBox.Text = "";
            }
        }
        private void SavePurchase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SupplierBox.SelectedItem == null)
                    throw new Exception("Выберите поставщика.");
                if (MaterialBox.SelectedItem == null)
                    throw new Exception("Выберите материал.");
                if (!decimal.TryParse(QuantityBox.Text, out decimal quantity) || quantity <= 0)
                    throw new Exception("Введите корректное количество.");
                if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
                    throw new Exception("Введите корректную цену.");

                if (selectedPurchase == null)
                {
                    var purchase = new Purchase
                    {
                        SupplierID = ((Suppliers)SupplierBox.SelectedItem).SupplierID,
                        MaterialID = ((Materials)MaterialBox.SelectedItem).MaterialID,
                        Quantity = quantity,
                        Price = price,
                        PurchaseDate = DateTime.Now
                    };

                    db.Purchase.Add(purchase);

                    var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == purchase.MaterialID);
                    if (stock == null)
                    {
                        stock = new MaterialStocks
                        {
                            MaterialID = purchase.MaterialID,
                            Quantity = purchase.Quantity
                        };
                        db.MaterialStocks.Add(stock);
                    }
                    else
                    {
                        stock.Quantity += purchase.Quantity;
                    }
                }
                else
                {
                    selectedPurchase.SupplierID = ((Suppliers)SupplierBox.SelectedItem).SupplierID;

                    if (selectedPurchase.MaterialID != ((Materials)MaterialBox.SelectedItem).MaterialID)
                    {
                        var oldStock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == selectedPurchase.MaterialID);
                        if (oldStock != null)
                            oldStock.Quantity -= selectedPurchase.Quantity;

                        selectedPurchase.MaterialID = ((Materials)MaterialBox.SelectedItem).MaterialID;

                        var newStock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == selectedPurchase.MaterialID);
                        if (newStock == null)
                        {
                            newStock = new MaterialStocks
                            {
                                MaterialID = selectedPurchase.MaterialID,
                                Quantity = selectedPurchase.Quantity
                            };
                            db.MaterialStocks.Add(newStock);
                        }
                        else
                        {
                            newStock.Quantity += selectedPurchase.Quantity;
                        }
                    }
                    else
                    {
                        var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == selectedPurchase.MaterialID);
                        if (stock != null)
                        {
                            stock.Quantity -= selectedPurchase.Quantity;
                            stock.Quantity += quantity;
                        }
                    }

                    selectedPurchase.Quantity = quantity;
                    selectedPurchase.Price = price;
                }

                db.SaveChanges();
                selectedPurchase = null;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeletePurchase_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPurchase == null)
                return;

            var result = MessageBox.Show("Удалить выбранную поставку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == selectedPurchase.MaterialID);
                if (stock != null)
                    stock.Quantity -= selectedPurchase.Quantity;

                db.Purchase.Remove(selectedPurchase);
                db.SaveChanges();

                selectedPurchase = null;
                LoadData();
            }
        }
    }
}