using StorageSystem.MVVM;

namespace StorageSystem.Models
{
	public sealed class Storage
	{
		public string Id { get; set; }
		public string Address { get; set; }
	}

	public sealed class StorageVM : BaseViewModel
	{
		private string _id, _address;
		private bool _selected;

		public string Id
		{
			get => _id;
			set => SetField(ref _id, value);
		}

		public string Address
		{
			get => _address;
			set => SetField(ref _address, value);
		}

		public bool Selected
		{
			get => _selected;
			set => SetField(ref _selected, value);
		}
		
		public StorageVM(Storage storage)
		{
			Id = storage.Id.Trim();
			Address = storage.Address.Trim();
		}

		public Storage ToModel()
		{
			return new Storage()
			{
				Id = Id,
				Address = Address
			};
		}

		public override string ToString()
		{
			return Address;
		}
	}
}
