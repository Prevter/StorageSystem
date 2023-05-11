using StorageSystem.Assets;
using StorageSystem.Common;
using System.Collections.Generic;

namespace StorageSystem.MVVM
{
	public sealed class NavigationPage : BaseViewModel
	{
		private string _filename, _title, _icon;
		private bool _isSelected;

		public string Filename
		{
			get => _filename;
			set => SetField(ref _filename, value);
		}

		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}

		public bool IsSelected
		{
			get => _isSelected;
			set => SetField(ref _isSelected, value);
		}

		public string Icon
		{
			get => _icon;
			set => SetField(ref _icon, value);
		}
	}

	public sealed class MainPageViewModel : BaseViewModel
	{
		public List<NavigationPage> Pages { get; set; }

		private string _currentPage;
		private RelayCommand _logoutCommand, _selectPageCommand;

		public string Username
		{
			get => DatabaseController.Username;
		}

		public string CurrentPage
		{
			get => _currentPage;
			set => SetField(ref _currentPage, value);
		}

		public RelayCommand LogoutCommand
		{
			get => _logoutCommand ??= new RelayCommand(obj =>
			{
				// TODO: Disconnect from database
				App.ChangePage("../Pages/LoginPage.xaml", "Вхід");
			});
		}

		public RelayCommand SelectPageCommand
		{
			get => _selectPageCommand ??= new RelayCommand(obj =>
			{
				if (obj is NavigationPage page)
				{
					foreach (var p in Pages)
						p.IsSelected = p == page;

					CurrentPage = page.Filename;
					App.ChangeTitle(page.Title);
				}
			});
		}

		public MainPageViewModel()
		{
			// TODO: Initialize pages according to permissions
			Pages = new()
			{
				new()
				{
					Filename = "../Pages/ProductsPage.xaml",
					Title = "Продукти",
					Icon = PageIcons.Product,
					IsSelected = true
				},
				new()
				{
					Filename = "../Pages/ManufacturersPage.xaml",
					Title = "Виробники",
					Icon = PageIcons.Manufacturer
				},
				new()
				{
					Filename = "../Pages/ShopsPage.xaml",
					Title = "Магазини",
					Icon = PageIcons.Shop
				},
				new()
				{
					Filename = "../Pages/StoragesPage.xaml",
					Title = "Склади",
					Icon = PageIcons.Storage
				},
				new()
				{
					Filename = "../Pages/ReceiptsPage.xaml",
					Title = "Звіти",
					Icon = PageIcons.Receipt
				},
			};

			SelectPageCommand.Execute(Pages[0]);
		}
	}
}
