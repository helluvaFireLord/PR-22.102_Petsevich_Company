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
    public partial class MaterialsPage : Page
    {
        CompanyDBEntities db = new CompanyDBEntities();

        public MaterialsPage()
        {
            InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            var materials = db.Materials
                              .Include("Units")
                              .ToList();

            DataContext = materials;
        }
    }
}