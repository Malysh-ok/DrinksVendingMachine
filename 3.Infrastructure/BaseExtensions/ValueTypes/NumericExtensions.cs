using System;
using System.Globalization;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения числовых значений.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Абсолютная погрешность вычислений с двойной точностью по умолчанию.
        /// </summary>
        public const double DeltaD = 0.000000001;
        
        /// <summary>
        /// Абсолютная погрешность вычислений с одинарной точностью по умолчанию.
        /// </summary>
        public const float DeltaF = 0.000000001f;
        
        /// <summary>
        /// Сравнение двух вещественных чисел типа <see cref="double"/> с указанной точностью.
        /// </summary>
        /// <param name="first">Первое сравниваемое значение.</param>
        /// <param name="second">Второе сравниваемое значение.</param>
        /// <param name="delta">Абсолютная погрешность.</param>
        /// <returns>Результат операции сравнения.</returns>
        public static bool IsEquals(this double first, double second, double delta = DeltaD)
        {
            return Math.Abs(first - second) < delta;
        }

        /// <summary>
        /// Сравнение двух вещественных чисел типа <see cref="float"/> с указанной точностью.
        /// </summary>
        /// <param name="first">Первое сравниваемое значение.</param>
        /// <param name="second">Второе сравниваемое значение.</param>
        /// <param name="delta">Абсолютная погрешность.</param>
        /// <returns>Результат операции сравнения.</returns>
        public static bool IsEquals(this float first, float second, float delta = DeltaF)
        {
            return Math.Abs(first - second) < delta;
        }

        /// <summary>
        /// Ограничение значение указанными рамками отрезка.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="min">Нижняя граница отрезка.</param>
        /// <param name="max">Верхняя граница отрезка.</param>
        /// <returns>Ограниченное значение.</returns>
        public static int Bound(this int value, int min, int max)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз
            
            if (min > max)
                throw new ArgumentException(CommonPhrases.Exception_ParamInvalidLowerBound_);

            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <inheritdoc cref="Bound(int,int,int)"/>
        public static uint Bound(this uint value, uint min, uint max)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if(min > max)
                throw new ArgumentException(CommonPhrases.Exception_ParamInvalidLowerBound_);

            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        
        /// <inheritdoc cref="Bound(int,int,int)"/>
        public static double Bound(this double value, double min, double max)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз
            
            if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max))
                throw new InvalidOperationException();
            
            if (min > max)
                throw new ArgumentException(CommonPhrases.Exception_ParamInvalidLowerBound_);

            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// Проверка значения на вхождение в диапазон.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="min">Минимальное значение.</param>
        /// <param name="max">Максимальное значение.</param>
        /// <param name="inclusive">Признак того, что сравнивать необходимо с конечными значениями включительно.</param>
        public static bool IsBetween(this int value, int min, int max, bool inclusive = false)
        {
            return inclusive
                ? min <= value && value <= max
                : min < value && value < max;
        }

        /// <inheritdoc cref="IsBetween(int,int,int,bool)"/>
        public static bool IsBetween(this uint value, uint min, uint max, bool inclusive = false)
        {
            return inclusive
                ? min <= value && value <= max
                : min < value && value < max;
        }
        
        /// <inheritdoc cref="IsBetween(int,int,int,bool)"/>
        public static bool IsBetween(this double value, double min, double max, bool inclusive = false)
        {
            if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max))
                throw new InvalidOperationException();

            return inclusive
                ? min <= value && value <= max
                : min < value && value < max;
        }
        
        /// <summary>
        /// Замена значения <see cref="double.NaN"/> на указанное.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="defaultValue">Значение, на которое следует заменить <see cref="double.NaN"/>.</param>
        /// <returns>Полученное значение.</returns>
        public static double NaNToDouble(this double value, double defaultValue)
        {
            return double.IsNaN(value) ? defaultValue : value;
        }

        /// <summary>
        /// Замена неверного значения <see cref="double.NaN"/>, <see cref="double.IsNegativeInfinity"/>,
        /// <see cref="double.IsInfinity"/>, <see cref="double.IsPositiveInfinity"/> на указанное значение по умолчанию.
        /// </summary>
        /// <param name="value">Проверяемое значение.</param>
        /// <param name="defaultValue">Значение, на которое следует заменить.</param>
        /// <returns>Полученное значение.</returns>
        public static double IncorrectToDouble(this double value, double defaultValue)
        {
            return (double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
                ? defaultValue
                : value;
        }
        
        /// <summary>
        /// Проверка бита.
        /// </summary>
        public static bool TestBit(this ushort value, int bit)
        {
            var mask = (ushort)(1 << bit);
            var ush = (ushort)(value & mask);
            return (ush >> bit) != 0;
        }
        
        /// <summary>
        /// Проверка бита.
        /// </summary>
        public static bool TestBit(this uint value, int bit)
        {
            var mask = (ushort)(1 << bit);
            var ush = (ushort)(value & mask);
            return (ush >> bit) != 0;
        }

        /// <summary>
        /// Установить бит.
        /// </summary>
        public static ushort SetBit(this ushort value, int bit)
        {
            value |= (ushort)(1 << bit);
            return value;
        }

        /// <summary>
        /// Установить бит.
        /// </summary>
        public static uint SetBit(this uint value, int bit)
        {
            value |= (ushort)(1 << bit);
            return value;
        }

        /// <summary>
        /// Очистить бит.
        /// </summary>
        public static ushort ClearBit(this ushort value, int bit)
        {
            value &= (ushort)~(1 << bit);
            return value;
        }

        /// <summary>
        /// Очистить бит.
        /// </summary>
        public static uint ClearBit(this uint value, int bit)
        {
            value &= (ushort)~(1 << bit);
            return value;
        }

        /// <summary>
        /// Изменяет последовательность байт в <see cref="ushort"/>.
        /// </summary>
        public static ushort ReverseBytes(this ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            var result = BitConverter.ToUInt16(bytes, 0);
            return result;
        }
        
        /// <summary>
        /// Изменяет последовательность байт в <see cref="ushort"/>.
        /// </summary>
        public static uint ReverseBytes(this uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            var result = BitConverter.ToUInt32(bytes, 0);
            return result;
        }

        /// <summary>
        /// Получить int из обнуляемого типа.
        /// </summary>
        public static int FromNullable(this int? number, int defaultValue = 0)
        {
            return number ?? defaultValue;
        }
        
        /// <summary>
        /// Получить uint из обнуляемого типа.
        /// </summary>
        public static uint FromNullable(this uint? number, uint defaultValue = 0)
        {
            return number ?? defaultValue;
        }
        
        /// <summary>
        /// Получить double из обнуляемого типа.
        /// </summary>
        public static double FromNullable(this double? number, double defaultValue = 0)
        {
            return number ?? defaultValue;
        }
        
        /// <summary>
        /// Преобразование к нужному типу.
        /// </summary>
        public static T CastTo<T>(this IConvertible value, T defaultValue = default(T))
        {
            if (value is T val)
                return val;
            
            try
            {
                return (T) Convert.ChangeType(value, typeof(T),
                    CultureInfo.InstalledUICulture.NumberFormat);
            }
            catch 
            {
                return defaultValue;
            }
        }    
    }
}
