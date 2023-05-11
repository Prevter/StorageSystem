using StorageSystem.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace StorageSystem.Models
{
	public sealed class Shop
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Floor { get; set; }
		public string Location { get; set; }
	}

	public sealed class ShopVM : BaseViewModel
	{
		private string _id, _name, _location;
		private int _floor;
		private bool _selected;

		public ShopVM()
		{
			Selected = false;
		}

		public ShopVM(Shop shop)
		{
			_id = shop.Id.Trim();
			_name = shop.Name;
			_location = shop.Location;
			_floor = shop.Floor;
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

		public string Location
		{
			get => _location;
			set => SetField(ref _location, value);
		}

		public int Floor
		{
			get => _floor;
			set => SetField(ref _floor, value);
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
