using FloxelLib.MVVM;
using StorageSystem.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Models
{
	public sealed class ShopProduct
	{
		public string ShopId { get; set; }
		public string ProductId { get; set; }
		public decimal Price { get; set; }
	}

	public sealed partial class ShopProductVM : BaseViewModel
	{
		[UpdateProperty]
		private ShopVM _shop;

		[UpdateProperty]
		private ProductVM _product;

		[UpdateProperty]
		private decimal _price;

		[UpdateProperty]
		private bool _selected;

		public ShopProductVM(ShopVM shop, ProductVM product, decimal price)
		{
			Shop = shop;
			Product = product;
			Price = price;
		}

		public ShopProduct ToModel()
		{
			return new ShopProduct()
			{
				ShopId = Shop.Id,
				ProductId = Product.Id,
				Price = Price
			};
		}

		public override string ToString()
		{
			return $"{Shop.Name} - {Product.Name} - {Price}";
		}

	}
}
