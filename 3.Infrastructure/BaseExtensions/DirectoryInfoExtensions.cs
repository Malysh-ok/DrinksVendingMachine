using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="DirectoryInfo" />.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Размер директории в байтах.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static long DirSize(this DirectoryInfo di, bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            
            return di.EnumerateFiles("*", searchOption).Sum(fi => fi.Length);
        }
        
        /// <summary>
        /// Количество файлов в директории.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static int FileCount(this DirectoryInfo di, bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            
            return di.EnumerateFiles("*", searchOption).Count();
        }
        
        /// <summary>
        /// Получение последовательности файлов директории.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo di, bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            
            return di.EnumerateFiles("*", searchOption);
        }

        /// <summary>
        /// Получение последовательности файлов директории, старше определенной даты/времени создания.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="creationDt">Дата/время, относительно которой ищутся более старые файлы.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static IEnumerable<FileInfo> GetOldFiles(this DirectoryInfo di, DateTime creationDt,
            bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return di.EnumerateFiles("*", searchOption)
                .Where(fi => fi.CreationTime < creationDt);
        }

        /// <summary>
        /// Получение последовательности самых старых файлов директории; 
        /// при этом суммарный размер оставшихся файлов не превышает <paramref name="maxDirectoryCapacity"/>.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="maxDirectoryCapacity">Максимально допустимая вместимость директории в байтах.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static IEnumerable<FileInfo> GetOldFiles(this DirectoryInfo di, long maxDirectoryCapacity,
            bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var dirSize = di.DirSize(isIncludesSubDir);
            long sum = 0;

            return di.EnumerateFiles("*", searchOption)
                .OrderBy(fi => fi.CreationTime)
                .TakeWhile(fi => dirSize - (sum += fi.Length) > maxDirectoryCapacity);
        }

        /// <summary>
        /// Получение последовательности самых старых файлов директории; 
        /// при этом суммарное количество оставшихся файлов не превышает <paramref name="maxFileCount"/>.
        /// </summary>
        /// <param name="di">Исходный объект данных о директории.</param>
        /// <param name="maxFileCount">Максимально допустимое количество файлов в директории.</param>
        /// <param name="isIncludesSubDir">Признак, указывающий на то, что учитываются данные всех подкаталогов.</param>
        public static IEnumerable<FileInfo> GetOldFiles(this DirectoryInfo di, int maxFileCount,
            bool isIncludesSubDir = true)
        {
            var searchOption = isIncludesSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var fileCount = di.FileCount(isIncludesSubDir);
            
            return di.EnumerateFiles("*", searchOption)
                .OrderBy(fi => fi.CreationTime)
                .Take(fileCount - maxFileCount);
        }
    }
}