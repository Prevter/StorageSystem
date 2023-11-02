using FloxelLib.MVVM;

namespace StorageSystem.Models
{
    public sealed class Storage
    {
        public string Id { get; set; }
        public string Address { get; set; }
    }

    public sealed partial class StorageVM : BaseViewModel
    {
        [UpdateProperty]
        private string _id, _address;

        [UpdateProperty]
        private bool _selected;

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
