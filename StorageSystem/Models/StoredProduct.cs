using FloxelLib.MVVM;
using StorageSystem.MVVM;

namespace StorageSystem.Models
{
	public sealed class StoredProduct
	{
		public string ProductId { get; set; }
		public string StorageId { get; set; }
		public string ShopId { get; set; }
		public int Amount { get; set; }
	}

	public sealed partial class StoredProductVM : BaseViewModel
	{
		[UpdateProperty]
		private StorageVM _storage;

		[UpdateProperty]
		private ProductVM _product;

		[UpdateProperty]
		private ShopVM _shop;

		[UpdateProperty]
		private int _amount;

		[UpdateProperty]
		private bool _selected;

		public StoredProductVM(ProductVM product, ShopVM shop, StorageVM storage, int amount)
		{
			Product = product;
			Shop = shop;
			Amount = amount;
			Storage = storage;
		}

		public StoredProduct ToModel()
		{
			return new StoredProduct()
			{
				ProductId = Product.Id,
				ShopId = Shop.Id,
				StorageId = Storage.Id,
				Amount = Amount
			};
		}

		public override string ToString()
		{
			return $"{Product} ({Amount})";
		}
	}
}
