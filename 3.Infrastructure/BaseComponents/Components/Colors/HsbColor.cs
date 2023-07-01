using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Colors
{
    /// <summary>
    /// Структура для определения Hsb-цвета.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public struct HsbColor
    {
        /// <summary>
        /// Точность при сравнении вещественных чисел.
        /// </summary>
        public const float TOLERANCE = 1e-6f;

        /// <summary>
        /// Макс. значение прозрачности.
        /// </summary>
        public const int AlphaMax = 255;

        /// <summary>
        /// Макс. значение цветового тона.
        /// </summary>
        public const float HueMax = 360;

        /// <summary>
        /// Макс. значение насыщенности.
        /// </summary>
        public const float SaturationMax = 1;

        /// <summary>
        /// Макс. значение яркости.
        /// </summary>
        public const float BrightnessMax = 1;

        /// <summary>
        /// Получает пустую структуру HsbColor.
        /// </summary>
        public static readonly HsbColor Empty = FromAhsb(AlphaMax, 0, 0, 0);

        private int? _alpha;
        /// <summary>
        /// Получает или устанавливает компонент прозрачности (от 0 до 255).
        /// </summary>
        public int Alpha
        {
            get => _alpha ?? AlphaMax;      // если отсутствует - создаем с максимально возможным значением
            set => _alpha = (value > AlphaMax) ? AlphaMax : ((value < 0) ? 0 : value);
        }

        private float _hue;
        /// <summary>
        /// Получает или устанавливает компонент цветового тона в градусах (от 0 до 360).
        /// </summary>
        public float Hue
        {
            get => _hue;
            set => _hue = NormalizedHue(value);
        }

        private float _saturation;
        /// <summary>
        /// Получает или устанавливает компонент насыщенности в долях единицы  (от 0 до 1).
        /// </summary>
        public float Saturation
        {
            get => _saturation;
            set => _saturation = (value > SaturationMax) ? SaturationMax : ((value < 0) ? 0 : value);
        }

        private float _brightness;
        /// <summary>
        /// Получает или устанавливает компонент яркости в долях единицы (от 0 до 1).
        /// </summary>
        public float Brightness
        {
            get => _brightness;
            set => _brightness = (value > BrightnessMax) ? BrightnessMax : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Нормализация цветового тона.
        /// </summary>
        /// <remarks>
        /// Корректирует значение таким образом, чтобы оно принадлежало диапазону [0; 360°].
        /// </remarks>
        private static float NormalizedHue(float hue)
        {
            hue %= HueMax;
            return hue < 0 ? HueMax + hue : hue;
        }
        
        /// <summary>
        /// Создать HsbColor.
        /// </summary>
        /// <param name="a"> Значение прозрачности. </param>
        /// <param name="h"> Значение тона. </param>
        /// <param name="s"> Значение насыщенности. </param>
        /// <param name="b"> Значение яркости. </param>
        public static HsbColor FromAhsb(int a, float h, float s, float b)
        {
            HsbColor hsbColor;
            hsbColor._alpha = (a > AlphaMax) ? AlphaMax : ((a < 0) ? 0 : a);
            hsbColor._hue = NormalizedHue(h);
            hsbColor._saturation = (s > SaturationMax) ? SaturationMax : ((s < 0) ? 0 : s);
            hsbColor._brightness = (b > BrightnessMax) ? BrightnessMax : ((b < 0) ? 0 : b);

            return hsbColor;
        }

        /// <summary>
        /// Создать HsbColor.
        /// </summary>
        /// <param name="h"> Значение тона. </param>
        /// <param name="s"> Значение насыщенности. </param>
        /// <param name="b"> Значение яркости. </param>
        public static HsbColor FromHsb(float h, float s, float b)
        {
            return FromAhsb(AlphaMax, h, s, b);
        }

        public static bool operator ==(HsbColor item1, HsbColor item2)
        {
            return (
                Math.Abs(item1.Hue - item2.Hue) < TOLERANCE
                && Math.Abs(item1.Saturation - item2.Saturation) < TOLERANCE
                && Math.Abs(item1.Brightness - item2.Brightness) < TOLERANCE
                );
        }

        public static bool operator !=(HsbColor item1, HsbColor item2)
        {
            return (
                Math.Abs(item1.Hue - item2.Hue) > TOLERANCE
                || Math.Abs(item1.Saturation - item2.Saturation) > TOLERANCE
                || Math.Abs(item1.Brightness - item2.Brightness) > TOLERANCE
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HsbColor)obj);
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Brightness.GetHashCode();
        }
    }
}