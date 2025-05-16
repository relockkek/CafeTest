using System.Windows;
using CafeAutomation.ViewModels;

namespace CafeAutomation.Views
{
    public partial class LoginView : Window
    {
        private LoginMVVM viewModel;

        public LoginView()
        {
            InitializeComponent();
            viewModel = new LoginMVVM();
            DataContext = viewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Передаём пароль из PasswordBox в ViewModel
            viewModel.Password = txtPassword.Password;

            if (viewModel.LoginCommand.CanExecute(null))
            {
                viewModel.LoginCommand.Execute(null);
            }
        }
    }
}