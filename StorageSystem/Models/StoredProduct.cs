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

	public sealed class StoredProductVM : BaseViewModel
	{
		private StorageVM _storage;
		private ProductVM _product;
		private ShopVM _shop;
		private int _amount;
		private bool _selected;

		public StorageVM Storage
		{
			get => _storage;
			set => SetField(ref _storage, value);
		}

		public ProductVM Product
		{
			get => _product;
			set => SetField(ref _product, value);
		}

		public ShopVM Shop
		{
			get => _shop;
			set => SetField(ref _shop, value);
		}

		public int Amount
		{
			get => _amount;
			set => SetField(ref _amount, value);
		}

		public bool Selected
		{
			get => _selected;
			set => SetField(ref _selected, value);
		}

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
