using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларации максимальной длины имени.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class NameMaxLengthAttribute : Attribute
    {
        public NameMaxLengthAttribute(int nameMaxLength)
        {
            NameMaxLength = nameMaxLength;
        }

        /// <summary>
        /// Максимальная длина имени.
        /// </summary>
        public int NameMaxLength { get; }
    }
}
