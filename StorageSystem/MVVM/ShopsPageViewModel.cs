using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace StorageSystem.MVVM
{
	public sealed class ShopsPageViewModel : BaseViewModel
	{
		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<ProductVM> Products { get; set; }
		public ObservableCollection<ShopProductVM> ShopProducts { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedName, _editedFloor, _editedLocation;

		private string? _editedProductId;
		private ProductVM _editedProduct;
		private ShopVM _editedProductShop;
		private string _editedPrice;

		private Visibility _modalVisibility = Visibility.Collapsed;
		private Visibility _productsModalVisibility = Visibility.Collapsed;
		private Visibility _productModalVisibility = Visibility.Collapsed;

		private RelayCommand _addShopCommand, _removeShopsCommand, _saveCommand, _abortCommand, _editCommand;
		private RelayCommand _addProductCommand, _removeProductsCommand, _editProductCommand, _abortProductCommand, _saveProductCommand, _editProductsCommand;

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

		public string EditedPrice
		{
			get => _editedPrice;
			set => SetField(ref _editedPrice, value);
		}

		public ProductVM EditedProduct
		{
			get => _editedProduct;
			set => SetField(ref _editedProduct, value);
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
				ReadonlyKeys = false;
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
					ReadonlyKeys = true;
					ModalVisibility = Visibility.Visible;
				}
			});
		}

		public RelayCommand AddProductCommand
		{
			get => _addProductCommand ??= new RelayCommand(obj =>
			{
				_editedProductId = null;
				EditedProduct = Products.First();
				ReadonlyKeys = false;
				ProductModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand RemoveProductsCommand
		{
			get => _removeProductsCommand ??= new RelayCommand(obj =>
			{
				var selected = ShopProducts.Where(sp => sp.Selected).ToList();
				if (!selected.Any())
					return;

				if (MessageBox.Show(
						$"Ви впевнені що хочете видалити {selected.Count} продуктів з магазину?",
						"Ви впевнені?",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question
					) != MessageBoxResult.Yes) return;

				selected.ForEach(sp => DatabaseController.DeleteShopProduct(sp.Shop.Id, sp.Product.Id));

				ShopProducts.RemoveAll(sp => sp.Selected);
				ShopProducts.ForEach(sp => sp.Selected = false);
			});
		}

		public RelayCommand EditProductsCommand
		{
			get => _editProductsCommand ??= new RelayCommand(obj =>
			{
				if (obj is ShopVM shop)
				{
					var shopProducts = DatabaseController.GetShopProducts().ToList();
					ShopProducts.Clear();
					foreach (var sp in shopProducts)
					{
						if (sp.ShopId.Trim() == shop.Id.Trim())
						{
							var productVM = Products.First(p => p.Id == sp.ProductId.Trim());
							ShopProducts.Add(new ShopProductVM(shop, productVM, sp.Price));
						}
					}
					_editedProductShop = shop;
					ProductsModalVisibility = Visibility.Visible;
				}
			});
		}

		public RelayCommand EditProductCommand
		{
			get => _editProductCommand ??= new RelayCommand(obj =>
			{
				if (obj is ShopProductVM shopProduct)
				{
					_editedProductId = shopProduct.Product.Id;
					EditedProduct = shopProduct.Product;
					EditedPrice = shopProduct.Price.ToString(CultureInfo.InvariantCulture);
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
					if (!decimal.TryParse(EditedPrice, CultureInfo.InvariantCulture, out decimal price))
						throw new Exception("Номер поверху має бути числом");

					var shop = new ShopProduct
					{
						ProductId = EditedProduct.Id,
						ShopId = _editedProductShop.Id,
						Price = price
					};

					if (_editedProductId is null)
					{
						// Create new
						DatabaseController.InsertShopProduct(shop);
						ShopProducts.Add(new ShopProductVM(_editedProductShop, EditedProduct, price));
					}
					else
					{
						ShopProducts.ForEach(sp =>
						{
							if (sp.Product.Id == _editedProductId)
							{
								sp.Product = EditedProduct;
								sp.Price = price;
							}

							DatabaseController.UpdateShopProduct(shop);
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

		public ShopsPageViewModel()
		{
			Shops = new();
			Products = new();
			ShopProducts = new();

			if (!DatabaseController.IsConnected()) return;

			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));

			var manufacturers = DatabaseController.GetManufacturers().ToList();
			foreach (var p in DatabaseController.GetProducts())
			{
				var manufacturer = manufacturers.First(m => m.Id.Trim() == p.ManufacturerId.Trim());
				Products.Add(new ProductVM(p, new ManufacturerVM(manufacturer)));
			}

		}
	}
}
