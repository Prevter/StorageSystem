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
	public sealed class ManufacturersPageViewModel : BaseViewModel
	{
		public ObservableCollection<ManufacturerVM> Manufacturers { get; set; }

		private string? _editedStartId; // null when creating
		private string _editedId, _editedName, _editedContacts;
		private Visibility _modalVisibility = Visibility.Collapsed;

		private RelayCommand _addManufacturerCommand, _removeManufacturersCommand, _saveCommand, _abortCommand, _editCommand;

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

		public string EditedContacts
		{
			get => _editedContacts;
			set => SetField(ref _editedContacts, value);
		}

		public RelayCommand AddManufacturerCommand
		{
			get => _addManufacturerCommand ??= new RelayCommand(obj =>
			{
				_editedStartId = null;
				EditedId = "M";
				EditedName = "";
				EditedContacts = "";

				ModalVisibility = Visibility.Visible;
			});
		}

		public RelayCommand RemoveManufacturersCommand
		{
			get => _removeManufacturersCommand ??= new RelayCommand(obj =>
			{
				var selected = Manufacturers.Where(m => m.Selected).ToList();
				if (!selected.Any())
					return;

				if (MessageBox.Show(
						$"Ви впевнені що хочете видалити {selected.Count} виробників?", 
						"Ви впевнені?", 
						MessageBoxButton.YesNo, 
						MessageBoxImage.Question
					) != MessageBoxResult.Yes) return;

				selected.ForEach(m => DatabaseController.DeleteManufacturer(m.Id));

				Manufacturers.RemoveAll(manufacturer => manufacturer.Selected);
				Manufacturers.ForEach(manufacturer => manufacturer.Selected = false);
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

							DatabaseController.UpdateManufacturer(_editedStartId, manufacturer);
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
				if (obj is ManufacturerVM manufacturer)
				{
					_editedStartId = manufacturer.Id;
					EditedId = manufacturer.Id;
					EditedName = manufacturer.Name;
					EditedContacts = manufacturer.Contacts;

					ModalVisibility = Visibility.Visible;
				}
			});
		}

		public ManufacturersPageViewModel()
		{
			Manufacturers = new();
			foreach (var m in DatabaseController.GetManufacturers())
				Manufacturers.Add(new ManufacturerVM(m));
		}
	}
}
