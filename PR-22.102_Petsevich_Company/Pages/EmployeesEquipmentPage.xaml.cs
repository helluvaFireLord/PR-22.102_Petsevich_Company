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
    public partial class EmployeesEquipmentPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();

        private Employees selectedEmployee;
        private Equipments selectedEquipment;
        private OrderAssignments selectedAssignment;

        public EmployeesEquipmentPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            PositionBox.ItemsSource = db.Positions.ToList();
            DepartmentBox.ItemsSource = db.Departament.ToList();
            EmployeesGrid.ItemsSource = db.Employees.ToList();

            StatusBox.ItemsSource = db.StatusesOfEquipment.ToList();
            EquipmentsGrid.ItemsSource = db.Equipments.ToList();

            OrderBox.ItemsSource = db.ProductionOrder.ToList();
            EmployeeAssignBox.ItemsSource = db.Employees.ToList();
            EquipmentAssignBox.ItemsSource = db.Equipments.ToList();
            AssignmentsGrid.ItemsSource = db.OrderAssignments.ToList();
        }
        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SurnameBox.Text) ||
                    string.IsNullOrWhiteSpace(NameBox.Text) ||
                    PositionBox.SelectedItem == null ||
                    DepartmentBox.SelectedItem == null ||
                    !decimal.TryParse(SalaryBox.Text, out decimal salary))
                {
                    MessageBox.Show("Заполните все поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedEmployee == null)
                {
                    selectedEmployee = new Employees();
                    db.Employees.Add(selectedEmployee);
                }

                selectedEmployee.Surname = SurnameBox.Text;
                selectedEmployee.Name = NameBox.Text;
                selectedEmployee.MiddleName = MiddleNameBox.Text;
                selectedEmployee.PositionID = ((Positions)PositionBox.SelectedItem).PositionID;
                selectedEmployee.DepartmentID = ((Departament)DepartmentBox.SelectedItem).DepartamentID;
                selectedEmployee.Salary = salary;

                db.SaveChanges();
                LoadData();
                ResetEmployeeForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сотрудника: {ex.Message}");
            }
        }
        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEmployee != null)
            {
                db.Employees.Remove(selectedEmployee);
                db.SaveChanges();
                LoadData();
                ResetEmployeeForm();
            }
        }
        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedEmployee = EmployeesGrid.SelectedItem as Employees;
            if (selectedEmployee != null)
            {
                EmployeeGroup.Header = "Редактирование сотрудника";
                DeleteEmployee.IsEnabled = true;

                SurnameBox.Text = selectedEmployee.Surname;
                NameBox.Text = selectedEmployee.Name;
                MiddleNameBox.Text = selectedEmployee.MiddleName;
                PositionBox.SelectedItem = selectedEmployee.Positions;
                DepartmentBox.SelectedItem = selectedEmployee.Departament;
                SalaryBox.Text = selectedEmployee.Salary.ToString();
            }
            else
            {
                ResetEmployeeForm();
            }
        }
        private void ResetEmployeeForm()
        {
            EmployeeGroup.Header = "Добавление сотрудника";
            DeleteEmployee.IsEnabled = false;
            selectedEmployee = null;

            SurnameBox.Text = "";
            NameBox.Text = "";
            MiddleNameBox.Text = "";
            PositionBox.SelectedIndex = -1;
            DepartmentBox.SelectedIndex = -1;
            SalaryBox.Text = "";
        }
        private void SaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EquipmentNameBox.Text) ||
                    string.IsNullOrWhiteSpace(InventoryNumberBox.Text) ||
                    StatusBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingEquipment = db.Equipments
                    .FirstOrDefault(eq => eq.InventoryNumber == InventoryNumberBox.Text);

                if (existingEquipment != null && (selectedEquipment == null || existingEquipment.EquipmentID != selectedEquipment.EquipmentID))
                {
                    MessageBox.Show("Оборудование с таким инвентарным номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedEquipment == null)
                {
                    selectedEquipment = new Equipments();
                    db.Equipments.Add(selectedEquipment);
                }

                selectedEquipment.Name = EquipmentNameBox.Text;
                selectedEquipment.InventoryNumber = InventoryNumberBox.Text;
                selectedEquipment.StatusID = ((StatusesOfEquipment)StatusBox.SelectedItem).StatusID;

                db.SaveChanges();
                LoadData();
                ResetEquipmentForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEquipment != null)
            {
                db.Equipments.Remove(selectedEquipment);
                db.SaveChanges();
                LoadData();
                ResetEquipmentForm();
            }
        }
        private void EquipmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedEquipment = EquipmentsGrid.SelectedItem as Equipments;
            if (selectedEquipment != null)
            {
                EquipmentGroup.Header = "Редактирование оборудования";
                DeleteEquipment.IsEnabled = true;

                EquipmentNameBox.Text = selectedEquipment.Name;
                InventoryNumberBox.Text = selectedEquipment.InventoryNumber;
                StatusBox.SelectedItem = selectedEquipment.StatusesOfEquipment;
            }
            else
            {
                ResetEquipmentForm();
            }
        }
        private void ResetEquipmentForm()
        {
            EquipmentGroup.Header = "Добавление оборудования";
            DeleteEquipment.IsEnabled = false;
            selectedEquipment = null;

            EquipmentNameBox.Text = "";
            InventoryNumberBox.Text = "";
            StatusBox.SelectedIndex = -1;
        }

        private void AssignmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedAssignment = AssignmentsGrid.SelectedItem as OrderAssignments;
            if (selectedAssignment != null)
            {
                AssignmentGroup.Header = "Редактирование назначения";
                DeleteAssignment.IsEnabled = true;

                OrderBox.SelectedItem = selectedAssignment.ProductionOrder;
                EmployeeAssignBox.SelectedItem = selectedAssignment.Employees;
                EquipmentAssignBox.SelectedItem = selectedAssignment.Equipments;
            }
            else
            {
                ResetAssignmentForm();
            }
        }
        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrderBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EmployeeAssignBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EquipmentAssignBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите оборудование!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedAssignment == null)
                {
                    selectedAssignment = new OrderAssignments();
                    db.OrderAssignments.Add(selectedAssignment);
                }

                selectedAssignment.OrderID = ((ProductionOrder)OrderBox.SelectedItem).OrderID;
                selectedAssignment.EmployeeID = ((Employees)EmployeeAssignBox.SelectedItem).EmployeeID;
                selectedAssignment.EquipmentID = ((Equipments)EquipmentAssignBox.SelectedItem).EquipmentID;

                db.SaveChanges();
                LoadData();
                ResetAssignmentForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении назначения: {ex.Message}");
            }
        }
        private void DeleteAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAssignment != null)
            {
                db.OrderAssignments.Remove(selectedAssignment);
                db.SaveChanges();
                LoadData();
                ResetAssignmentForm();
            }
        }
        private void ResetAssignmentForm()
        {
            AssignmentGroup.Header = "Добавление назначения";
            DeleteAssignment.IsEnabled = false;
            selectedAssignment = null;

            OrderBox.SelectedIndex = -1;
            EmployeeAssignBox.SelectedIndex = -1;
            EquipmentAssignBox.SelectedIndex = -1;
        }
    }
}