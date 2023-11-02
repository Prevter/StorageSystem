using FloxelLib.MVVM;

namespace StorageSystem.Models
{
    public sealed class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerId { get; set; }
    }

    public sealed partial class ProductVM : BaseViewModel
    {
        [UpdateProperty]
        private string _id, _name;
        [UpdateProperty]
        private ManufacturerVM _manufacturer;
        [UpdateProperty]
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

        public override string ToString()
        {
            return Name;
        }
    }
}
