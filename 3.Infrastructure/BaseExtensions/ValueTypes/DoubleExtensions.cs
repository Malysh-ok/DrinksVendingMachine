using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения для <see cref="Double" />.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class DoubleExtensions
    {
        /// <summary>
        /// Формат вещественных чисел с разделителем ".".
        /// </summary>
        public static NumberFormatInfo NfiDot => 
            (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

        /// <summary>
        /// Формат вещественных чисел с разделителем ",".
        /// </summary>
        public static NumberFormatInfo NfiComma
        {
            get
            {
                var nfiRus = (NumberFormatInfo)CultureInfo.GetCultureInfo("ru-RU").NumberFormat.Clone();
                nfiRus.CurrencyDecimalSeparator = ",";
                nfiRus.CurrencyGroupSeparator = " ";
                return nfiRus;
            }
        }

        /// <summary>
        /// Попытаться преобразовать строку в вещественное число.
        /// </summary>
        /// <remarks>
        /// Метод работает с разделителями целой части '.' и ','.
        /// Если преобразование невозможно, - result = 0.
        /// </remarks>
        /// <param name="str">Строка для преобразования.</param>
        /// <param name="result">Полученное вещественное число.</param>
        public static bool TryParseDouble(this string str, out double result)
        {
            return double.TryParse(str, NumberStyles.Float, 
                str.Contains(".") ? NfiDot : NfiComma, out result);
        }

        /// <summary>
        /// Преобразовать строку в вещественное число.
        /// </summary>
        /// <remarks>
        /// Метод работает с разделителями целой части '.' и ','.
        /// Если преобразование невозможно, - возвращается double.NaN.
        /// </remarks>
        /// <param name="str">Строка для преобразования.</param>
        /// <param name="isThrowException">Признак генерации исключения при неудачном преобразовании.</param>
        public static double ToDouble(this string str, bool isThrowException = false)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз
            
            if (str.TryParseDouble(out var result))
                return result;

            if (isThrowException)
                throw new FormatException(CommonPhrases.Exception_ParamIsNotNumber.Format(str));
            
            return double.NaN;
        }

        /// <summary>
        /// Нормализация (замена разделителя '.' или ',' на значение, определяемое параметром <paramref name="culture"/>)
        /// и преобразование строки в вещественное число.
        /// </summary>
        /// <remarks>
        /// Если преобразование невозможно, - возвращается double.NaN.<para/>
        /// Работает немного быстрее, чем <see cref="ToDouble(string, bool)"/>.
        /// </remarks>
        /// <param name="str">Строка для преобразования.</param>
        /// <param name="culture">Языковый стандарт, к которой происходит нормализация.</param>
        /// <param name="isThrowException">Признак генерации исключения при неудачном преобразовании.</param>
        public static double ToDouble(this string str, CultureInfo culture, bool isThrowException = false)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            var normalized = str.IsNullOrEmpty()
                ? str
                : str.NormalizeForDouble(culture);
            
            if (double.TryParse(normalized, out var result))
            {
                return result;
            }

            if (isThrowException)
                throw new FormatException(CommonPhrases.Exception_ParamIsNotNumber.Format(str));
            
            return double.NaN;
        }
        
        /// <summary>
        /// Проверка вхождения числа num в диапазон от lower до upper.
        /// </summary>
        /// <remarks> Если параметр inclusive = true, - сравнение с конечными значениями включительно. </remarks>
        public static bool IsBetween(this double num, double lower, double upper, bool inclusive = false)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }    }
}