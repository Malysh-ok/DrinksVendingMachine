using System;
using System.Collections.ObjectModel;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы-расширения для <see cref="ObservableCollection{T}" />.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Проверка коллекции на наличие в ней элементов.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="source">Коллекция.</param>
        /// <returns>Признак, есть ли в коллекции элементы.</returns>
        /// <exception cref="ArgumentNullException" />
        public static bool IsEmpty<T>(this ObservableCollection<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Count == 0;
        }
    }
}
