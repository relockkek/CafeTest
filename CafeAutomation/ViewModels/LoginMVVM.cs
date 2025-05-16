using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.Views; 
using CafeAutomation.Models;
using System.Windows.Input;

namespace CafeAutomation.ViewModels
{
    internal class LoginMVVM : BaseVM
    {
        private string login;
        private string password;

        public string Login
        {
            get => login;
            set
            {
                login = value;
                Signal();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                Signal();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginMVVM()
        {
            if (!UsersDB.GetDb().CheckAdminExists())
            {
                var admin = new Users
                {
                    EmployeeID = 1,
                    Username = "admin",
                    Password = "12345",
                    Role = "admin"
                };

                if (UsersDB.GetDb().Insert(admin))
                {
                    MessageBox.Show("Администратор успешно создан");
                }
                else
                {
                    MessageBox.Show("Ошибка создания администратора");
                }
            }

            LoginCommand = new CommandMvvm((_) =>
            {
                var user = UsersDB.GetDb().FindByUsernameAndPassword(Login, Password);
                if (user != null)
                {
                    new MainWindow().Show();
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }, (_) => !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password));
        }
    }
}