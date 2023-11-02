using FloxelLib;
using FloxelLib.MVVM;
using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

using MessageBox = FloxelLib.Common.MessageBox;
using MessageBoxResult = FloxelLib.Common.MessageBoxResult;

namespace StorageSystem.MVVM
{
	public sealed partial class ShopsPageViewModel : BaseViewModel
	{
		public ObservableCollection<ShopVM> Shops { get; set; }
		public ObservableCollection<ProductVM> Products { get; set; }
		public ObservableCollection<ShopProductVM> ShopProducts { get; set; }

		private string? _editedStartId; // null when creating
		[UpdateProperty]
		private string _editedId, _editedName, _editedFloor, _editedLocation;

		private string? _editedProductId;
		[UpdateProperty]
		private ProductVM _editedProduct;
		[UpdateProperty]
		private ShopVM _editedProductShop;
		[UpdateProperty]
		private string _editedPrice;

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

		[RelayCommand]
		public void AddShop()
		{
			_editedStartId = null;
			EditedId = "SH";
			EditedName = "";
			EditedLocation = "";
			EditedFloor = "";
			ReadonlyKeys = false;
			ModalVisibility = Visibility.Visible;
		}

		[RelayCommand]
		public void RemoveShops()
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

			Shops.RemoveAll(shop => shop.Selected);
			Shops.ForEach(shop => shop.Selected = false);
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

						DatabaseController.UpdateShop(shop);
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
		public void Edit(object obj)
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
		}

		[RelayCommand]
		public void AddProduct()
		{
			_editedProductId = null;
			EditedProduct = Products.First();
			ReadonlyKeys = false;
			ProductModalVisibility = Visibility.Visible;
		}

		[RelayCommand]
		public void RemoveProducts()
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
		}

		[RelayCommand]
		public void EditProducts(object obj)
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
		}

		[RelayCommand]
		public void EditProduct(object obj)
		{
			if (obj is ShopProductVM shopProduct)
			{
				_editedProductId = shopProduct.Product.Id;
				EditedProduct = shopProduct.Product;
				EditedPrice = shopProduct.Price.ToString(CultureInfo.InvariantCulture);
				ReadonlyKeys = true;
				ProductModalVisibility = Visibility.Visible;
			}
		}

		[RelayCommand]
		public void SaveProduct()
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
		}

		public ShopsPageViewModel()
		{
			Shops = new();
			Products = new();
			ShopProducts = new();

			if (!DatabaseController.IsConnected()) return;

			foreach (var s in DatabaseController.GetShops())
				Shops.Add(new ShopVM(s));

			// var manufacturers = DatabaseController.GetManufacturers().ToList();
			foreach (var p in DatabaseController.GetProducts())
			{
				// var manufacturer = manufacturers.First(m => m.Id.Trim() == p.ManufacturerId.Trim());
				// we could add manufacturer data to our products, but we don't need it here.
				Products.Add(new ProductVM(p, new ManufacturerVM()));
			}

		}
	}
}
