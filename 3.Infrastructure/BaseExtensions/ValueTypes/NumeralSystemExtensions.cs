using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы преобразований систем счисления.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class NumeralSystemExtensions
    {
        private const string Digits = 
            "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";   // цифры base58
        private static readonly IDictionary<char, int> DigitDictionary = 
            new Dictionary<char, int>(Digits.Length);                       // словарь символов base58
        private const int BitsInLong = 64;

        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static NumeralSystemExtensions()
        {
            for (var i = 0; i < Digits.Length; i++)
            {
                DigitDictionary.Add(Digits[i], i);
            }
        }

        internal static char GetChar(int index, int alphabetOffset)
        {
            var i = (index + alphabetOffset) % Digits.Length;
            return Digits[i];
        }

        internal static int GetIndex(int index, int alphabetOffset)
        {
            var i = (index + Digits.Length - alphabetOffset) % Digits.Length;
            return i;
        }

        /// <summary>
        /// Конвертирует десятичное число в Base58.
        /// </summary>
        /// <param name="number">Число в десятичной системе счисления.</param>
        /// <param name="alphabetOffset">Смещение алфавита.</param>
        /// <returns>Строковое значение в Base58.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBase58(this ulong number, uint alphabetOffset = 0)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;
            
            if (number == 0)
                return "".PadLeft(11, Digits.ElementAt(0));

            if (alphabetOffset >= Digits.Length)
                throw new ArgumentException(
                    CommonPhrases.Exception_ParamIsTooLong.Format(nameof(alphabetOffset), Digits.Length));

            var index = BitsInLong - 1;
            var charArray = new char[BitsInLong];

            while (number != 0)
            {
                var remainder = (int)(number % (ulong)Digits.Length);
                charArray[index--] = GetChar(remainder, (int)alphabetOffset);
                number /= (ulong)Digits.Length;
            }

            var result = new string(charArray, index + 1, BitsInLong - index - 1)
                .PadLeft(11, GetChar(0, (int)alphabetOffset));

            
            return result;
        }

        /// <summary>
        /// Конвертирует значение из Base58 в десятичное число. 
        /// </summary>
        /// <param name="value">Строковое значение в Base58.</param>
        /// <param name="alphabetOffset">Смещение алфавита.</param>
        /// <returns>Десятичное число.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FromBase58(this string value, uint alphabetOffset = 0)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз
            
            if (string.IsNullOrEmpty(value))
                return 0;

            if (alphabetOffset >= Digits.Length)
                throw new ArgumentException(
                    CommonPhrases.Exception_ParamIsTooLong.Format(nameof(alphabetOffset), Digits.Length));

            ulong result = 0;
            ulong multiplier = 1;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                var c = value[i];
                //Символ отсутствует или не принадлежит системе счисления. 
                if (!DigitDictionary.TryGetValue(c, out var digit) || digit >= Digits.Length)
                    throw new FormatException(CommonPhrases.Exception_InvalidCharInParam.Format(nameof(value), c));

                var digitIndex = GetIndex(digit, (int)alphabetOffset);
                
                result += (ulong)digitIndex * multiplier;
                multiplier *= (ulong)Digits.Length;
            }
            return result;
        }

        /// <summary>
        /// Преобразует шестнадцатеричную строку в десятичное положительное число. 
        /// </summary>
        /// <param name="hexString">Исходная шестнадцатеричная строка</param>
        /// <param name="defaultValue">Значение результата, в случае ошибки преобразования.</param>
        public static uint HexToUint(this string hexString, uint defaultValue = 0)
        {
            if (uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            try
            {
                return Convert.ToUInt32(hexString, 16);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Преобразует шестнадцатеричную строку в десятичное число. 
        /// </summary>
        /// <param name="hexString">Исходная шестнадцатеричная строка</param>
        /// <param name="defaultValue">Значение результата, в случае ошибки преобразования.</param>
        public static int HexToInt(this string hexString, int defaultValue = 0)
        {
            if (int.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            try
            {
                return Convert.ToInt32(hexString, 16);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Преобразовать массив байт в строку на основе шестнадцатеричного представления.
        /// </summary>
        public static string BytesToHex(this byte[] bytes)
        {
            return BytesToHex(bytes, 0, bytes?.Length ?? 0);
        }

        /// <summary>
        /// Преобразовать заданный диапазон массива байт в строку на основе шестнадцатеричного представления.
        /// </summary>
        /// <param name="bytes">Массив для преобразования.</param>
        /// <param name="index">Индекс первого элемента заданного диапазона.</param>
        /// <param name="count">Количество элементов диапазона.</param>
        public static string BytesToHex(this byte[] bytes, int index, int count)
        {
            var sb = new StringBuilder();
            var last = index + count;
            for (var i = index; i < last; i++)
                sb.Append(bytes[i].ToString("X2"));
            
            return sb.ToString();
        }

        /// <summary>
        /// Преобразовать строку шестнадцатеричных чисел в массив байт, используя существующий массив.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="strIndex">Смещение в исходной строке.</param>
        /// <param name="buf">Массив, в который помещается результат.</param>
        /// <param name="bufIndex">Смещение в результирующем массиве.</param>
        /// <param name="byteCount">Количество байт для преобразования.</param>
        /// <returns>True - в случае успешного преобразования, иначе - false.</returns>
        public static bool HexToBytes(this string str, int strIndex, byte[] buf, int bufIndex, int byteCount)
        {
            var strLen = str?.Length ?? 0;
            var convBytes = 0;

            while (strIndex < strLen && convBytes < byteCount)
            {
                try
                {
                    buf[bufIndex] = byte.Parse(str!.Substring(strIndex, 2), NumberStyles.AllowHexSpecifier);
                    bufIndex++;
                    convBytes++;
                    strIndex += 2;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            return convBytes > 0;
        }

        /// <summary>
        /// Преобразовать строку шестнадцатеричных чисел в массив байт, создав новый массив.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="buf">Массив, в который помещается результат.</param>
        /// <param name="isSkipWhiteSpace">Признак предварительного удаления пробелов из исходной строки.</param>
        /// <returns>True - в случае успешного преобразования, иначе - false.</returns>
        public static bool HexToBytes(this string str, out byte[] buf, bool isSkipWhiteSpace = false)
        {
            if (isSkipWhiteSpace)
                str = str.RemoveWhiteSpace();

            var strLen = str?.Length ?? 0;
            var bufLen = strLen / 2;
            buf = new byte[bufLen];
            
            return HexToBytes(str, 0, buf, 0, bufLen);
        }

        /// <summary>
        /// Преобразовать строку шестнадцатеричных чисел в массив байт, в случае ошибки - генерируется исключение.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="isSkipWhiteSpace">Признак предварительного удаления пробелов из исходной строки.</param>
        public static byte[] HexToBytes(this string str, bool isSkipWhiteSpace = false)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (HexToBytes(str, out var bytes, isSkipWhiteSpace))
                return bytes;

            throw new FormatException(CommonPhrases.Exception_ParamIsNotHex.Format(str));
        }        
    }
}
