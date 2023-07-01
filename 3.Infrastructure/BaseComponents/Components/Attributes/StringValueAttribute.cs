using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Для указания строковых значений перечислений.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        /// <inheritdoc />
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Строковое значение.
        /// </summary>
        public string Value { get; set; }
    }
}
