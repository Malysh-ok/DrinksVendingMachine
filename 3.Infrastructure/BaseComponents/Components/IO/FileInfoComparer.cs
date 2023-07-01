using System.Collections.Generic;
using System.IO;

namespace Infrastructure.BaseComponents.Components.IO
{
    /// <summary>
    /// Компаратор для сравнения объектов FileInfo в соответствии с физическим смыслом
    /// (т.е. по полному имени файла).
    /// </summary>
    public class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        /// <inheritdoc />
        public bool Equals(FileInfo fi1, FileInfo fi2)
        {
            if (fi1 == null && fi2 == null)
                return true;
            if (fi1 == null | fi2 == null)
                return false;
            return string.Equals(fi1.FullName, fi2.FullName);
        }
        
        /// <inheritdoc />
        public int GetHashCode(FileInfo fi)
        {
            return fi.FullName.GetHashCode();
        }
    }
}