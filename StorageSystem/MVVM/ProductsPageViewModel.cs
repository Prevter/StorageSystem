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
	public sealed class ProductsPageViewModel : BaseViewModel
	{
		public ObservableCollection<ProductVM> Products { get; set; }
		public ObservableCollection<ManufacturerVM> Manufacturers { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedName;
		private ManufacturerVM _editedManufacturer;
		private Visibility _modalVisibility = Visibility.Collapsed;

		private RelayCommand _addProductCommand, _removeProductsCommand, _saveCommand, _abortCommand, _editCommand;
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

		public ManufacturerVM EditedManufacturer
		{
			get => _editedManufacturer;
			set => SetField(ref _editedManufacturer, value);
		}

		public RelayCommand AddProductCommand
		{
			get => _addProductCommand ??= new RelayCommand(obj =>
			{
				_editedStartId = null;
				EditedId = "P";
				EditedName = "";
				EditedManufacturer = Manufacturers.First();
				ReadonlyKeys = false;
				ModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand RemoveProductsCommand
		{
			get => _removeProductsCommand ??= new RelayCommand(obj =>
			{
				var selected = Products.Where(m => m.Selected).ToList();
				if (!selected.Any())
					return;

				if (MessageBox.Show(
						$"Ви впевнені що хочете видалити {selected.Count} продуктів?",
						"Ви впевнені?",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question
					) != MessageBoxResult.Yes) return;

				selected.ForEach(p => DatabaseController.DeleteProduct(p.Id));

				Products.RemoveAll(product => product.Selected);
				Products.ForEach(product => product.Selected = false);
			});
		}

		public RelayCommand AbortCommand
		{
			get => _abortCommand ??= new RelayCommand(obj => ModalVisibility = Visibility.Hidden);
		}

		public RelayCommand SaveCommand
		{
			get => _saveCommand ??= new RelayCommand(obj =>
			{
				try
				{
					var product = new Product
					{
						Id = EditedId,
						Name = EditedName,
						ManufacturerId = EditedManufacturer.Id
					};

					if (_editedStartId is null)
					{
						// Create new
						DatabaseController.InsertProduct(product);
						Products.Add(new ProductVM(product, EditedManufacturer));
					}
					else
					{
						// Edit where id == _editedStartId
						Products.ForEach(p =>
						{
							if (p.Id == _editedStartId)
							{
								p.Id = EditedId;
								p.Name = EditedName;
								p.Manufacturer = EditedManufacturer;
							}

							DatabaseController.UpdateProduct(product);
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
				if (obj is ProductVM product)
				{
					_editedStartId = product.Id;
					EditedId = product.Id;
					EditedName = product.Name;
					EditedManufacturer = product.Manufacturer;
					ReadonlyKeys = true;
					ModalVisibility = Visibility.Visible;
				}
			});
		}

		public ProductsPageViewModel()
		{
			Manufacturers = new();
			Products = new();
			
			if (!DatabaseController.IsConnected()) return;

			foreach (var m in DatabaseController.GetManufacturers())
				Manufacturers.Add(new ManufacturerVM(m));

			foreach (var p in DatabaseController.GetProducts())
			{
				var manufacturer = Manufacturers.First(m => m.Id == p.ManufacturerId.Trim());
				Products.Add(new ProductVM(p, manufacturer));
			}
		}
	}
}
