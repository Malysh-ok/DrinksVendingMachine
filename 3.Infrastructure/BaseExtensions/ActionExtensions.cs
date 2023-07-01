using System;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="Action"/>.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Выполняет для <paramref name="element"/> вызов делегата <paramref name="action"/>,
        /// но только в том случае, если тип <paramref name="element"/> есть <typeparamref name="TTarget"/>.
        /// </summary>
        public static void Is<TTarget>(this object element, Action<TTarget> action) where TTarget : class
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (element is TTarget target)
            {
                action(target);
            }
        }

        /// <summary>
        /// Выполняет для <paramref name="element"/> вызов делегата <paramref name="action"/>,
        /// но только в том случае, если тип <paramref name="element"/> есть <typeparamref name="TTarget"/>.
        /// </summary>
        public static void Is<TTarget>(this ValueType element, Action<TTarget> action) where TTarget : struct
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (element is TTarget target)
            {
                action(target);
            }
        }
    }
}
