using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.BaseComponents.Helpers
{
    /// <summary>
    /// Методы, помогающие работать с объектами, поддерживающими сравнение (<see cref="IComparable"/>).
    /// </summary>
    public static class CompareHelper
    {
        /// <summary>
        /// Вычисление минимальной сущности на основе IComparable.
        /// </summary>
        public static TComparable Min<TComparable>(IList<TComparable> comparables) where TComparable : IComparable
        {
            var min = comparables.FirstOrDefault();
            for (var i = 1; i< comparables.Count; i++)
            {
                if (comparables[i] != null)
                {
                    if (comparables[i].CompareTo(min) < 0)
                    {
                        min = comparables[i];
                    }
                }
            }
            return min;
        }
        
        /// <summary>
        /// Вычисление максимальной сущности на основе IComparable.
        /// </summary>
        public static TComparable Max<TComparable>(IList<TComparable> comparables) where TComparable : IComparable
        {
            var max = comparables.FirstOrDefault();
            for (var i = 1; i< comparables.Count; i++)
            {
                if (comparables[i] != null)
                {
                    if (comparables[i].CompareTo(max) > 0)
                    {
                        max = comparables[i];
                    }
                }
            }
            return max;
        }
    }
}