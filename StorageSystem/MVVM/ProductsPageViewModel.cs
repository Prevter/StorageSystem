using FloxelLib;
using FloxelLib.MVVM;
using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using MessageBox = FloxelLib.Common.MessageBox;
using MessageBoxResult = FloxelLib.Common.MessageBoxResult;

namespace StorageSystem.MVVM
{
    public sealed partial class ProductsPageViewModel : BaseViewModel
    {
        public ObservableCollection<ProductVM> Products { get; set; }
        public ObservableCollection<ManufacturerVM> Manufacturers { get; set; }

        private string? _editedStartId; // null when creating

        [UpdateProperty]
        private string _editedId, _editedName;

        [UpdateProperty]
        private ManufacturerVM _editedManufacturer;

        [UpdateProperty]
        private Visibility _modalVisibility = Visibility.Collapsed;

        [UpdateProperty]
        private bool _readonlyKeys;

        public Visibility EditModeControls
        {
            get => DatabaseController.ReadonlyAccess ? Visibility.Collapsed : Visibility.Visible;
        }

        [RelayCommand]
        public void AddProduct()
        {
            _editedStartId = null;
            EditedId = "P";
            EditedName = "";
            EditedManufacturer = Manufacturers.First();
            ReadonlyKeys = false;
            ModalVisibility = Visibility.Visible;
        }

        [RelayCommand]
        public void RemoveProducts()
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
        }

        [RelayCommand]
        public void Abort()
        {
            ModalVisibility = Visibility.Hidden;
        }

        [RelayCommand]
        public void Save()
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
        }

        [RelayCommand]
        public void Edit(object obj)
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
