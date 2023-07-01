using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения для <see cref="Enum" />.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Преобразует текст к перечислению.
        /// </summary>
        /// <param name="enumString">Текст для преобразования.</param>
        /// <param name="isThrowException">Признак генерации исключения при неудачном преобразовании.</param>
        public static T ToEnum<T>(this string enumString, bool isThrowException = false) 
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return Enum.TryParse(enumString, true, out T value)
                ? value
                : isThrowException 
                    ? throw new ArgumentException(nameof(enumString)) 
                    : default(T);
        }
        
        /// <summary>
        /// Преобразует перечисление к int.
        /// </summary>
        public static int ToInt<T>(this T source) 
            where T : struct, IComparable, IFormattable, IConvertible
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (!typeof(T).IsEnum)
                throw new ArgumentException(
                    CommonPhrases.Exception_ParamIsNotEnum.Format(typeof(T), nameof(source)));

            return (int)(IConvertible)source;
        }

        /// <inheritdoc cref="ToEnumWithException{T}(int)"/>
        /// <param name="isThrowException">Признак генерации исключения при неудачном преобразовании.</param>
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public static T ToEnum<T>(this int enumInt, bool isThrowException = false)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var enm =  (T)Enum.ToObject(typeof(T), enumInt);
            
            if (isThrowException && !Enum.IsDefined(typeof(T), enm))
                throw new ArgumentException(
                    CommonPhrases.Exception_ParamIsNotConvertToEnum.Format(nameof(enumInt)));
            
            return enm;
        }
        
        /// <summary>
        /// Преобразует целое число к перечислению.
        /// </summary>
        /// <param name="enumInt">Число для преобразования.</param>
        public static T ToEnumWithException<T>(this int enumInt)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return enumInt.ToEnum<T>(true);
        }

        /// <summary>
        /// Достать описание из атрибутов перечислимого типа.
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute != null ? attribute.Description : value.ToString();
        }

        /// <summary>
        /// Содержит ли перечисление хотя бы один из указанных флагов.
        /// </summary>
        /// <param name="value">Проверяемое перечисление.</param>
        /// <param name="testFlags">Проверяемые флаги.</param>
        public static bool ContainsAny(this Enum value, Enum testFlags)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (testFlags == null)
                throw new ArgumentNullException(nameof(testFlags));

            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз
            
            if (value.GetType() != testFlags.GetType())
            {
                throw new ArgumentException(
                    CommonPhrases.Exception_EnumTypeMismatchInParam.Format(
                        testFlags.GetType(), value.GetType()));
            }

            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            dynamic flagsType = Convert.ChangeType(value, underlyingType);
            dynamic testFlagsType = Convert.ChangeType(testFlags, underlyingType);
            return (flagsType & testFlagsType) != 0;
        }

        /// <summary>
        /// Содержит ли перечисление только указанные флаги.
        /// </summary>
        /// <param name="value">Проверяемое перечисление.</param>
        /// <param name="testFlags">Проверяемые флаги.</param>
        public static bool ContainsExact(this Enum value, Enum testFlags)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (testFlags == null)
                throw new ArgumentNullException(nameof(testFlags));
            
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (value.GetType() != testFlags.GetType())
            {
                throw new ArgumentException(
                    CommonPhrases.Exception_EnumTypeMismatchInParam.Format(
                        testFlags.GetType(), value.GetType()));
            }
            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            dynamic flagsType = Convert.ChangeType(value, underlyingType);
            dynamic testFlagsType = Convert.ChangeType(testFlags, underlyingType);
            return (flagsType & testFlagsType) == testFlagsType;
        }

        /// <summary>
        /// Установка флагов.
        /// </summary>
        /// <param name="value">Исходное перечисление.</param>
        /// <param name="addingFlags">Устанавливаемые флаги.</param>
        public static T Set<T>(this Enum value, T addingFlags) 
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var underlyingType = Enum.GetUnderlyingType(value.GetType());

            dynamic flagsType = Convert.ChangeType(value, underlyingType);
            dynamic addingFlagsType = Convert.ChangeType(addingFlags, underlyingType);

            flagsType |= addingFlagsType;

            return (T)flagsType;
        }

        /// <summary>
        /// Очистка флагов.
        /// </summary>
        /// <param name="value">Исходное перечисление.</param>
        /// <param name="removingFlags">Сбрасываемые флаги.</param>
        public static T Clear<T>(this Enum value, T removingFlags) 
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var underlyingType = Enum.GetUnderlyingType(value.GetType());

            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            dynamic removingFlagsAsInt = Convert.ChangeType(removingFlags, underlyingType);
            valueAsInt &= ~removingFlagsAsInt;

            return (T)valueAsInt;
        }

        /// <summary>
        /// Выполнение делегата <paramref name="processFlag"/> над всеми установленными флагами перечисления.
        /// </summary>
        /// <param name="value">Исходное перечисление.</param>
        /// <param name="processFlag">Выполняемый делегат.</param>
        public static void ForEach<T>(this Enum value, Action<T> processFlag) 
            where T : struct, IComparable, IFormattable, IConvertible
        {
            if (processFlag == null)
                throw new ArgumentNullException(nameof(processFlag));

            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);

            for (var bit = 1; bit != 0; bit <<= 1)
            {
                var tempBit = valueAsInt & bit;
                if ((valueAsInt & bit) != 0)
                    processFlag((T)tempBit);
            }
        }
    }
}
