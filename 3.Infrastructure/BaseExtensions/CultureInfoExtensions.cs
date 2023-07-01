using System.Globalization;

namespace Infrastructure.BaseExtensions;

public static class CultureInfoExtensions
{
    /// <summary>
    /// Получить наименование языка языкового стандарта.
    /// </summary>
    public static string LangName(this CultureInfo culture) => culture.TwoLetterISOLanguageName;
    
    /// <summary>
    /// Получить признак того, что язык языкового стандарта <paramref name="culture"/> является русским.
    /// </summary>
    public static bool IsRusLang(this CultureInfo culture) => culture.TwoLetterISOLanguageName == "ru";
    
    /// <summary>
    /// Получить признак того, что языковой стандарт с заданным именем существует.
    /// </summary>
    public static bool IsExistsCultureName(this string cultureName)
    {
        try
        {
            _ = CultureInfo.GetCultureInfo(cultureName);

            return true;
        }
        catch
        {
            return false;
        }
    }
}