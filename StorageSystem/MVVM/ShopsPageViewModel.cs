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
	public sealed class ShopsPageViewModel : BaseViewModel
	{
		public ObservableCollection<ShopVM> Shops { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedName, _editedFloor, _editedLocation;
		private Visibility _modalVisibility = Visibility.Collapsed;
		private Visibility _productsModalVisibility = Visibility.Collapsed;

		private RelayCommand _addShopCommand, _removeShopsCommand, _saveCommand, _abortCommand, _editCommand, _editProductsCommand;

		public Visibility EditModeControls
		{
			get => DatabaseController.ReadonlyAccess ? Visibility.Collapsed : Visibility.Visible;
		}

		public Visibility ModalVisibility
		{
			get => _modalVisibility;
			set => SetField(ref _modalVisibility, value);
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

		public string EditedName
		{
			get => _editedName;
			set => SetField(ref _editedName, value);
		}

		public string EditedLocation
		{
			get => _editedLocation;
			set => SetField(ref _editedLocation, value);
		}

		public string EditedFloor
		{
			get => _editedFloor;
			set => SetField(ref _editedFloor, value);
		}

		public RelayCommand AddShopCommand
		{
			get => _addShopCommand ??= new RelayCommand(obj =>
			{
				_editedStartId = null;
				EditedId = "SH";
				EditedName = "";
				EditedLocation = "";
				EditedFloor = "";
				ModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand RemoveShopsCommand
		{
			get => _removeShopsCommand ??= new RelayCommand(obj =>
			{
				var selected = Shops.Where(s => s.Selected).ToList();
				if (!selected.Any())
					return;

				if (MessageBox.Show(
						$"Ви впевнені що хочете видалити {selected.Count} магазинів?",
						"Ви впевнені?",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question
					) != MessageBoxResult.Yes) return;

				selected.ForEach(s => DatabaseController.DeleteShop(s.Id));

				Shops.RemoveAll(product => product.Selected);
				Shops.ForEach(product => product.Selected = false);
			});
		}

		public RelayCommand AbortCommand
		{
			get => _abortCommand ??= new RelayCommand(obj =>
			{
				ProductsModalVisibility = Visibility.Hidden;
				ModalVisibility = Visibility.Hidden;
			});
		}

		public RelayCommand SaveCommand
		{
			get => _saveCommand ??= new RelayCommand(obj =>
			{
				try
				{
					if (!int.TryParse(EditedFloor, out int floor))
						throw new Exception("Номер поверху має бути числом");

					var shop = new Shop
					{
						Id = EditedId,
						Name = EditedName,
						Location = EditedLocation,
						Floor = floor
					};

					if (_editedStartId is null)
					{
						// Create new
						DatabaseController.InsertShop(shop);
						Shops.Add(new ShopVM(shop));
					}
					else
					{
						// Edit where id == _editedStartId
						Shops.ForEach(s =>
						{
							if (s.Id == _editedStartId)
							{
								s.Id = EditedId;
								s.Name = EditedName;
								s.Location = EditedLocation;
								s.Floor = floor;
							}

							DatabaseController.UpdateShop(_editedStartId, shop);
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

		public RelayCommand EditCommand
		{
			get => _editCommand ??= new RelayCommand(obj =>
			{
				if (obj is ShopVM shop)
				{
					_editedStartId = shop.Id;
					EditedId = shop.Id;
					EditedName = shop.Name;
					EditedFloor = shop.Floor.ToString();
					EditedLocation = shop.Location;

					ModalVisibility = Visibility.Visible;
				}
			});
		}

		public ShopsPageViewModel()
		{
			Shops = new(); 
			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));
		}
	}
}
