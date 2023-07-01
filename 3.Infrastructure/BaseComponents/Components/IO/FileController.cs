using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Infrastructure.BaseExtensions;
using Infrastructure.BaseExtensions.Collections;

namespace Infrastructure.BaseComponents.Components.IO
{
    /// <summary>
    /// Контроллер файлов, отслеживающий определенную папку (директорию)
    /// с целью определения по различным критериям устаревших файлов с возможностью их удаления.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class FileController
    {
        /// <summary>
        /// Количество байт в мегабайте.
        /// </summary>
        private const int MEGABYTE = 1048576;
        
        /// <summary>
        /// Контролируемая папка (директория).
        /// </summary>
        public DirectoryInfo Folder { get; }

        /// <summary>
        /// Максимальное количество файлов в папке.
        /// </summary>
        public int MaxFileCount { get; private set; }

        /// <summary>
        /// Максимальная вместимость папки в мегабайтах.
        /// </summary>
        public long MaxCapacity { get; private set; }
        
        /// <summary>
        /// Дата/время создания самого старого файла.
        /// </summary>
        public DateTime OldestFileCreationDate { get; private set; }
        
        /// <summary>
        /// Коллекция (множество) всех файлов.
        /// </summary>
        public HashSet<FileInfo> Files { get; private set; }
        
        /// <summary>
        /// Коллекция (множество) устаревших файлов.
        /// </summary>
        /// <remarks>
        /// Заполняется методами <see cref="CheckFileCount"/>, <see cref="CheckFileCount"/>, <see cref="CheckCreationDate"/>.
        /// </remarks>
        public HashSet<FileInfo> OldFiles { get; private set; }
        
        /// <summary>
        /// Конструктор, ограничивающий создание экземпляра без параметров.
        /// </summary>
        private FileController()
        {
            ClearOldFiles();
        }

        /// <summary>
        /// Конструктор, создающий критерии контроля по умолчанию.
        /// </summary>
        /// <param name="folderName">Контролируемая папка (директория).</param>
        public FileController(string folderName)
            : this()
        {
            Folder = new DirectoryInfo(folderName);
            Files = Folder.GetAllFiles().ToHashSet(new FileInfoComparer());
            MaxFileCount = 1000;
            MaxCapacity = 1024;     // 1 ГБ
            OldestFileCreationDate = DateTime.MinValue;
            
            // Создаем папку, если не существует TODO: проверить, нужно ли?
            // Directory.CreateDirectory(Folder.FullName);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="folderName">Контролируемая папка (директория).</param>
        /// <param name="maxFileCount">Максимальное количество файлов в папке.</param>
        /// <param name="maxCapacity">Максимальная вместимость папки в мегабайтах
        /// (если &lt;= 0, - принимается максимальное значение).</param>
        /// <exception cref="ArgumentException">Неправильное имя контролируемой папки.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Количество файлов должно быть положительным числом.</exception>
        public FileController(string folderName, int maxFileCount, int maxCapacity = int.MaxValue)
            : this(folderName)
        {
            if (folderName.IsNullOrWhiteSpace())
                throw new ArgumentException(nameof(folderName));
            if (maxFileCount < 1)
                throw new ArgumentOutOfRangeException(nameof(maxFileCount));
            if (maxCapacity < 1)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity));
            
            MaxFileCount = maxFileCount;
            MaxCapacity = maxCapacity;
        }

        /// <inheritdoc cref="FileController(string,int,int)"/>
        /// <param name="oldestFileCreationDate">Дата/время создания самого старого файла.</param>
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public FileController(string folderName, int maxFileCount, DateTime oldestFileCreationDate, 
            int maxCapacity = int.MaxValue) : this(folderName, maxFileCount, maxCapacity)
        {
            OldestFileCreationDate = oldestFileCreationDate;
        }

        /// <summary>
        /// Проверить контролируемую папку на устаревшие файлы по различным критериям.
        /// </summary>
        /// <returns>True, если имеются устаревшие файлы.</returns>
        public bool CheckAll()
        {
            var checkC = CheckCapacity();
            var checkCd = CheckCreationDate();
            var checkFc = CheckFileCount();

            return checkC || checkCd || checkFc;
        }
        
