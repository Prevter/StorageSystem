namespace StorageSystem.MVVM
{
	public sealed class MainViewModel : BaseViewModel
	{
		private string _windowTitle = "Вхід - Склад360";
		private string _currentPage = "../Pages/LoginPage.xaml";

		public string WindowTitle
		{
			get => _windowTitle;
			set => SetField(ref _windowTitle, value);
		}

		public string CurrentPage
		{
			get => _currentPage;
			set => SetField(ref _currentPage, value);
		}

		public MainViewModel()
		{
			App.PageChanged += (sender, page) => CurrentPage = page;
			App.TitleChanged += (sender, title) => WindowTitle = title + " - Склад360";
		}
	}
}
