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
    public partial class ProductionOrdersPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();
        private ProductionOrder selectedOrder = null;

        public ProductionOrdersPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            ClientBox.ItemsSource = db.Clients.ToList();
            ProductBox.ItemsSource = db.Products.ToList();
            StatusBox.ItemsSource = db.StatusesOfOrder.ToList();

            OrdersGrid.ItemsSource = db.ProductionOrder
                .Include(o => o.Clients)
                .Include(o => o.Products)
                .Include(o => o.StatusesOfOrder)
                .ToList();

            ClearForm();
        }
        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedOrder = OrdersGrid.SelectedItem as ProductionOrder;

            if (selectedOrder != null)
            {
                OrderGroup.Header = "Редактирование заказа";
                DeleteOrder.IsEnabled = true;

                ClientBox.SelectedItem = db.Clients.Find(selectedOrder.ClientID);
                ProductBox.SelectedItem = db.Products.Find(selectedOrder.ProductID);
                QuantityBox.Text = selectedOrder.Quantity.ToString();
                StartDatePicker.SelectedDate = selectedOrder.StartDate;
                EndDatePicker.SelectedDate = selectedOrder.EndDate;
                StatusBox.SelectedItem = db.StatusesOfOrder.Find(selectedOrder.StatusID);
            }
            else
            {
                ClearForm();
            }
        }
        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ClientBox.SelectedItem == null || ProductBox.SelectedItem == null || StatusBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите клиента, продукцию и статус заказа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!StartDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите дату начала заказа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!EndDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите дату окончания заказа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EndDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
                {
                    MessageBox.Show("Дата окончания не может быть раньше даты начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ProductionOrder order;

                if (selectedOrder == null)
                {
                    order = new ProductionOrder();
                    db.ProductionOrder.Add(order);
                }
                else
                {
                    order = selectedOrder;

                    var oldBOM = db.BillOfMaterials.Where(b => b.ProductID == order.ProductID).ToList();
                    foreach (var bom in oldBOM)
                    {
                        var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == bom.MaterialID);
                        if (stock != null)
                            stock.Quantity += bom.Quantity * order.Quantity;
                    }
                }
                order.ClientID = ((Clients)ClientBox.SelectedItem).ClientID;
                order.ProductID = ((Products)ProductBox.SelectedItem).ProductID;
                order.Quantity = quantity;
                order.StartDate = StartDatePicker.SelectedDate.Value;
                order.EndDate = EndDatePicker.SelectedDate;
                order.StatusID = ((StatusesOfOrder)StatusBox.SelectedItem).StatusID;

                var bomItems = db.BillOfMaterials
                                 .Where(b => b.ProductID == order.ProductID)
                                 .ToList();

                foreach (var bom in bomItems)
                {
                    var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == bom.MaterialID);
                    if (stock != null)
                        stock.Quantity -= bom.Quantity * order.Quantity;
                }

                db.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null) return;

            var result = MessageBox.Show("Вы действительно хотите удалить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var bomItems = db.BillOfMaterials
                                 .Where(b => b.ProductID == selectedOrder.ProductID)
                                 .ToList();

                foreach (var bom in bomItems)
                {
                    var stock = db.MaterialStocks.FirstOrDefault(s => s.MaterialID == bom.MaterialID);
                    if (stock != null)
                        stock.Quantity += bom.Quantity * selectedOrder.Quantity;
                }

                db.ProductionOrder.Remove(selectedOrder);
                db.SaveChanges();
                LoadData();
            }
        }
        private void ClearForm()
        {
            selectedOrder = null;
            OrderGroup.Header = "Добавление нового заказа";
            DeleteOrder.IsEnabled = false;

            ClientBox.SelectedIndex = -1;
            ProductBox.SelectedIndex = -1;
            QuantityBox.Text = "";
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            StatusBox.SelectedIndex = -1;
        }
    }
}