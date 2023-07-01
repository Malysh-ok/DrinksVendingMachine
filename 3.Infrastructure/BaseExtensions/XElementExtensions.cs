using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="XElement"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class XElementExtensions
    {
        /// <summary>
        /// Перемещение указанных элементов в начало родительского элемента.
        /// </summary>
        public static void MoveFirst(this XElement parent, string name)
        {
            var foundElements = parent.Descendants().Where(elements => 
                elements.Name.LocalName.Equals(name)).ToList();
            for(var i = foundElements.Count - 1; i >= 0; i--)
            {
                var element = foundElements[i];
                if (element != null)
                {
                    element.Remove();
                    parent.AddFirst(element);
                }
            }
        }

        /// <summary>
        /// Сравнение имени элемента с параметром <paramref name="name"/>.
        /// </summary>
        /// <returns>True - если совпадают, иначе - false.</returns>
        public static bool CompareName(this XElement element, string name)
        {
            return element.Name.LocalName.Equals(name);
        }

        
        /// <summary>
        /// Поиск и получение элемента среди всех потомков по его имени.
        /// </summary>
        public static XElement GetDescendantFromName(this XElement parent, string name)
        {
            return parent.Descendants().FirstOrDefault(elements =>
                elements.Name.LocalName.Equals(name));
        }
        
        /// <summary>
        /// Поиск и получение элемента среди дочерних элементов (потомков первого уровня) по его имени.
        /// </summary>
        public static XElement GetChildFromName(this XElement parent, string name)
        {
            return parent.Elements().FirstOrDefault(elements =>
                elements.Name.LocalName.Equals(name));
        }
    }
}
