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

namespace StorageSystem.MVVM
{
    public sealed class StoragesPageViewModel : BaseViewModel
    {
        public ObservableCollection<StorageVM> Storages { get; set; }
		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<ProductVM> AllProducts { get; set; }
		public ObservableCollection<ProductVM> Products { get; set; }
		public ObservableCollection<StoredProductVM> StoredProducts { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedAddress;
		private StorageVM _editedProductStorage;
		private RelayCommand _addStorageCommand, _removeStoragesCommand, _editCommand, _saveCommand, _abortCommand;

		private string? _editedProductId, _editedProductShopId;
		private ShopVM _editedProductShop;
		private ProductVM _editedProduct;
		private string _editedProductAmount;
		private RelayCommand _addProductCommand, _removeProductsCommand, _editProductsCommand, _editProductCommand, _abortProductCommand, _saveProductCommand;

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

		public ProductVM EditedProduct
		{
			get => _editedProduct;
			set => SetField(ref _editedProduct, value);
		}

		public string EditedProductAmount
		{
			get => _editedProductAmount;
			set => SetField(ref _editedProductAmount, value);
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

		public RelayCommand AbortProductCommand
		{
			get => _abortProductCommand ??= new RelayCommand(obj =>
			{
				ProductModalVisibility = Visibility.Hidden;
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

		public RelayCommand EditProductsCommand
		{
			get => _editProductsCommand ??= new RelayCommand(obj =>
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
			});
		}

		public RelayCommand AddProductCommand
		{
			get => _addProductCommand ??= new RelayCommand(obj =>
			{
				_editedProductId = null;
				_editedProductShopId = null;
				EditedProductShop = Shops.First();
				EditedProduct = Products.First();
				EditedProductAmount = "0";
				ProductModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand RemoveProductsCommand
		{
			get => _removeProductsCommand ??= new RelayCommand(obj =>
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
			});
		}

		public RelayCommand EditProductCommand
		{
			get => _editProductCommand ??= new RelayCommand(obj =>
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
			});
		}

		public RelayCommand SaveProductCommand
		{
			get => _saveProductCommand ??= new RelayCommand(obj =>
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
			});
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
