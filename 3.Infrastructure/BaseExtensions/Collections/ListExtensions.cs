using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы-расширения для <see cref="IList{T}" /> и <see cref="IReadOnlyList{T}" />.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ListExtensions
    {
        private static readonly Random RANDOM = new Random((int)DateTime.Now.Ticks);
        
        /// <summary>
        /// Получить обобщенный список.
        /// </summary>
        /// <remarks> Непонятно, для чего это нужно. </remarks>
        public static object CreateObjectList<T>()
        {
            var obj = Activator.CreateInstance(typeof(T));
            Type listItemType = obj.GetType().GetGenericArguments().SingleOrDefault();

            return obj is System.Collections.IEnumerable 
                ? Activator.CreateInstance(typeof(List<>).MakeGenericType(listItemType)) 
                : new List<T>();
        }

        /// <summary>
        /// Проверка списка на наличие в нём элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <returns>Признак, есть ли в списке элементы.</returns>
        public static bool IsEmpty<T>(this List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return list.Count == 0;
        }

        /// <summary>
        /// Получение случайного элемента из указанного списка.
        /// </summary>
        /// <typeparam name="T">Тип элемента списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <returns>Случайный элемент.</returns>
        public static T Random<T>(this IList<T> list)
        {
            return list[RANDOM.Next(list.Count)];
        }

        /// <summary>
        /// Получение случайного элемента из указанного списка.
        /// </summary>
        /// <typeparam name="T">Тип элемента списка.</typeparam>
        /// <param name="list">Исходный список.</param>
        /// <returns>Случайный элемент.</returns>
        public static T Random<T>(this IReadOnlyList<T> list)
        {
            return list[RANDOM.Next(list.Count)];
        }

        /// <summary>
        /// Выполняет поиск заданного элемента в упорядоченном списке (или его части), 
        /// используя для этого универсальный интерфейс IComparable&lt;<typeparamref name="T"/>&gt;,
        /// реализуемый каждым элементом и заданным объектом.
        /// </summary>
        /// <typeparam name="T">Тип элемента упорядоченного списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="value">Элемент, индекс которого требуется найти.</param>
        /// <param name="index">Начальный индекс диапазона поиска
        /// (при передаче null поиск осуществляется с минимального индекса).</param>
        /// <param name="length">Длина диапазона поиска
        /// (при передаче null поиск осуществляется до максимального индекса).</param>
        /// <param name="comparer">Реализация интерфейса IComparer&lt;<typeparamref name="T"/>&gt;,
        /// которая используется при сравнении элементов.
        /// (при передаче null используется компаратор по умолчанию).</param>
        /// <returns>Найденный элемент.</returns>
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T value,
            int? index = null, int? length = null, IComparer<T> comparer = null)
        {
            var left = index ?? 0;
            var right = left + (length ?? list.Count - 1);
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be nonnegative");
            if (left >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must not exceed the number of elements");
            if (right >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(length), "Length exceeds the remained count of elements");
            comparer ??= Comparer<T>.Default;

            while (left < right)
            {
                var med = (left >> 1) + (right >> 1) + left & right & 1;
                var compare = comparer.Compare(list[med], value);

                if (compare > 0)
                    left = med + 1;
                else if (compare < 0)
                    right = med;
                else
                    return med;
            }

            if (right < list.Count && comparer.Compare(list[right], value) == 0)
                return right;

            return ~left;
        }

        /// <summary>
        /// Добавление нового элемента в упорядоченный список. 
        /// </summary>
        /// <typeparam name="T">Тип элемента упорядоченного списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="item">Добавляемый элемент.</param>
        /// <param name="comparer">Компаратор для сравнения элементов списка.</param>
        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            var i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }

        /// <summary>
        /// Обмен местами двух элементов списка.
        /// </summary>
        /// <typeparam name="T">Тип элемента списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="index1">Индекс первого элемента для обмена.</param>
        /// <param name="index2">Индекс второго элемента для обмена.</param>
        /// <param name="rangeCheck">Осуществлять ли внутри метода проверку на выход за пределы индексов списка.</param>
        public static void Swap<T>(this IList<T> list, int index1, int index2, bool rangeCheck = true)
        {
            if (rangeCheck)
            {
                if (index1 < 0)
                    throw new ArgumentOutOfRangeException(nameof(index1), "Index1 must be nonnegative");
                if (index2 < 0)
                    throw new ArgumentOutOfRangeException(nameof(index2), "Index2 must be nonnegative");
                if (index1 >= list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index1),
                        "Index1 must not exceed the number of elements");
                if (index2 >= list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index2),
                        "Index2 must not exceed the number of elements");
            }

            (list[index1], list[index2]) = (list[index2], list[index1]);
        }

        /// <summary>
        /// Работа как со стеком: "засунуть" элемент.
        /// </summary>
        /// <typeparam name="T">Тип элемента списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="item">Добавляемый элемент.</param>
        /// <param name="capacity">Вместимость списка.</param>
        public static void Push<T>(this IList<T> list, T item, int capacity = -1)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            list.Insert(0, item);

            if (capacity != -1)
            {
                for (var i = list.Count-1; i >= capacity; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }
   
        /// <summary>
        /// Работа как со стеком: "вытащить" элемент.
        /// </summary>
        /// <typeparam name="T">Тип элемента списка.</typeparam>
        /// <param name="list">Список.</param>
        /// <returns>Получаемый элемент.</returns>
        public static T Pop<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
        
            if (list.Count > 0)
            {
                var firstItem = list[0];
                list.RemoveAt(0);
                return firstItem;
            }

            return default(T);
        }

        /// <summary>
        /// Получить индекс элемента в списке.
        /// </summary>
        /// <typeparam name="T">Тип элемента.</typeparam>
        /// <param name="list">Список.</param>
        /// <param name="item">Элемент.</param>
        /// <returns>Индекс элемента в списке.</returns>
        public static int GetIndex<T>(this IList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Получение ключа SortedList по значению.
        /// </summary>
        /// <param name="this">Список SortedList, с которым происходит работа. </param>
        /// <param name="value">Значение списка, к которому необходимо получить ключ.</param>
        /// <param name="key">Получаемый ключ.</param>
        /// <returns>True - если ключ найден, false - в противном случае.</returns>
        public static bool KeyByValue<TKey, TValue>
            (this SortedList<TKey, TValue> @this, TValue value, out TKey key)
        {
            var index = @this.IndexOfValue(value);
            
            if (index >= 0)
            {
                key = @this.Keys[index];
                return true;
            }
            
            key = default;
            return false;
        }
    }
}
