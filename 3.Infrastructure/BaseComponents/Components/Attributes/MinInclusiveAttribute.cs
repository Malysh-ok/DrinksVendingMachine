using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут для декларирования минимального приемлемого значения.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]

    public class MinInclusiveAttribute : Attribute
    {
        /// <inheritdoc />
        public MinInclusiveAttribute(int minInclusive)
        {
            MinInclusive = minInclusive;
        }

        /// <summary>
        /// Минимальное приемлемое значение.
        /// </summary>
        public int MinInclusive { get; }
    }
}