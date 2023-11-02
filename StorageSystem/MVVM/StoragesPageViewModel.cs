using FloxelLib;
using FloxelLib.MVVM;
using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using MessageBox = FloxelLib.Common.MessageBox;
using MessageBoxResult = FloxelLib.Common.MessageBoxResult;

namespace StorageSystem.MVVM
{
	public sealed partial class StoragesPageViewModel : BaseViewModel
	{
		public ObservableCollection<StorageVM> Storages { get; set; }
		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<ProductVM> AllProducts { get; set; }
		public ObservableCollection<ProductVM> Products { get; set; }
		public ObservableCollection<StoredProductVM> StoredProducts { get; set; }

		private string? _editedStartId; // null when creating

		[UpdateProperty]
		private string _editedId, _editedAddress;

		[UpdateProperty]
		private StorageVM _editedProductStorage;

		private string? _editedProductId, _editedProductShopId;

		private ShopVM _editedProductShop;

		[UpdateProperty]
		private ProductVM _editedProduct;

		[UpdateProperty]
		private string _editedProductAmount;

		[UpdateProperty]
		private Visibility _modalVisibility = Visibility.Collapsed,
			_productsModalVisibility = Visibility.Collapsed,
			_productModalVisibility = Visibility.Collapsed;

		[UpdateProperty]
		private bool _readonlyKeys;

		public Visibility EditModeControls
		{
			get => DatabaseController.ReadonlyAccess ? Visibility.Collapsed : Visibility.Visible;
		}

		public ShopVM EditedProductShop
		{
			get => _editedProductShop;
			set
			{
				SetField(ref _editedProductShop, value);
				// Update products to match the shop
				Products.Clear();
				var products = DatabaseController.GetShopProducts(value.Id);
				foreach (var shopProduct in products)
				{
					var product = AllProducts.First(p => p.Id == shopProduct.ProductId.Trim());
					Products.Add(product);
				}
				EditedProduct = Products.First();
			}
		}

		[RelayCommand]
		public void AddStorage()
		{
			_editedStartId = null;
			EditedId = "S";
			EditedAddress = "";
			ReadonlyKeys = false;
			ModalVisibility = Visibility.Visible;
		}

		[RelayCommand]
		public void Edit(object obj)
		{
			if (obj is StorageVM storage)
			{
				_editedStartId = storage.Id;
				EditedId = storage.Id;
				EditedAddress = storage.Address;
				ReadonlyKeys = true;
				ModalVisibility = Visibility.Visible;
			}
		}

		[RelayCommand]
		public void RemoveStorages()
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
		}

		[RelayCommand]
		public void Abort()
		{
			ProductsModalVisibility = Visibility.Hidden;
			ProductModalVisibility = Visibility.Hidden;
			ModalVisibility = Visibility.Hidden;
		}

		[RelayCommand]
		public void AbortProduct()
		{
			ProductModalVisibility = Visibility.Hidden;
		}

		[RelayCommand]
		public void Save()
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
		}

		[RelayCommand]
		public void EditProducts(object obj)
		{
			if (obj is StorageVM storage)
			{
				var storedProducts = DatabaseController.GetStoredProducts().ToList();
				StoredProducts.Clear();
				foreach (var sp in storedProducts)
				{
					if (sp.StorageId.Trim() == storage.Id.Trim())
					{
						var productVM = AllProducts.First(p => p.Id == sp.ProductId.Trim());
						var shopVM = Shops.First(s => s.Id == sp.ShopId.Trim());
						StoredProducts.Add(new StoredProductVM(productVM, shopVM, storage, sp.Amount));
					}
				}
				_editedProductStorage = storage;
				ProductsModalVisibility = Visibility.Visible;
			}
		}

		[RelayCommand]
		public void AddProduct()
		{
			_editedProductId = null;
			_editedProductShopId = null;
			EditedProductShop = Shops.First();
			EditedProduct = Products.First();
			EditedProductAmount = "0";
			ProductModalVisibility = Visibility.Visible;
		}

		[RelayCommand]
		public void RemoveProducts()
		{
			var selected = StoredProducts.Where(sp => sp.Selected).ToList();
			if (!selected.Any())
				return;

			if (MessageBox.Show(
					$"Ви впевнені що хочете видалити {selected.Count} продуктів зі складу?",
					"Ви впевнені?",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question
				) != MessageBoxResult.Yes) return;

			selected.ForEach(sp => DatabaseController.DeleteStoredProduct(sp.Product.Id, _editedProductStorage.Id, sp.Shop.Id));

			StoredProducts.RemoveAll(sp => sp.Selected);
			StoredProducts.ForEach(sp => sp.Selected = false);
		}

		[RelayCommand]
		public void EditProduct(object obj)
		{
			if (obj is StoredProductVM storedProduct)
			{
				_editedProductId = storedProduct.Product.Id;
				EditedProductShop = storedProduct.Shop;
				EditedProduct = storedProduct.Product;
				EditedProductAmount = storedProduct.Amount.ToString(CultureInfo.InvariantCulture);
				ReadonlyKeys = true;
				ProductModalVisibility = Visibility.Visible;
			}
		}

		[RelayCommand]
		public void SaveProduct()
		{
			try
			{
				if (!int.TryParse(EditedProductAmount, CultureInfo.InvariantCulture, out int amount))
					throw new Exception("Номер поверху має бути числом");

				var storedProduct = new StoredProduct
				{
					ProductId = EditedProduct.Id,
					ShopId = _editedProductShop.Id,
					StorageId = _editedProductStorage.Id,
					Amount = amount
				};

				if (_editedProductId is null)
				{
					// Create new
					DatabaseController.InsertStoredProduct(storedProduct);
					StoredProducts.Add(new StoredProductVM(EditedProduct, EditedProductShop, _editedProductStorage, amount));
				}
				else
				{
					StoredProducts.ForEach(sp =>
					{
						if (sp.Product.Id == _editedProductId)
						{
							sp.Product = EditedProduct;
							sp.Shop = EditedProductShop;
							sp.Amount = amount;
						}

						DatabaseController.UpdateStoredProduct(storedProduct);
					});
				}

				ProductModalVisibility = Visibility.Hidden;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public StoragesPageViewModel()
		{
			Storages = new();
			Shops = new();
			AllProducts = new();
			Products = new();
			StoredProducts = new();

			if (!DatabaseController.IsConnected()) return;

			foreach (var storage in DatabaseController.GetStorages())
				Storages.Add(new StorageVM(storage));

			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));

			foreach (var p in DatabaseController.GetProducts())
				AllProducts.Add(new ProductVM(p, new ManufacturerVM()));
		}
	}
}
