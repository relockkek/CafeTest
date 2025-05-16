using System.Windows;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class EditEmployeeDialog : Window
    {
        public Employees Employee { get; private set; }

        public EditEmployeeDialog(Employees emp)
        {
            InitializeComponent();
            Employee = emp != null ? new Employees
            {
                ID = emp.ID,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Patronymic = emp.Patronymic,
                Phone = emp.Phone,
                Position = emp.Position,
                Salary = emp.Salary,
                HireDate = emp.HireDate
            } : new Employees();

            DataContext = Employee;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
