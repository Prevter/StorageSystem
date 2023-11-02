using FloxelLib;
using FloxelLib.MVVM;
using StorageSystem.Common;
using StorageSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MessageBox = FloxelLib.Common.MessageBox;

namespace StorageSystem.MVVM
{
    public sealed partial class ManufacturersPageViewModel : BaseViewModel
    {
        public ObservableCollection<ManufacturerVM> Manufacturers { get; set; }

        private string? _editedStartId; // null when creating

        [UpdateProperty]
        private string _editedId, _editedName, _editedContacts;

        [UpdateProperty]
        private Visibility _modalVisibility = Visibility.Collapsed;

        [UpdateProperty]
        private bool _readonlyKeys;

        public Visibility EditModeControls
        {
            get => DatabaseController.ReadonlyAccess ? Visibility.Collapsed : Visibility.Visible;
        }

        [RelayCommand]
        private void AddManufacturer()
        {
            _editedStartId = null;
            EditedId = "M";
            EditedName = "";
            EditedContacts = "";
            ReadonlyKeys = false;
            ModalVisibility = Visibility.Visible;
        }

        [RelayCommand]
        public void RemoveManufacturers()
        {
            var selected = Manufacturers.Where(m => m.Selected).ToList();
            if (!selected.Any())
                return;

            if (MessageBox.Show(
                    $"Ви впевнені що хочете видалити {selected.Count} виробників?",
                    "Ви впевнені?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                ) != FloxelLib.Common.MessageBoxResult.Yes) return;

            selected.ForEach(m => DatabaseController.DeleteManufacturer(m.Id));

            Manufacturers.RemoveAll(manufacturer => manufacturer.Selected);
            Manufacturers.ForEach(manufacturer => manufacturer.Selected = false);
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
                var manufacturer = new Manufacturer
                {
                    Id = EditedId,
                    Name = EditedName,
                    Contacts = EditedContacts
                };

                if (_editedStartId is null)
                {
                    // Create new
                    DatabaseController.InsertManufacturer(manufacturer);
                    Manufacturers.Add(new ManufacturerVM(manufacturer));
                }
                else
                {
                    // Edit where id == _editedStartId
                    Manufacturers.ForEach(m =>
                    {
                        if (m.Id == _editedStartId)
                        {
                            m.Id = EditedId;
                            m.Name = EditedName;
                            m.Contacts = EditedContacts;
                        }

                        DatabaseController.UpdateManufacturer(manufacturer);
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
            if (obj is ManufacturerVM manufacturer)
            {
                _editedStartId = manufacturer.Id;
                EditedId = manufacturer.Id;
                EditedName = manufacturer.Name;
                EditedContacts = manufacturer.Contacts;
                ReadonlyKeys = true;
                ModalVisibility = Visibility.Visible;
            }
        }

        public ManufacturersPageViewModel()
        {
            Manufacturers = new();
            if (!DatabaseController.IsConnected()) return;

            foreach (var m in DatabaseController.GetManufacturers())
                Manufacturers.Add(new ManufacturerVM(m));
        }
    }
}
