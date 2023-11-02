using FloxelLib.MVVM;
using StorageSystem.Assets;
using StorageSystem.Common;
using System.Collections.Generic;

namespace StorageSystem.MVVM
{
	public sealed partial class NavigationPage : BaseViewModel
	{
		[UpdateProperty]
		private string _filename, _title, _icon;

		[UpdateProperty]
		private bool _isSelected;
	}

	public sealed partial class MainPageViewModel : BaseViewModel
	{
		public List<NavigationPage> Pages { get; set; }

		[UpdateProperty]
		private string _currentPage;

		public string Username
		{
			get => DatabaseController.Username;
		}

		[RelayCommand]
		public void Logout()
		{
			App.ChangePage("../Pages/LoginPage.xaml", "Вхід");
		}

		[RelayCommand]
		public void SelectPage(object obj)
		{
			if (obj is NavigationPage page)
			{
				foreach (var p in Pages)
					p.IsSelected = p == page;

				CurrentPage = page.Filename;
				App.ChangeTitle(page.Title);
			}
		}

		public MainPageViewModel()
		{
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
