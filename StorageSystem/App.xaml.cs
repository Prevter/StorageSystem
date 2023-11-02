using StorageSystem.Common;
using System;
using System.Windows;

namespace StorageSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static event EventHandler<string>? PageChanged;
        public static event EventHandler<string>? TitleChanged;

        public static void ChangePage(string page, string? title = null)
        {
            PageChanged?.Invoke(null, page);
            if (title is not null)
                TitleChanged?.Invoke(null, title);
        }

        public static void ChangeTitle(string title)
        {
            TitleChanged?.Invoke(null, title);
        }

        public App()
        {
            AppHelpers.Settings = Settings.Load();
        }
    }
}
