using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class EmployeesMVVM : BaseVM
    {
        private Employees selectedEmployee;
        private ObservableCollection<Employees> employees = new();

        public ObservableCollection<Employees> Employees
        {
            get => employees;
            set
            {
                employees = value;
                Signal();
            }
        }

        public Employees SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                Signal();
            }
        }

        public CommandMvvm AddEmployee { get; }
        public CommandMvvm UpdateEmployee { get; }
        public CommandMvvm RemoveEmployee { get; }

        public EmployeesMVVM()
        {
            LoadDataAsync();

            AddEmployee = new CommandMvvm((_) =>
            {
                var emp = new Employees
                {
                    FirstName = "Имя",
                    LastName = "Фамилия"
                };
                if (EmployeesDB.GetDb().Insert(emp))
                {
                    LoadDataAsync();
                    SelectedEmployee = emp;
                }
            }, (_) => true);

            UpdateEmployee = new CommandMvvm(async (_) =>
            {
                if (SelectedEmployee != null && await EmployeesDB.GetDb().UpdateAsync(SelectedEmployee))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedEmployee != null);

            RemoveEmployee = new CommandMvvm(async (_) =>
            {
                if (SelectedEmployee != null && await EmployeesDB.GetDb().DeleteAsync(SelectedEmployee))
                {
                    MessageBox.Show("Удалён");
                    await LoadDataAsync();
                }
            }, (_) => SelectedEmployee != null);
        }

        public async Task LoadDataAsync()
        {
            var data = await EmployeesDB.GetDb().SelectAllAsync();
            Employees = new ObservableCollection<Employees>(data);
        }
    }
}