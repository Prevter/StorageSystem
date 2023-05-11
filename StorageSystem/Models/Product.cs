using StorageSystem.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Models
{
	public sealed class Product
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string ManufacturerId { get; set; }
	}

	public sealed class ProductVM : BaseViewModel
	{
		private string _id, _name;
		private ManufacturerVM _manufacturer;
		private bool _selected;

		public ProductVM()
		{
			Selected = false;
		}

		public ProductVM(Product product, ManufacturerVM manufacturer)
		{
			_id = product.Id.Trim();
			_name = product.Name;
			_manufacturer = manufacturer;
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

		public ManufacturerVM Manufacturer
		{
			get => _manufacturer;
			set => SetField(ref _manufacturer, value);
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