        /// <summary>
        /// Проверить контролируемую папку на превышение вместимости,
        /// с помещением излишков (устаревших файлов) в коллекцию <see cref="OldFiles"/>.
        /// </summary>
        /// <returns>True, если вместимость превышена.</returns>
        public bool CheckCapacity()
        {
            var oldFiles = 
                Folder.GetOldFiles(MaxCapacity * MEGABYTE).ToHashSet(new FileInfoComparer());
            OldFiles.UnionWith(oldFiles);
            
            return oldFiles.Any();
        }
        
        /// <summary>
        /// Проверить контролируемую папку на наличие устаревших файлов,
        /// с помещением излишков (устаревших файлов) в коллекцию <see cref="OldFiles"/>.
        /// </summary>
        /// <returns>True, если имеются устаревшие файлы.</returns>
        public bool CheckCreationDate()
        {
            var oldFiles = Folder.GetOldFiles(OldestFileCreationDate).ToHashSet(new FileInfoComparer());
            OldFiles.UnionWith(oldFiles);
            
            return oldFiles.Any();
        }
        
        /// <summary>
        /// Проверить контролируемую папку на превышение количества файлов сверх нормы,
        /// с помещением излишков (старых файлов) в коллекцию <see cref="OldFiles"/>.
        /// </summary>
        /// <returns>True, если количества файлов превысило норму.</returns>
        public bool CheckFileCount()
        {
            var oldFiles = Folder.GetOldFiles(MaxFileCount).ToList();
            OldFiles.UnionWith(oldFiles);

            return oldFiles.Any();
        }

        /// <summary>
        /// Очистить коллекцию устаревших файлов.
        /// </summary>
        public void ClearOldFiles()
        {
            OldFiles = new HashSet<FileInfo>(new FileInfoComparer());
        }

        /// <summary>
        /// Удалить устаревшие файлы с диска.
        /// </summary>
        /// <returns>Результат удаления в виде объекта <see cref="Result{T}"/>.</returns>
        public Result<bool> RemoveOldFiles()
        {
            try
            {
                OldFiles.ForEach(fi => File.Delete(fi.FullName));
                ClearOldFiles();        // очищаем список устаревших файлов
                
                return Result<bool>.Done(true);
            }
            catch (Exception e)
            {
                return Result<bool>.Fail(e);
            }
        }

        /// <summary>
        /// Установить максимальное количество файлов в контролируемой папке.
        /// </summary>
        /// <param name="maxFilesCount">Максимальное количество файлов.</param>
        /// <returns>Текущий объект <see cref="FileController"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Количество файлов должно быть положительным числом.</exception>
        public FileController SetMaxFilesCount(int maxFilesCount)
        {
            if (maxFilesCount < 1)
                throw new ArgumentOutOfRangeException(nameof(maxFilesCount));

            MaxFileCount = maxFilesCount;
            return this;
        }
        
        /// <summary>
        /// Установить максимальную вместимость контролируемой папки.
        /// </summary>
        /// <param name="maxCapacity">Вместимость папки в мегабайтах
        /// (если &lt;= 0, - принимается максимальное значение).</param>
        /// <returns>Текущий объект <see cref="FileController"/>.</returns>
        public FileController SetMaxCapacity(int maxCapacity)
        {
            MaxCapacity = maxCapacity <= 0 ? int.MaxValue : maxCapacity;
            return this;
        }

        /// <summary>
        /// Установить дату/время создания самого старого файла в контролируемой папке.
        /// </summary>
        /// <param name="oldestFileCreationDate">Дата/время создания самого старого файла.</param>
        /// <returns>Текущий объект <see cref="FileController"/>.</returns>
        public FileController SetOldestFileCreationDate(DateTime oldestFileCreationDate)
        {
            OldestFileCreationDate = oldestFileCreationDate;
            return this;
        }
    }
}