using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageSystem.MVVM
{
    public sealed class StoragesPageViewModel : BaseViewModel
    {
        public ObservableCollection<StorageVM> Storages { get; set; }
		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<ProductVM> Products { get; set; }
		// public ObservableCollection<StorageProductVM> StorageProducts { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedAddress;
		private RelayCommand _addStorageCommand, _removeStoragesCommand, _editCommand, _saveCommand, _abortCommand;

		private Visibility _modalVisibility = Visibility.Collapsed;
		private Visibility _productsModalVisibility = Visibility.Collapsed;
		private Visibility _productModalVisibility = Visibility.Collapsed;
		private bool _readonlyKeys;

		public bool ReadonlyKeys
		{
			get => _readonlyKeys;
			set => SetField(ref _readonlyKeys, value);
		}

		public Visibility EditModeControls
		{
			get => DatabaseController.ReadonlyAccess ? Visibility.Collapsed : Visibility.Visible;
		}

		public Visibility ModalVisibility
		{
			get => _modalVisibility;
			set => SetField(ref _modalVisibility, value);
		}

		public Visibility ProductModalVisibility
		{
			get => _productModalVisibility;
			set => SetField(ref _productModalVisibility, value);
		}

		public Visibility ProductsModalVisibility
		{
			get => _productsModalVisibility;
			set => SetField(ref _productsModalVisibility, value);
		}

		public string EditedId
		{
			get => _editedId;
			set => SetField(ref _editedId, value);
		}

		public string EditedAddress
		{
			get => _editedAddress;
			set => SetField(ref _editedAddress, value);
		}

		public RelayCommand AddStorageCommand
		{
			get => _addStorageCommand ??= new RelayCommand(obj =>
			{
				_editedStartId = null;
				EditedId = "S";
				EditedAddress = "";
				ReadonlyKeys = false;
				ModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand EditCommand
		{
			get => _editCommand ??= new RelayCommand(obj =>
			{
				if (obj is StorageVM storage)
				{
					_editedStartId = storage.Id;
					EditedId = storage.Id;
					EditedAddress = storage.Address;
					ReadonlyKeys = true;
					ModalVisibility = Visibility.Visible;
				}
			});
		}

		public RelayCommand RemoveStoragesCommand
		{
			get => _removeStoragesCommand ??= new RelayCommand(obj =>
			{
				var selected = Storages.Where(s => s.Selected).ToList();
				if (!selected.Any())
					return;

				if (MessageBox.Show(
						$"Ви впевнені що хочете видалити {selected.Count} складів?",
						"Ви впевнені?",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question
					) != MessageBoxResult.Yes) return;

				selected.ForEach(s => DatabaseController.DeleteStorage(s.Id));

				Storages.RemoveAll(storage => storage.Selected);
				Storages.ForEach(storage => storage.Selected = false);
			});
		}

		public RelayCommand AbortCommand
		{
			get => _abortCommand ??= new RelayCommand(obj =>
			{
				ProductsModalVisibility = Visibility.Hidden;
				ProductModalVisibility = Visibility.Hidden;
				ModalVisibility = Visibility.Hidden;
			});
		}

		public RelayCommand SaveCommand
		{
			get => _saveCommand ??= new RelayCommand(obj =>
			{
				try
				{
					var storage = new Storage
					{
						Id = EditedId,
						Address = EditedAddress
					};

					if (_editedStartId is null)
					{
						// Create new
						DatabaseController.InsertStorage(storage);
						Storages.Add(new StorageVM(storage));
					}
					else
					{
						// Edit where id == _editedStartId
						Storages.ForEach(s =>
						{
							if (s.Id == _editedStartId)
							{
								s.Id = EditedId;
								s.Address = EditedAddress;
							}

							DatabaseController.UpdateStorage(storage);
						});
					}

					ModalVisibility = Visibility.Hidden;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			});
		}

		public StoragesPageViewModel()
        {
			Storages = new();
			Shops = new();
			Products = new();

			if (!DatabaseController.IsConnected()) return;

			foreach (var storage in DatabaseController.GetStorages())
				Storages.Add(new StorageVM(storage));

			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));

			foreach (var p in DatabaseController.GetProducts())
				Products.Add(new ProductVM(p, new ManufacturerVM()));
		}
    }
}
