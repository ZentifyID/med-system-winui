using Microsoft.UI.Xaml;

namespace MedSystem.App
{
    /// <summary>Управление темой приложения. По умолчанию — как в системе,
    /// ручной выбор сохраняется между запусками.</summary>
    public static class ThemeHelper
    {
        private const string SettingsKey = "AppTheme";
        private static Window? _window;

        public static void Initialize(Window window)
        {
            _window = window;
            if (_window.Content is FrameworkElement fe)
                fe.RequestedTheme = LoadSaved();
        }

        public static ElementTheme Current =>
            _window?.Content is FrameworkElement fe ? fe.RequestedTheme : ElementTheme.Default;

        public static void Apply(ElementTheme theme)
        {
            if (_window?.Content is FrameworkElement fe)
                fe.RequestedTheme = theme;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }

        private static ElementTheme LoadSaved() =>
            (Windows.Storage.ApplicationData.Current.LocalSettings.Values[SettingsKey] as string) switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default,
            };
    }
}
