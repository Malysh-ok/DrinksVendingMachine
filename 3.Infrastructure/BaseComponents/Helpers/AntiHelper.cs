using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Infrastructure.BaseComponents.Helpers
{
    /// <summary>
    /// Содержит методы, которые нарушают общепринятые шаблоны.
    /// </summary>
    public static class AntiHelper
    {
        /// <summary>
        /// Установить private значение (isReadOnly) у ReadOnlyAttribute через Reflection.
        /// </summary>
        /// <param name="instance">Экземпляр.</param>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Требуемое значение у ReadOnlyAttribute.</param>
        /// <example> AntiHelper.UpdateReadOnlyAttribute(this, nameof(X), false);</example>
        public static void UpdateReadOnlyAttribute(object instance, string propertyName, bool value)
        {
            if (instance == null) throw new ArgumentException(nameof(instance));
            
            const string fieldName = "isReadOnly";
            var propertyDescriptor = TypeDescriptor.GetProperties(instance.GetType())[propertyName];
            if (propertyDescriptor != null)
            {
                var readOnlyAttribute = propertyDescriptor.Attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
                if (readOnlyAttribute != null)
                {
                    var fieldToChange = readOnlyAttribute.GetType()
                        .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldToChange != null)
                    {
                        fieldToChange.SetValue(readOnlyAttribute, value);
                    }
                }
            }
        }

        /// <summary>
        /// Установить private значение (isReadOnly) у всех ReadOnlyAttribute через Reflection.
        /// </summary>
        /// <param name="instance">Экземпляр.</param>
        /// <param name="value">Требуемое значение у ReadOnlyAttribute.</param>
        /// <example> AntiHelper.UpdateReadOnlyAttribute(this, nameof(X), false);</example>
        public static void UpdateAllReadOnlyAttributes(object instance, bool value)
        {
            const string fieldName = "isReadOnly";
            var propertyDescriptors = TypeDescriptor.GetProperties(instance.GetType());
            foreach (var property in propertyDescriptors)
            {
                if (property is PropertyDescriptor propertyDescriptor)
                {
                    var readOnlyAttribute = propertyDescriptor.Attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
                    if (readOnlyAttribute != null)
                    {
                        var fieldToChange = readOnlyAttribute.GetType()
                            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldToChange != null)
                        {
                            fieldToChange.SetValue(readOnlyAttribute, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Установить private значение (isReadOnly) у всех ReadOnlyAttribute через Reflection.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="value">Требуемое значение у ReadOnlyAttribute.</param>
        /// <example> AntiHelper.UpdateReadOnlyAttribute(this, nameof(X), false);</example>
        public static void UpdateAllReadOnlyAttributes(Type type, bool value)
        {
            const string fieldName = "isReadOnly";
            var propertyDescriptors = TypeDescriptor.GetProperties(type);
            foreach (var property in propertyDescriptors)
            {
                if (property is PropertyDescriptor propertyDescriptor)
                {
                    var readOnlyAttribute = propertyDescriptor.Attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
                    if (readOnlyAttribute != null)
                    {
                        var fieldToChange = readOnlyAttribute.GetType()
                            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldToChange != null)
                        {
                            fieldToChange.SetValue(readOnlyAttribute, value);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Установить private значение (browsable) у BrowsableAttribute через Reflection.
        /// </summary>
        /// <param name="instance">Экземпляр.</param>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Требуемое значение у BrowsableAttribute.</param>
        /// <example> AntiHelper.UpdateBrowsableAttribute(this, nameof(X), false);</example>
        public static void UpdateBrowsableAttribute(object instance, string propertyName, bool value)
        {
            const string fieldName = "browsable";
            var propertyDescriptor = TypeDescriptor.GetProperties(instance.GetType())[propertyName];
            if (propertyDescriptor != null)
            {
                var browsableAttribute = propertyDescriptor.Attributes.OfType<BrowsableAttribute>().FirstOrDefault();
                if (browsableAttribute != null)
                {
                    var fieldToChange = browsableAttribute.GetType()
                        .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldToChange != null)
                    {
                        fieldToChange.SetValue(browsableAttribute, value);
                    }
                }
            }
        }
    }
}
