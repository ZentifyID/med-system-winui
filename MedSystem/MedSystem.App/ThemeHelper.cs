using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;

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
                SetTheme(fe, LoadSaved());
        }

        public static ElementTheme Current =>
            _window?.Content is FrameworkElement fe ? fe.RequestedTheme : ElementTheme.Default;

        public static void Apply(ElementTheme theme)
        {
            if (_window?.Content is FrameworkElement fe)
                SetTheme(fe, theme);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }

        private static void SetTheme(FrameworkElement root, ElementTheme theme)
        {
            root.RequestedTheme = theme;
            if (_window != null && AppWindowTitleBar.IsCustomizationSupported())
            {
                _window.AppWindow.TitleBar.PreferredTheme = theme switch
                {
                    ElementTheme.Light => TitleBarTheme.Light,
                    ElementTheme.Dark => TitleBarTheme.Dark,
                    _ => TitleBarTheme.UseDefaultAppMode,
                };
            }
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
