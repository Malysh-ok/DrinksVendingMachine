using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для работы с механизмом рефлексии C#.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Получить значение атрибута указанного типа из указанной сущности.
        /// </summary>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        /// <param name="memberInfo">Описатель типа сущности, из которой извлекается атрибут.</param>
        /// <param name="inherit">Извлекать ли наследника указанного атрибута
        /// (если вместо указанного типа атрибута найден его наследник).</param>
        /// <returns>Атрибут ожидаемого типа.</returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
            where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(TAttribute));
        }

        /// <summary>
        /// Получить коллекции атрибутов указанного типа из указанной сущности.
        /// </summary>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        /// <param name="memberInfo">Описатель типа сущности, из которой извлекаются атрибуты.</param>
        /// <param name="inherit">Извлекать ли наследников указанных атрибутов
        /// (если вместо указанного типа атрибута найдётся его наследник).</param>
        /// <returns>Коллекция атрибутов ожидаемого типа.</returns>
        public static IReadOnlyCollection<TAttribute> GetAttributes<TAttribute>(this MemberInfo memberInfo, 
            bool inherit = true)
            where TAttribute : Attribute
        {
            return (IReadOnlyCollection<TAttribute>)Attribute.GetCustomAttributes(memberInfo, typeof(TAttribute));
        }

        /// <summary>
        /// Проверка, применим ли к типу <paramref name="type"/> атрибут типа <typeparamref name="T"/> или его наследника.
        /// </summary>
        /// <typeparam name="T">Тип атрибута для проверки.</typeparam>
        /// <param name="type">Тип, для которого требуется проверить применимость атрибута.</param>
        /// <param name="isInherit">Признак того, проверять ли применимость атрибута у наследников типа <paramref name="type"/>.</param>
        /// <returns>Результат проверки, применим ли наследник указанного типа к сущности.</returns>
        public static bool IsDefined<T>(this Type type, bool isInherit = true) where T : Attribute
        {
            return type.IsDefined(typeof(T), isInherit);
        }

        /// <summary>
        /// Создание generic-списка с указанным типизированным параметром.
        ///  </summary>
        /// <param name="type">Тип элемента списка.</param>
        /// <returns>Результирующий список.</returns>
        public static IList CreateTypedList(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var listType = typeof(List<>).MakeGenericType(type);
            
            return (IList)Activator.CreateInstance(listType);
        }

        /// <summary>
        /// Получить значение по умолчанию для указанного <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo"><see cref="PropertyInfo"/></param>
        /// <returns>Значение по умолчанию.</returns>
        public static object GetDefault(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsValueType 
                ? Activator.CreateInstance(propertyInfo.PropertyType) 
                : null;
        }
    }
}
