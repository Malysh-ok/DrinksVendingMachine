using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы расширения для классов <see cref="HashSet{T}"/>.
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Добавляет последовательность во множество.
        /// </summary>
        public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            var allAdded = true;
            foreach (var item in items)
            {
                allAdded &= @this.Add(item);
            }
            return allAdded;
        }

        /// <summary>
        /// Создает множество из всех значений перечисления.
        /// </summary>
        public static HashSet<TEnum> FromEnum<TEnum>(this HashSet<TEnum> @this)
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToHashSet();
        }
    }
}