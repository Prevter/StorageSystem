using StorageSystem.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Models
{
	public sealed class Manufacturer
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Contacts { get; set; }
	}

	public sealed class ManufacturerVM : BaseViewModel
	{
		private string _id, _name, _contacts;
		private bool _selected;

		public ManufacturerVM()
		{
			Selected = false;
		}

		public ManufacturerVM(Manufacturer manufacturer)
		{
			_id = manufacturer.Id.Trim();
			_name = manufacturer.Name;
			_contacts = manufacturer.Contacts;
			_selected = false;
		}

		public string Id
		{
			get => _id;
			set => SetField(ref _id, value);
		}

		public string Name
		{
			get => _name;
			set => SetField(ref _name, value);
		}

		public string Contacts
		{
			get => _contacts;
			set => SetField(ref _contacts, value);
		}

		public bool Selected
		{
			get => _selected;
			set => SetField(ref _selected, value);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
