using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларирования максимального количества элементов в коллекции.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MaxOccursAttribute : Attribute
    {
        /// <inheritdoc />
        public MaxOccursAttribute(int maxOcc)
        {
            MaxOccurs = maxOcc;
        }

        /// <summary>
        /// Максимальное количество элементов.
        /// </summary>
        public int MaxOccurs { get; }
    }
}