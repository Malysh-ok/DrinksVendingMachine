using System;

namespace Infrastructure.BaseComponents.Components.Unicode
{
    /// <summary>
    /// Блок (непрерывный диапазон) из кодовых точек Юникода.
    /// </summary>
    public class UnicodeRange
    {
        /// <summary>
        /// Первая кодовая точка в диапазоне.
        /// </summary>
        public int FirstCodePoint { get; private set; }

        /// <summary>
        /// Кол-во кодовых точек в диапазоне.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public UnicodeRange()
        {
            FirstCodePoint = 0;
            Length = UInt16.MaxValue;
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UnicodeRange(int firstCodePoint, int length)
        {
            if (firstCodePoint < 0 || length < 0 ||
                    firstCodePoint > UInt16.MaxValue || firstCodePoint + length > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException();

            FirstCodePoint = firstCodePoint;
            Length = length;
        }

        /// <summary>
        /// Создает новый экземпляр UnicodeRange из диапазона символов.
        /// </summary>
        public static UnicodeRange Create(char firstCharacter, char lastCharacter)
        {
            var firstCodePoint = Convert.ToUInt16(firstCharacter);
            var lastCodePoint = Convert.ToUInt16(lastCharacter);
            
            return new UnicodeRange(firstCodePoint, lastCodePoint - firstCodePoint);
        }
    }
}
