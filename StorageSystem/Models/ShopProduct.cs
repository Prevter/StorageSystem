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

	public sealed class ShopProductVM : BaseViewModel
	{
		private ShopVM _shop;
		private ProductVM _product;
		private decimal _price;
		private bool _selected;

		public ShopVM Shop
		{
			get => _shop;
			set => SetField(ref _shop, value);
		}

		public ProductVM Product
		{
			get => _product;
			set => SetField(ref _product, value);
		}

		public decimal Price
		{
			get => _price;
			set => SetField(ref _price, value);
		}

		public bool Selected
		{
			get => _selected;
			set => SetField(ref _selected, value);
		}

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
