using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Infrastructure.BaseComponents.Components.SystemInfo;
using Infrastructure.BaseExtensions;
using Infrastructure.BaseExtensions.Collections;

namespace Infrastructure.BaseComponents.Components.IO
{
    /// <summary>
    /// Кроссплатформенный класс для работы с путями.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class PathEx
    {
        /// <summary>
        /// Возвращает признак того, что путь состоит из валидных символов.
        /// </summary>
        public static bool IsValidPath(string pathString) =>
            !pathString.IsNullOrEmpty() && pathString.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        
        /// <summary>
        /// Возвращает признак того, что имя файла состоит из валидных символов.
        /// </summary>
        public static bool IsValidFileName(string fileName) =>
            !fileName.IsNullOrEmpty() && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        
        /// <summary>
        /// Возвращает символ разделителя директорий.
        /// </summary>
        public static char GetPathSeparatorChar()
        {
            return OsPlatformEx.IsUnix ? '/' : '\\';
        }
        
        /// <summary>
        /// Возвращает символ разделителя директорий в виде строки.
        /// </summary>
        public static string GetPathSeparatorString()
        {
            return OsPlatformEx.IsUnix ? "/" : "\\";
        }
        
        /// <summary>
        /// Объединить массив строк в путь.
        /// </summary>
        /// <remarks>
        /// Кроссплатформенная версия Path.Combine.
        /// </remarks>
        /// <param name="pathStrings">Массив строк для объединения в путь.</param>
        public static string Combine(params string[] pathStrings)
        {
            return Normalize(pathStrings.Aggregate(string.Empty, 
                (current, pathString) => AddLastPathSeparator(current) + RemoveFirstPathSeparator(pathString)));
        }

        /// <summary>
        /// Объединить массив строк в путь.
        /// </summary>
        /// <remarks>
        /// Кроссплатформенная версия Path.Combine.
        /// </remarks>
        /// <param name="isAddLastSeparator">Признак добавления конечного разделителя.</param>
        /// <param name="pathStrings">Массив строк для объединения в путь.</param>
        public static string Combine(bool isAddLastSeparator, params string[] pathStrings)
        {
            return Normalize(pathStrings.Aggregate(string.Empty, 
                    (current, pathString) => AddLastPathSeparator(current) + RemoveFirstPathSeparator(pathString)),
                isAddLastSeparator);
        }
        
        /// <summary>
        /// Скорректировать разделитель директории
        /// (замена разделителей на те, что предусмотрены текущей системой).
        /// </summary>
        public static string CorrectPathSeparator(string path, bool isAddLastSeparator = false) =>
            path.Replace('\\', GetPathSeparatorChar())
                .Replace('/', GetPathSeparatorChar())
                .Func(isAddLastSeparator, AddLastPathSeparator);

        /// <summary>
        /// Удаляет лишние разделители из строки с директорией.
        /// </summary>
        /// <param name="pathString">Исходный путь.</param>
        /// <param name="isAddLastSeparator">Признак добавления конечного разделителя.</param>
        public static string Normalize(string pathString, bool isAddLastSeparator = false)
        {
            if (pathString == null)
                throw new ArgumentNullException(nameof(pathString));

            var separator = GetPathSeparatorChar();
            var arr = pathString.Split(separator);
            var isSeparatorEndOfLine = arr[^1].IsNullOrWhiteSpace();
            var newArr = arr.RemoveEmptyStr();
            var newString = string.Join(separator, newArr);
            
            if (isAddLastSeparator)
                // Добавляем разделитель в конец пути, если был установлен признак
                newString += separator;
            else if (isSeparatorEndOfLine)
                // Добавляем разделитель в конец пути, если он был в исходной строке
                newString += separator;
            
            return newString;
        }
        
        /// <summary>
        /// Возвращает признак наличия символа разделителя директории в начале строки.
        /// </summary>
        public static bool IsFirstPathSeparator(string path)
        {
            return !path.IsNullOrEmpty() && path.First().Equals(GetPathSeparatorChar());
        }

        /// <summary>
        /// Возвращает признак наличия символа разделителя директорий в конце строки.
        /// </summary>
        public static bool IsLastPathSeparator(string path)
        {
            return !path.IsNullOrEmpty() && path.Last().Equals(GetPathSeparatorChar());
        }
        
        /// <summary>
        /// Добавляет символ разделителя директорий в конце строки пути, при необходимости.
        /// </summary>
        public static string AddLastPathSeparator(string pathString)
        {
            if (string.IsNullOrEmpty(pathString))
                return string.Empty;
            return string.Concat(pathString,
                !IsLastPathSeparator(pathString) ? GetPathSeparatorString() : string.Empty);
        }

        /// <summary>
        /// Добавляет символ разделителя директорий в начало строки, при необходимости.
        /// </summary>
        public static string AddFirstPathSeparator(string pathString)
        {
            if (pathString.IsNullOrEmpty())
                return string.Empty;
            if ((pathString.Length > 3 && pathString[1] == ':' && pathString[2] == GetPathSeparatorChar()) ||
                (OsPlatformEx.IsUnix && pathString[0] == GetPathSeparatorChar()))
                return pathString;
            return string.Concat(!IsFirstPathSeparator(pathString) 
                ? GetPathSeparatorString() 
                : string.Empty, pathString);
        }
        
        /// <summary>
        /// Удаляет символ разделителя директорий с начала строки.
        /// </summary>
        public static string RemoveFirstPathSeparator(string pathString)
        {
            return IsFirstPathSeparator(pathString) ? pathString.Remove(0, 1) : pathString;
        }

        /// <summary>
        /// Удаляет разделитель директории с конца строки.
        /// </summary>
        public static string RemoveLastPathSeparator(string pathString)
        {
            return IsLastPathSeparator(pathString) ? pathString.Remove(pathString.Length - 1) : pathString;
        }
    }
}