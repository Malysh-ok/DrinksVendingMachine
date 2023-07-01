using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Infrastructure.BaseComponents.Helpers;

/// <summary>
/// Методы, помогающие работать с <see cref="CultureInfo"/>.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CultureInfoHelper
{
    /// <summary>
    /// Получить английский языковой стандарт.
    /// </summary>
    public static CultureInfo EnCultureInfo => CultureInfo.GetCultureInfo("en-US");     // может en-EN?
    
    /// <summary>
    /// Получить русский языковой стандарт.
    /// </summary>
    public static CultureInfo RuCultureInfo => CultureInfo.GetCultureInfo("ru-RU");
    
    /// <summary>
    /// Получить список имен всех языковых стандартов.
    /// </summary>
    public static IEnumerable<string> GetAllCultureNames()
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(ci => ci.Name.ToString());
    }
    
    /// <summary>
    /// Получить список имен всех нейтральных языковых стандартов.
    /// </summary>
    /// <remarks>
    /// Языки, чье название состоит из 2 символов (а фактически, иногда и из 3).
    /// </remarks>
    public static IEnumerable<string> GetAllNeutralCultureNames()
    {
        return CultureInfo.GetCultures(CultureTypes.NeutralCultures)
            .Select(ci => ci.TwoLetterISOLanguageName.ToString()).Distinct();
    }
}