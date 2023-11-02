using FloxelLib.MVVM;
using StorageSystem.Common;
using System;
using System.Windows;

using MessageBox = FloxelLib.Common.MessageBox;

namespace StorageSystem.MVVM
{
    public sealed partial class LoginPageViewModel : BaseViewModel
    {
        [UpdateProperty]
        private string _username = "", _password = "";

        [UpdateProperty]
        private string _serverAddress = AppHelpers.Settings.DataSource, _serverDatabase = AppHelpers.Settings.InitialCatalog;

        [UpdateProperty]
        private Visibility _modalVisibility = Visibility.Collapsed;

        public LoginPageViewModel()
        {
            Username = "";
            Password = "";
        }

        [RelayCommand]
        public void Login()
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
        }

        [RelayCommand]
        public void Save()
        {
            AppHelpers.Settings.DataSource = ServerAddress;
            AppHelpers.Settings.InitialCatalog = ServerDatabase;
            AppHelpers.Settings.Save();
            ModalVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        public void OpenServerEdit()
        {
            ModalVisibility = Visibility.Visible;
        }
    }
}
