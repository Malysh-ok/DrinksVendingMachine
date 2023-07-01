using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Определяет нижнюю границу для числовых значений (значение должно быть меньше указанного здесь).
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MinExclusiveAttribute : Attribute
    {
        /// <inheritdoc />
        public MinExclusiveAttribute(int minExclusive)
        {
            MinExclusive = minExclusive;
        }

        /// <summary>
        /// Нижняя граница для числовых значений.
        /// </summary>
        public int MinExclusive { get; }
    }
}
