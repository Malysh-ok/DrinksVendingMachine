using System.Text;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="StringBuilder"/>.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Удаление символов перевода строки из конца текста, если они есть.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <param name="isAllDelete">Признак того, что нужно удалить все символы.
        /// Если равен false - удаляется только последний символ.</param>
        /// <returns>Измененный текст.</returns>
        public static StringBuilder RemoveTerminatingNewlineChars(this StringBuilder text, bool isAllDelete = true)
        {
            while(text.Length > 0)
            {
                var isLf = text[^1] == '\n';
                
                if (isLf || text[^1] == '\r')
                {
                    text.Remove(text.Length - 1, 1);
                    
                    if (text.Length > 0 && text[^1] == '\r')
                        text.Remove(text.Length - 1, 1);
                }
                else
                {
                    break;
                }

                if (!isAllDelete)
                    break;
            }

            return text;
        }
    }
}