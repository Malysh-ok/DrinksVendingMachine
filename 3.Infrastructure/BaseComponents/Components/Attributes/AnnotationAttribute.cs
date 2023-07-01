using System;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Модель стандарта IEC 61850.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AnnotationAttribute : Attribute
    {
        /// <summary>
        /// Модель стандарта IEC 61850.
        /// </summary>
        /// <param name="annotation">Описание.</param>
        /// <param name="part">Часть и пункт стандарта.</param>
        public AnnotationAttribute(string annotation, string part = "")
        {
            Annotation = annotation;
            Part = part;
        }

        /// <summary>
        /// Описание модели по стандарту.
        /// </summary>
        public string Annotation { get; }

        /// <summary>
        /// Номер части стандарта и пункт ( Часть 7-2 (Пункт 1.1) ).
        /// </summary>
        public string Part { get;  }
    }
}
