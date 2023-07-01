using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.BaseExtensions.Collections
{
    /// <summary>
    /// Методы расширения для <see cref="Queue"/>.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Проверяет очередь <see cref="Queue{T}"/> на "пустоту",
        /// и если она не пуста - удаляет объект из начала очереди и возвращает его.
        /// </summary>
        public static T TryDequeue<T>(this Queue<T> queue, T defaultResult = default)
        {
            return queue.IsEmpty() 
                ? defaultResult 
                : queue.Dequeue();
        }

    }
}