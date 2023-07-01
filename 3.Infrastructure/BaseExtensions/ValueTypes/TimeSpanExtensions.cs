using System;
using System.Globalization;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения для <see cref="TimeSpan" />.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Проверка значения на вхождение в диапазон.
        /// </summary>
        /// <param name="timeSpan">Проверяемое значение.</param>
        /// <param name="min">Минимальное значение.</param>
        /// <param name="max">Максимальное значение.</param>
        /// <param name="inclusive">Признак того, что сравнивать необходимо с конечными значениями включительно.</param>
        public static bool IsBetween(this TimeSpan timeSpan, TimeSpan min, TimeSpan max, bool inclusive = false)
        {
            return inclusive
                ? min <= timeSpan && timeSpan <= max
                : min < timeSpan && timeSpan < max;
        }
        
        /// <summary>
        /// Ограничение значение timeSpan указанными рамками отрезка.
        /// </summary>
        /// <param name="timeSpan">Проверяемое значение.</param>
        /// <param name="min">Нижняя граница отрезка.</param>
        /// <param name="max">Верхняя граница отрезка.</param>
        /// <returns>Ограниченное значение.</returns>
        public static int Bound(this int timeSpan, int min, int max)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (min > max)
                throw new ArgumentException(CommonPhrases.Exception_ParamInvalidLowerBound_);
            
            if (timeSpan > max) return max;
            if (timeSpan < min) return min;
            return timeSpan;
        }

    }
}
