using System;
using System.Collections.Generic;
using System.Linq;

//https://stackoverflow.com/questions/8868119/find-all-parent-types-both-base-classes-and-interfaces

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Возвращает родительские типы и интерфейсы.
        /// </summary>
        public static IEnumerable<Type> GetParentTypes(this Type type, bool typesOnly = false)
        {
            // is there any base type?
            if (type == null)
            {
                yield break;
            }

            if (!typesOnly)
            {
                // return all implemented or inherited interfaces
                foreach (var i in type.GetInterfaces())
                {
                    yield return i;
                }
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        /// <summary>
        /// Проверка на наследование.
        /// </summary>
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            // null does not have base type
            if (type == null)
            {
                return false;
            }

            // only interface or object can have null base type
            if (baseType == null)
            {
                return type.IsInterface || type == typeof(object);
            }

            // check implemented interfaces
            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
