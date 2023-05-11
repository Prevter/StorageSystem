using StorageSystem.Common;
using System;
using System.Windows;

namespace StorageSystem.MVVM
{
	public sealed class LoginPageViewModel : BaseViewModel
	{
		private string _username = "";
		private string _password = "";

		private string _serverAddress = AppHelpers.Settings.DataSource, _serverDatabase = AppHelpers.Settings.InitialCatalog;
		private RelayCommand _loginCommand, _saveCommand, _openServerEditCommand;

		private Visibility _modalVisibility = Visibility.Collapsed;

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

		public string ServerAddress
		{
			get => _serverAddress;
			set => SetField(ref _serverAddress, value);
		}

		public string ServerDatabase
		{
			get => _serverDatabase;
			set => SetField(ref _serverDatabase, value);
		}

		public Visibility ModalVisibility
		{
			get => _modalVisibility;
			set => SetField(ref _modalVisibility, value);
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
				try 
				{
					DatabaseController.TryConnect(Username, Password);
					App.ChangePage("../Pages/MainPage.xaml", "Головна");
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			});
		}

		public RelayCommand SaveCommand
		{
			get => _saveCommand ??= new RelayCommand(obj =>
			{
				AppHelpers.Settings.DataSource = ServerAddress;
				AppHelpers.Settings.InitialCatalog = ServerDatabase;
				AppHelpers.Settings.Save();
				ModalVisibility = Visibility.Collapsed;
			});
		}

		public RelayCommand OpenServerEditCommand
		{
			get => _openServerEditCommand ??= new RelayCommand(obj =>
			{
				ModalVisibility = Visibility.Visible;
			});
		}
	}
}
