using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MedSystem.App
{
    /// <summary>
    /// Кисти для бейджей-«пилюль» вокруг дат: приглушённый цветной фон,
    /// насыщенный текст того же оттенка. Использует системные пары
    /// Caution/Critical из WinUI — корректно выглядят в обеих темах.
    /// </summary>
    public static class Badges
    {
        public static Brush TransparentBg { get; } = new SolidColorBrush(Colors.Transparent);

        public static Brush NormalFg => Get("TextFillColorSecondaryBrush", Color.FromArgb(255, 96, 96, 96));

        public static Brush CautionBg => Get("SystemFillColorCautionBackgroundBrush", Color.FromArgb(255, 255, 244, 206));
        public static Brush CautionFg => Get("SystemFillColorCautionBrush", Color.FromArgb(255, 157, 93, 0));

        public static Brush CriticalBg => Get("SystemFillColorCriticalBackgroundBrush", Color.FromArgb(255, 253, 231, 233));
        public static Brush CriticalFg => Get("SystemFillColorCriticalBrush", Color.FromArgb(255, 196, 43, 28));

        /// <summary>Пара кистей (фон, текст) по статусу: просрочено/истекает/норма.</summary>
        public static (Brush Bg, Brush Fg) For(bool isExpired, bool isExpiring)
        {
            if (isExpired)
                return (CriticalBg, CriticalFg);
            if (isExpiring)
                return (CautionBg, CautionFg);
            return (TransparentBg, NormalFg);
        }

        private static Brush Get(string key, Color fallback) =>
            Application.Current.Resources.TryGetValue(key, out var value) && value is Brush brush
                ? brush
                : new SolidColorBrush(fallback);
    }
}
