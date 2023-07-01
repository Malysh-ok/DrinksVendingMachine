using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларирования минимальной длины.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MinLengthAttribute : Attribute
    {
        /// <inheritdoc />
        public MinLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }

        /// <summary>
        /// Минимальная длина. 
        /// </summary>
        public int MinLength { get; }
    }
}
