using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MedSystem.App
{
    /// <summary>
    /// Кисти для бейджей вокруг дат и количества: приглушённый фон,
    /// насыщенный текст. Цвета заданы явно на каждую тему — в тёмной
    /// фоны полупрозрачные и мягкие, текст светлый.
    /// </summary>
    public static class Badges
    {
        public static Brush TransparentBg { get; } = S(0x00, 0x00, 0x00, 0x00);

        // Светлая тема
        private static readonly Brush LightNormalFg = S(0xFF, 0x5D, 0x5D, 0x5D);
        private static readonly Brush LightCautionBg = S(0xFF, 0xFF, 0xF4, 0xCE);
        private static readonly Brush LightCautionFg = S(0xFF, 0x9D, 0x5D, 0x00);
        private static readonly Brush LightCriticalBg = S(0xFF, 0xFD, 0xE7, 0xE9);
        private static readonly Brush LightCriticalFg = S(0xFF, 0xC4, 0x2B, 0x1C);

        // Тёмная тема: полупрозрачные фоны, мягкие светлые оттенки текста
        private static readonly Brush DarkNormalFg = S(0xC8, 0xFF, 0xFF, 0xFF);
        private static readonly Brush DarkCautionBg = S(0x29, 0xFC, 0xE1, 0x00);
        private static readonly Brush DarkCautionFg = S(0xFF, 0xE9, 0xC8, 0x5D);
        private static readonly Brush DarkCriticalBg = S(0x26, 0xFF, 0x99, 0xA4);
        private static readonly Brush DarkCriticalFg = S(0xFF, 0xFF, 0x99, 0xA4);

        public static Brush NormalFg => LightNormalFg;

        /// <summary>Пара кистей (фон, текст) по статусу и теме.</summary>
        public static (Brush Bg, Brush Fg) For(bool isExpired, bool isExpiring, bool isDarkTheme)
        {
            if (isExpired)
                return isDarkTheme ? (DarkCriticalBg, DarkCriticalFg) : (LightCriticalBg, LightCriticalFg);
            if (isExpiring)
                return isDarkTheme ? (DarkCautionBg, DarkCautionFg) : (LightCautionBg, LightCautionFg);
            return (TransparentBg, isDarkTheme ? DarkNormalFg : LightNormalFg);
        }

        private static SolidColorBrush S(byte a, byte r, byte g, byte b) =>
            new(Color.FromArgb(a, r, g, b));
    }
}
