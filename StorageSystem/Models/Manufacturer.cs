using FloxelLib.MVVM;

namespace StorageSystem.Models
{
    public sealed class Manufacturer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Contacts { get; set; }
    }

    public sealed partial class ManufacturerVM : BaseViewModel
    {
        [UpdateProperty]
        private string _id, _name, _contacts;
        [UpdateProperty]
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

        public override string ToString()
        {
            return Name;
        }
    }
}
