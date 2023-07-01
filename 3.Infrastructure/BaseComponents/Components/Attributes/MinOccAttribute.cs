using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларирования минимального количества элементов в коллекции.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]

    public class MinOccursAttribute : Attribute
    {
        /// <inheritdoc />
        public MinOccursAttribute(int minOcc)
        {
            MinOccurs = minOcc;
        }

        /// <summary>
        /// Минимальное количество элементов в коллекции.
        /// </summary>
        public int MinOccurs { get; }
    }
}
