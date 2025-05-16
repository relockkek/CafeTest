using System.Windows;
using System.Windows.Controls;
using CafeAutomation.Views;
using CafeAutomation.Models;
using CafeAutomation.DB;
using System.Threading.Tasks;

namespace CafeAutomation.Views
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
        }

        private async void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditEmployeeDialog(null);
            if (dialog.ShowDialog() == true)
            {
                if (EmployeesDB.GetDb().Insert(dialog.Employee))
                await ((CafeAutomation.ViewModels.EmployeesMVVM)DataContext).LoadDataAsync();

            }
        }

        private async void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (((CafeAutomation.ViewModels.EmployeesMVVM)DataContext).SelectedEmployee is Employees emp)
            {
                var dialog = new EditEmployeeDialog(emp);
                if (dialog.ShowDialog() == true)
                {
                    await EmployeesDB.GetDb().UpdateAsync(dialog.Employee);
                    await ((CafeAutomation.ViewModels.EmployeesMVVM)DataContext).LoadDataAsync();

                }
            }
        }
    }
}