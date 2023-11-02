using FloxelLib.MVVM;
using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace StorageSystem.MVVM
{
	public enum ReportsPage
	{
		Products,
		Shops,
		Storages
	}

	public sealed class UpdateViewEvent : EventArgs
	{
		public ReportsPage Page { get; set; }
		public string? Parameters { get; set; }
	}

	public sealed class ReceiptsPageViewModel : BaseViewModel
	{
		public event EventHandler<UpdateViewEvent>? PageChanged;

		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<StorageVM> Storages { get; set; }

		private ShopVM _selectedShop;
		private StorageVM _selectedStorage;

		private int _selectedTabIndex;

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				SetField(ref _selectedTabIndex, value);
				UpdateViewEvent args = new()
				{
					Page = (ReportsPage)value
				};
				switch (value)
				{
					case 1:
						args.Parameters = _selectedShop?.Id;
						break;
					case 2:
						args.Parameters = _selectedStorage?.Id;
						break;
				}
				PageChanged?.Invoke(this, args);
			}
		}

		public ShopVM SelectedShop
		{
			get => _selectedShop;
			set
			{
				SetField(ref _selectedShop, value);
				PageChanged?.Invoke(this, new()
				{
					Page = ReportsPage.Shops,
					Parameters = _selectedShop.Id
				});
			}
		}

		public StorageVM SelectedStorage
		{
			get => _selectedStorage;
			set
			{
				SetField(ref _selectedStorage, value);
				PageChanged?.Invoke(this, new()
				{
					Page = ReportsPage.Storages,
					Parameters = _selectedStorage.Id
				});
			}
		}

		public ReceiptsPageViewModel()
		{
			Storages = new();
			Shops = new();

			if (!DatabaseController.IsConnected()) return;

			foreach (var storage in DatabaseController.GetStorages())
				Storages.Add(new StorageVM(storage));

			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));

			_selectedShop = Shops.First();
			_selectedStorage = Storages.First();
		}
	}
}
