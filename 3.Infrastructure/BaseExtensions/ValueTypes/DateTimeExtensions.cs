using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Infrastructure.BaseExtensions.ValueTypes
{
    /// <summary>
    /// Методы расширения для <see cref="DateTime" />.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Начало отсчёта времени, которое используется приложениями SCADA-системы.
        /// </summary>
        /// <remarks>Совпадает с началом отсчёта времени в OLE Automation и Delphi.</remarks>
        public static DateTime ScadaEpoch =>
            new DateTime(1899, 12, 30, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Минимально возможное дата/время с точки зрения UNIX.
        /// </summary>
        /// <remarks>Используется в Javascript при реализации даты.</remarks>
        public static DateTime UnixEpoch =>
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Закодировать дату и время в вещественное значение времени.
        /// </summary>
        /// <remarks> Совместим с методом <see cref="DateTime.ToOADate()">DateTime.ToOADate()</see>. </remarks>
        public static double EncodeDateTime(this DateTime dateTime)
            => (dateTime - ScadaEpoch).TotalDays;

        /// <summary>
        /// Декодировать вещественное значение времени, преобразовав его в формат DateTime.
        /// </summary>
        /// <remarks>Совместим с методом <see cref="DateTime.FromOADate(double)">DateTime.FromOADate()</see>.</remarks>
        public static DateTime DecodeDateTime(this double dateTime)
            => ScadaEpoch.AddDays(dateTime);

        /// <summary>
        /// Время в формате DateTime -> формат Unix (long).
        /// </summary>
        public static long ToUnixTimestamp(this DateTime dateTime)
            => (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;

        /// <summary>
        /// Время в формате Unix (long) -> формат DateTime.
        /// </summary>
        public static DateTime FromUnixTimestamp(this long timestamp)
            => UnixEpoch.AddMilliseconds(timestamp).ToLocalTime();
        
        /// <summary>
        /// Комбинировать заданные дату и время в единое значение.
        /// </summary>
        public static DateTime CombineDateTime(this DateTime date, double time)
            => date.AddDays(time - Math.Truncate(time));

        /// <summary>
        /// Проверка вхождения Даты/Времени dt в диапазон от begin до end.
        /// </summary>
        /// <remarks> Если параметр inclusive = true, - сравнение с конечными значениями включительно. </remarks>
        public static bool IsBetween(this DateTime dt, DateTime begin, DateTime end, bool inclusive = false)
            => inclusive
                ? begin <= dt && dt <= end
                : begin < dt && dt < end;

        /// <summary>
        /// Получить из даты/времени название файла без расширения.
        /// </summary>
        /// <param name="dt">Исходные дата/время.</param>
        public static string DateToFileName(this DateTime dt)
            => $"{dt:yyyy.MM.dd}";
        
        /// <summary>
        /// Получить из даты/времени название файла.
        /// </summary>
        /// <param name="dt">Исходные дата/время.</param>
        /// <param name="extension">Расширение файла.</param>
        public static string DateToFileName(this DateTime dt, string extension)
            => $"{DateToFileName(dt)}.{extension}";
        
        /// <summary>
        /// Получить год в строковом представлении.
        /// </summary>
        public static string GetYear(this DateTime dateTime, bool isFourDigit = false)
        {
            return isFourDigit
                ? dateTime.ToString("yyyy", CultureInfo.CurrentCulture)
                : dateTime.Year.ToString();
        }
        
        /// <summary>
        /// Получить месяц в строковом представлении.
        /// </summary>
        public static string GetMonth(this DateTime dateTime, bool isTwoDigit = false)
        {
            return isTwoDigit
                ? dateTime.ToString("MM", CultureInfo.CurrentCulture)
                : dateTime.Month.ToString();
        }

        /// <summary>
        /// Получить день в строковом представлении.
        /// </summary>
        public static string GetDay(this DateTime dateTime, bool isTwoDigit = false)
        {
            return isTwoDigit
                ? dateTime.ToString("dd", CultureInfo.CurrentCulture)
                : dateTime.Day.ToString();
        }

        /// <summary>
        /// Получить дату/время из обнуляемого типа.
        /// </summary>
        public static DateTime FromNullable(this DateTime? dt, DateTime? defaultValue = null)
        {
            return dt ?? (defaultValue ?? DateTime.MinValue);
        }

        #region [---------- С использованием CultureInfo ----------]
        
        /// <summary>
        /// Определить, является ли заданная строка записью даты, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        /// <remarks> Строка не должна содержать компонента времени. </remarks>
        public static bool IsDate(this string s, CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;
            
            return DateTime.TryParse(s, culture, DateTimeStyles.None, out var dateTime) &&
                dateTime.TimeOfDay.TotalMilliseconds == 0;
        }

        /// <summary>
        /// Преобразовать строку в дату, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        /// <remarks> Строка не должна содержать компонента времени. </remarks>
        public static DateTime ToDate(this string s, CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;
            
            if (DateTime.TryParse(s, culture, DateTimeStyles.None, out var dateTime) && 
                    dateTime.TimeOfDay.TotalMilliseconds == 0)
                return dateTime.Date;
                
            return DateTime.MinValue;
        }
        
        /// <summary>
        /// Преобразовать строку во время, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        /// <remarks> Компонент даты игнорируется. </remarks>
        public static DateTime ToTime(this string s, CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;

            return DateTime.TryParse(s, culture, DateTimeStyles.None, out var dateTime)
                ? DateTime.MinValue.AddMilliseconds(dateTime.TimeOfDay.TotalMilliseconds) 
                : DateTime.MinValue;
        }

        /// <summary>
        /// Попытаться преобразовать строку в дату и время, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        public static bool TryParseDateTime(this string s, out DateTime result, CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;
            
            return DateTime.TryParse(s, culture, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Преобразовать строку в дату/время, используя
        /// <see cref="CultureInfo">CultureInfo</see> и массив форматов formats.
        /// </summary>
        /// <returns> True - если преобразование успешно, иначе - false. </returns>
        public static bool TryParseDateTimeWithFormats(this string s, out DateTime result, 
            CultureInfo culture = null, string[] formats = null)
        {
            culture ??= CultureInfo.CurrentCulture;
            formats ??= new[] { "dd'.'MM'.'yyyy", "dd'/'MM'/'yyyy", "yyyyMMddHHmmss'.'fff" };
            
            return DateTime.TryParseExact(s, formats, culture, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Преобразовать дату в строку, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        public static string StrFromDate(this DateTime dt, CultureInfo culture = null)
        {
            culture ??= CultureInfo.CurrentCulture;
            var dateFormat = culture.DateTimeFormat.ShortDatePattern;

            // Приводим формат годов к четырехзначному
            var indB = dateFormat.IndexOf('y');
            var indE = dateFormat.LastIndexOf('y');
            var count = indE - indB + 1;
            if (indB > -1 && count != 4)
            {
                dateFormat = dateFormat.Remove(indB, count);
                dateFormat = dateFormat.Insert(indB, "yyyy");
            }
            
            // Приводим формат месяцев к двузначному
            indB = dateFormat.IndexOf('M');
            indE = dateFormat.LastIndexOf('M');
            count = indE - indB + 1;
            if (indB > -1 && count != 2)
            {
                dateFormat = dateFormat.Remove(indB, count);
                dateFormat = dateFormat.Insert(indB, "MM");
            }

            // Приводим формат дней к двузначному
            indB = dateFormat.IndexOf('d');
            indE = dateFormat.LastIndexOf('d');
            count = indE - indB + 1;
            if (indB > -1 && count != 2)
            {
                dateFormat = dateFormat.Remove(indB, count);
                dateFormat = dateFormat.Insert(indB, "dd");
            }

            return dt.ToString(dateFormat, culture);         
        }
        
        /// <summary>
        /// Преобразовать время в строку, используя
        /// <see cref="CultureInfo">CultureInfo</see> и признак использования мс.
        /// </summary>
        public static string StrFromTime(this DateTime dateTime, CultureInfo culture = null, bool isIncludingMs = false)
        {
            culture ??= CultureInfo.CurrentCulture;
            var timeFormat = culture.DateTimeFormat.LongTimePattern;

            // Приводим 12-часовой формат к двузначному
            var indB = timeFormat.IndexOf('h');
            var indE = timeFormat.LastIndexOf('h');
            var count = indE - indB + 1;
            if (indB > -1 && count != 2)
            {
                timeFormat = timeFormat.Remove(indB, count);
                timeFormat = timeFormat.Insert(indB, "hh");
            }
            // TODO: Продумать, что делать с описателем "tt" (AM/PM)
            
            // Приводим 24-часовой формат к двузначному (взаимоисключающее с предыдущим)
            indB = timeFormat.IndexOf('H');
            indE = timeFormat.LastIndexOf('H');
            count = indE - indB + 1;
            if (indB > -1 && count != 2)
            {
                timeFormat = timeFormat.Remove(indB, count);
                timeFormat = timeFormat.Insert(indB, "HH");
            }
            
            // Приводим формат минут к двузначному
            indB = timeFormat.IndexOf('m');
            indE = timeFormat.LastIndexOf('m');
            count = indE - indB + 1;
            if (indB > -1 && count != 2)
            {
                timeFormat = timeFormat.Remove(indB, count);
                timeFormat = timeFormat.Insert(indB, "mm");
            }
            
            // Приводим формат секунд к двузначному
            indB = timeFormat.IndexOf('s');
            indE = timeFormat.LastIndexOf('s');
            count = indE - indB + 1;
            if (indB > -1 && (count != 2 || isIncludingMs))
            {
                timeFormat = timeFormat.Remove(indB, count);
                timeFormat = timeFormat.Insert(indB, isIncludingMs ? "ss.fff" : "ss");
            }            

            return dateTime.ToString(timeFormat, culture); 
        }

        /// <summary>
        /// Преобразовать время в строку.
        /// </summary>
        /// <param name="dateTime">Преобразуемая дата/время.</param>
        /// <param name="culture">Используемый <see cref="CultureInfo">языковой стандарт</see>.</param>
        /// <param name="isIncludingMs">Признак использования мс.</param>
        /// <param name="resultStrPattern">Паттерн результирующей строки,
        /// где {0} - это дата, {1} - это время.</param>
        public static string StrFromDateTime(this DateTime dateTime, CultureInfo culture = null, 
            bool isIncludingMs = false, string resultStrPattern = "{0} {1}")
        {
            var dateStr = dateTime.StrFromDate(culture);
            var timeStr = dateTime.StrFromTime(culture, isIncludingMs);
            
            return string.Format(resultStrPattern, dateStr, timeStr);
        }

        /// <summary>
        /// Получить год в строковом представлении, используя
        /// <see cref="CultureInfo">CultureInfo</see>.
        /// </summary>
        public static string GetYear(this DateTime dateTime, CultureInfo culture = null)
        {
            return dateTime.ToString("yyyy", culture);
        }

        #endregion
    }
}
