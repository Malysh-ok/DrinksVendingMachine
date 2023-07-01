using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Infrastructure.BaseComponents.Components.SystemInfo;

namespace Infrastructure.BaseComponents.Components.IO
{
    /// <summary>
    /// Специальные папки Windows.
    /// </summary>
    /// TODO: Реализовать кроссплатформенность ?
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SpecFolder
    {
        #region [---------- Вспомогательное ----------]
        
        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KNOWNFOLDERID.
        /// </summary>
        [DllImport("shell32.dll")]
        internal static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr ppszPath
        );
        
        #endregion

        #region [---------- Получение путей к специальным папкам ОС ----------]

                
        /// <summary>
        /// Получает полный путь к временной директории.
        /// </summary>
        public static string GetTmpPath()
        {
            var tmpPath = Path.GetTempPath();
            return tmpPath;
        }
        
        /// <summary>
        /// Возвращает путь к папке "Загрузки".
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string GetDownloadsPath()
        {
            if (OsPlatformEx.IsWindows)
            {
                const uint KF_FLAG_DONT_VERIFY = 0x00004000;
                const int S_OK = 0;

                var downloadsFolderId = Guid.Parse("{374DE290-123F-4565-9164-39C4925E467B}");

                if (S_OK != SHGetKnownFolderPath(downloadsFolderId, KF_FLAG_DONT_VERIFY, IntPtr.Zero, out var pPath))
                {
                    return null;
                }

                var path = Marshal.PtrToStringUni(pPath);
                Marshal.FreeCoTaskMem(pPath);
                
                return path;
            }

            if (OsPlatformEx.IsUnix)
            {
                // TODO: Возратить путь к Загрузкам в Unix-системе
                return  string.Empty;
            }
            
            return  string.Empty;
        }
        
        #endregion    
    }
}