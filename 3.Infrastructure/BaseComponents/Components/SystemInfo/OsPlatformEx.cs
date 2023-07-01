using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Infrastructure.BaseComponents.Components.SystemInfo;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class OsPlatformEx
{
    /// <summary> 
    /// Получить текущую платформу ОС. 
    /// </summary> 
    public static OSPlatform Current
    {
        get
        {
            var osPlatform = OSPlatform.Create("Other Platform");
            // Check if it's Windows 
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            osPlatform = isWindows ? OSPlatform.Windows : osPlatform;
            // Check if it's OSx 
            var isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            osPlatform = isOsx ? OSPlatform.OSX : osPlatform;
            // Check if it's Linux 
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            osPlatform = isLinux ? OSPlatform.Linux : osPlatform;

            return osPlatform;
        }
    }
        
    /// <summary>
    /// Проверяет, является ли текущая платформа Unix совместимой.
    /// </summary>
    public static bool IsUnix
    {
        get
        {
            var osPlatform = Current;
            return osPlatform == OSPlatform.Linux || osPlatform == OSPlatform.OSX;
        }
    }
        
    /// <summary>
    /// Проверяет, является ли текущая платформа Windows.
    /// </summary>
    public static bool IsWindows => Current == OSPlatform.Windows;
}