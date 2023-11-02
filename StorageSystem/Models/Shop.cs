using FloxelLib.MVVM;
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

	public sealed partial class ShopVM : BaseViewModel
	{
		[UpdateProperty]
		private string _id, _name, _location;

		[UpdateProperty]
		private int _floor;

		[UpdateProperty]
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

		public override string ToString()
		{
			return Name;
		}
	}
}
