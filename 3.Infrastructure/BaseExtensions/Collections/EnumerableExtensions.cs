using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы-расширения для <see cref="IEnumerable{T}" />.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Дополнение байтов до четного количества пустым символом строки.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        public static IList<byte> AddEmptyToEven(this IEnumerable<byte> array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var list = new List<byte>(array);
            if (list.Count % 2 != 0)
                list.Add(32); // " " Дополняем до четного пустым символом
            return list;
        }
        
        /// <summary>
        /// Возврат ссылки на коллекцию (если передаваемый объект реализует интерфейс коллекции).
        /// </summary>
        /// <remarks>Если класс, реализующий последовательность, реализует интерфейс
        /// <see cref="ICollection{T}" />, то пересоздаваться он не будет.
        /// В противном случае класс принудительно преобразуется к списку.</remarks>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <returns>Коллекция с элементами указанной последовательности.</returns>
        /// <exception cref="ArgumentNullException" />
        public static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source as ICollection<T> ?? source.ToList();
        }
        
        /// <summary>
        /// Преобразование последовательности в список.
        /// </summary>
        /// <remarks>Если класс, реализующий последовательность, является списком, пересоздаваться он не будет.</remarks>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <returns>Список с элементами указанной последовательности.</returns>
        /// <exception cref="ArgumentNullException" />
        public static List<T> AsList<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source as List<T> ?? source.ToList();
        }
        
        /// <summary>
        /// Присоединяет к первой последовательности вторую, а так же массив последовательностей.
        /// </summary>
        /// <param name="first">Первая последовательность.</param>
        /// <param name="second">Вторая последовательность.</param>
        /// <param name="additionalItems">Массив последовательностей.</param>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <returns>Результирующая последовательность.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second, 
            params IEnumerable<T>[] additionalItems)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (additionalItems == null)
                throw new ArgumentNullException(nameof(additionalItems));
            first = Enumerable.Concat(first, second);
            additionalItems.ForEach(item => first = first.Concat(item));
            return first;
        }
        
        /// <summary>
        /// Выполнение некоторого действия с элементами последовательности.
        /// </summary>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <param name="action">Выполняемое действие.</param>
        /// <remarks>Если действие не указано, метод просто итерирует по всем элементам последовательности.</remarks>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action = null)
        {
            if (action == null)
            {
                // Ничего не делаем, только пробегаемся по списку
                using var enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                }
            }
            else
            {
                foreach (var element in source)
                    action(element);
            }
        }

        /// <summary>
        /// Преобразует последовательность элементов в последовательность кортежей.
        /// (см. <see cref="WithIndex{T}(IEnumerable{T})"/>),
        /// и выполняет делегат <paramref name="action"/> над каждым элементом новой последовательности;<br/>
        /// в качестве параметров делегата выступают второе и первое поле кортежа.
        /// </summary>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <param name="action">Делегат, выполняемый над элементами результирующей последовательности.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            source.WithIndex().ForEach(pair => action(pair.Item2, pair.Item1));
        }

        /// <summary>
        /// Удаление из последовательности пустых строк.
        /// </summary>
        public static IEnumerable<string> RemoveEmptyStr(this IEnumerable<string> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Where(s => !s.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Отрезает часть последовательности.
        /// </summary>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <param name="startIndex">Начало отреза</param>
        /// <param name="size">Количество элементов для обрезания.</param>
        /// <returns>Преобразованная последовательность.</returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var enumerable = source as IList<T> ?? source.ToList();
            var num = enumerable.Count;
            if (startIndex < 0 || num < startIndex)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (size < 0 || startIndex + size > num)
                throw new ArgumentOutOfRangeException(nameof(size));
            return enumerable.Skip(startIndex).Take(size);
        }

        /// <summary>
        /// Преобразование в последовательность.
        /// </summary>
        /// <param name="element">Элемент для преобразования.</param>
        /// <typeparam name="T">Тип элемента.</typeparam>
        /// <returns>Результирующая последовательность.</returns>
        public static IEnumerable<T> ToSequence<T>(this T element)
        {
            yield return element;
        }

        /// <summary>
        /// Преобразование в последовательность.
        /// </summary>
        /// <param name="element">Элемент для преобразования.</param>
        /// <param name="additional">Массив элементов, добавляемых в результирующую последовательность.</param>
        /// <typeparam name="T">Тип элемента.</typeparam>
        /// <returns>Результирующая последовательность.</returns>
        public static IEnumerable<T> ToSequence<T>(T element, params T[] additional)
        {
            yield return element;
            foreach (T obj in additional)
                yield return obj;
        }
        
        /// <summary>
        /// Преобразование последовательности элементов в последовательность кортежей,
        /// где первое поле кортежа - индекс (отсчитываемый с нуля) элемента последовательности,
        /// второе поле - элемент исходной последовательности.
        /// </summary>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <returns>Результирующая последовательность кортежей.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Tuple<int, T>> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            var position = 0;
            return source.Select(value => Tuple.Create(position++, value));
        }

        /// <summary>
        /// Преобразование последовательности элементов в строку.
        /// </summary>
        /// <typeparam name="T">Тип элемента последовательности.</typeparam>
        /// <param name="source">Исходная последовательность.</param>
        /// <param name="delimiter">Разделитель между преобразованными элементами.</param>
        /// <returns>Результирующая строка.</returns>
        public static string ToStringOfValues<T>(this IEnumerable<T> source, string delimiter = ",")
        {
            var sb = new StringBuilder();
            foreach (var item in source)
            {
                sb.Append($"{item.ToString()}{delimiter}");
            }
            if (sb.Length > 0)
                sb.Length--;

            return sb.ToString();
        }
    }
}
