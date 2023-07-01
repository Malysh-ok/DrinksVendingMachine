using System;
using System.Collections.Generic;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы-расширения для <see cref="ICollection{T}" /> и <see cref="IReadOnlyCollection{T}" />.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Проверка коллекции на наличие в ней элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="source">Коллекция.</param>
        /// <returns>Признак, есть ли в коллекции элементы.</returns>
        /// <exception cref="ArgumentNullException" />
        public static bool IsEmpty<T>(this ICollection<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Count == 0;
        }

        /// <summary>
        /// Проверка коллекции на наличие в ней элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="source">Коллекция.</param>
        /// <returns>Признак, есть ли в коллекции элементы.</returns>
        /// <exception cref="ArgumentNullException" />
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Count == 0;
        }
    }
}
