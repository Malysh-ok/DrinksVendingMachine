using System;
using System.Threading;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы-расширения для <see cref="EventHandler{T}"/>.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Безопасный вызов делегатов почти по Рихтеру.
        /// </summary>
        /// <typeparam name="TEventArgs">Тип аргументов события.</typeparam>
        /// <param name="handler">Делегат.</param>
        /// <param name="sender">Объект создавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            Volatile.Read(ref handler)?.Invoke(sender, e);
        }

        /// <summary>
        /// Безопасный вызов делегатов почти по Рихтеру.
        /// </summary>
        /// <param name="handler">Делегат.</param>
        /// <param name="sender">Объект создавший событие.</param>
        public static void Raise(this EventHandler handler, object sender)
        {
            Volatile.Read(ref handler)?.Invoke(sender, EventArgs.Empty);
        }
    }
}
