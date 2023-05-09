using System.Windows;

namespace StorageSystem.MVVM
{
	public sealed class LoginPageViewModel : BaseViewModel
	{
		private string _username = "";
		private string _password = "";

		private RelayCommand _loginCommand;

		public string Username
		{
			get => _username;
			set => SetField(ref _username, value);
		}

		public string Password
		{
			get => _password;
			set => SetField(ref _password, value);
		}

		public LoginPageViewModel()
		{
			Username = "";
			Password = "";
		}

		public RelayCommand LoginCommand
		{
			get => _loginCommand ??= new RelayCommand(obj =>
			{
				// TODO: Connect to database and check credentials
				if (Username == "admin" && Password == "admin")
				{
					App.ChangePage("../Pages/MainPage.xaml", "Головна");
				}
				else
				{
					MessageBox.Show("Невірний пароль чи логін", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			});
		}
	}
}
