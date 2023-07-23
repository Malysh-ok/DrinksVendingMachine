using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="Array" />.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
    public static class ArrayExtensions
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static readonly Random RANDOM = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Получение массива псевдослучайных байтов.
        /// </summary>
        /// <param name="initValue">Начальное значение инициализации генерируемого массива.</param>
        /// <param name="capacity">Вместимость.</param>
        public static byte[] RandomByteArray(uint initValue, int capacity)
        {
            var nextRnd = initValue;
            var arr = new byte[capacity];
            for (var i = 0; i < arr.Length; i++)
            {
                nextRnd *= 1103515245;
                var rnd = ((nextRnd / 65536) * 2768) % (255 - 0 + 1) + 255;
                arr[i] = (byte)rnd;
            }

            return arr;
        }
        
        /// <summary>
        /// Получение массива псевдослучайных символов.
        /// </summary>
        /// <param name="capacity">Вместимость.</param>
        public static char[] RandomCharArray(int capacity)
        {
            var arr = new char[capacity];
            for (var i = 0; i < arr.Length; i++)
            {
                var num = RANDOM.Next(Int16.MaxValue);
                arr[i] = Convert.ToChar(num);
            }

            return arr;
        }
        
        /// <summary>
        /// Получение массива целых положительных чисел.
        /// </summary>
        /// <param name="capacity">Вместимость.</param>
        public static int[] RandomIntArray(int capacity)
        {
            var arr = new int[capacity];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = RANDOM.Next();
            }

            return arr;
        }
        
        /// <summary>
        /// Поиск параметров командной строки в массиве аргументов.
        /// </summary>
        /// <remarks>
        /// Параметры командной строки имеют формат: "-название значение"
        /// (название может также начинаться с "--" или "/").
        /// При поиске игнорируется регистр символов.
        /// </remarks>
        /// <param name="source">Исходный массив.</param>
        /// <returns>Последовательность найденных параметров в формате <see cref="KeyValuePair{T,T}"/>
        /// (название параметра, значение).</returns>
        public static IEnumerable<KeyValuePair<string, string>> FindAllArgs(this string[] source)
        {
            //TODO: сделать удаление кавычек в значениях параметров.
            var lst = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < source.Length; i++)
            {
                var pair = source[i].ToPairedValue("--");
                if (pair.Value is null) pair = source[i].ToPairedValue("-");
                if (pair.Value is null) pair = source[i].ToPairedValue("/");

                if (pair.Key.IsNullOrEmpty() && pair.Value is not null)
                {
                    lst.Add(new KeyValuePair<string, string>(
                        pair.Value,
                        i < source.Length - 1
                            ? source[i + 1]
                            : string.Empty
                    ));
                }
            }
            return lst;
        }

        /// <summary>
        /// Поиск параметра командной строки в массиве аргументов.
        /// </summary>
        /// <remarks>
        /// См. <see cref="FindAllArgs"/>
        /// </remarks>
        /// <param name="source">Исходный массив.</param>
        /// <param name="argKey">Название искомого параметра.</param>
        /// <returns>Найденное значение параметра (null, если не нашли).</returns>
        public static string FindArg(this string[] source, string argKey)
        {
            return FindAllArgs(source)
                .LastOrDefault(p => p.Key.Equals(argKey, 
                    StringComparison.CurrentCultureIgnoreCase)).Value;
        }

        /// <summary>
        /// Проверка массива на наличие в нем элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="source">Исходный массив.</param>
        /// <returns>Признак, есть ли в массиве элементы.</returns>
        /// <exception cref="ArgumentNullException" />
        public static bool IsEmpty<T>(this T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Length == 0;
        }

        /// <summary>
        /// Получить копию части массива.
        /// </summary>
        /// <typeparam name="T">Тип элементов массива.</typeparam>
        /// <param name="data">Исходный массив.</param>
        /// <param name="index">Индекс первого выбираемого элемента в исходном массиве.</param>
        /// <param name="length">Количество выбираемых элементов из исходного массива.</param>
        public static T[] SubArray<T>(this T[] data, long index, long length)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture;       // устанавливаем яз. стандарт для фраз

            if (data.Length < length + index)
                throw new ArgumentException(
                    CommonPhrases.Exception_ArrayBoundsError);
            
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            
            return result;
        }

        /// <summary>
        /// Получение строки из из массива байтов.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        /// <param name="encoding">Кодировка получаемой строки.</param>
        /// <param name="startIndex">Индекс начального элемента массива.</param>
        /// <param name="length">Индекс конечного элемента массива.</param>
        public static string ToStr(this byte[] array, Encoding encoding = null, 
            int startIndex = 0, int length = int.MaxValue)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            
            if ((uint)startIndex + length > array.Length)
                length = array.Length - startIndex;
            
            encoding ??= Encoding.UTF8;

            return encoding.GetString(array, startIndex, length);
        }

        /// <summary>
        /// Получение строки из из массива символов.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        public static string ToStr(this char[] array)
        {
            return new string(array);
        }

        
        /// <summary>
        /// Получение строки из массива ushort.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        /// <param name="encoding">Кодировка получаемой строки.</param>
        public static string ToStr(this ushort[] array, Encoding encoding = null)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var bytes = new List<byte>();
            foreach (var val in array)
            {
                bytes.Add((byte)val);
                bytes.Add((byte)(val >> 8));
            }

            return bytes.ToArray().ToStr(encoding);
        }
        
        /// <summary>
        /// Оптимизированный unsafe-метод сравнения массивов от Хафора Стефансона.
        /// </summary>
        /// <remarks>
        /// Использовать для быстрого сравнения средних и больших (от 10 байт) массивов.<br/>
        /// См. Выводы https://habr.com/en/post/214841/.
        /// </remarks>
        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        public static unsafe bool UnsafeCompare(this byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*) x1) != *((long*) x2))
                        return false;
                if ((l & 4) != 0)
                {
                    if (*((int*) x1) != *((int*) x2)) return false;
                    x1 += 4;
                    x2 += 4;
                }
                if ((l & 2) != 0)
                {
                    if (*((short*) x1) != *((short*) x2)) return false;
                    x1 += 2;
                    x2 += 2;
                }
                if ((l & 1) != 0)
                    if (*((byte*) x1) != *((byte*) x2))
                        return false;
                return true;
            }
        }
    }
}
