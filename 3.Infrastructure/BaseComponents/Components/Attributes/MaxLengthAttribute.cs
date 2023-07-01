using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларирования максимальной длины.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]

    public class MaxLengthAttribute : Attribute
    {
        /// <inheritdoc />
        public MaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }

        /// <summary>
        /// Максимальная длина.
        /// </summary>
        public int MaxLength { get; }
    }
}
