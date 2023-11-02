using FloxelLib.MVVM;

namespace StorageSystem.MVVM
{
    public sealed partial class MainViewModel : BaseViewModel
    {
        [UpdateProperty]
        private string _windowTitle = "Вхід - Склад360",
            _currentPage = "../Pages/LoginPage.xaml";

        public MainViewModel()
        {
            App.PageChanged += (sender, page) => CurrentPage = page;
            App.TitleChanged += (sender, title) => WindowTitle = title + " - Склад360";
        }
    }
}
