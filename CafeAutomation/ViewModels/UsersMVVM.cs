using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.Models;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;

namespace CafeAutomation.ViewModels
{
    internal class UsersMVVM : BaseVM
    {
        private Users selectedUser;
        private ObservableCollection<Users> users = new();

        public ObservableCollection<Users> Users
        {
            get => users;
            set
            {
                users = value;
                Signal();
            }
        }

        public Users SelectedUser
        {
            get => selectedUser;
            set
            {
                selectedUser = value;
                Signal();
            }
        }

        public CommandMvvm AddUser { get; }
        public CommandMvvm UpdateUser { get; }
        public CommandMvvm RemoveUser { get; }

        public UsersMVVM()
        {
            LoadDataAsync();

            AddUser = new CommandMvvm(_ =>
            {
                var newUser = new Users
                {
                    EmployeeID = 1,
                    Username = "admin",
                    Password = "12345",
                    Role = "admin"
                };

                if (UsersDB.GetDb().Insert(newUser))
                {
                    LoadDataAsync();
                    SelectedUser = newUser;
                }
            }, _ => true);


            UpdateUser = new CommandMvvm(async (_) =>
            {
                if (SelectedUser != null && await UsersDB.GetDb().UpdateAsync(SelectedUser))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedUser != null);

            RemoveUser = new CommandMvvm(async (_) =>
            {
                if (SelectedUser != null && await UsersDB.GetDb().DeleteAsync(SelectedUser))
                {
                    MessageBox.Show("Пользователь удалён");
                    await LoadDataAsync();
                }
            }, (_) => SelectedUser != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await UsersDB.GetDb().SelectAllAsync();
            Users = new ObservableCollection<Users>(data);
        }
    }
}