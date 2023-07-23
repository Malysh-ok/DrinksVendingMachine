using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using Infrastructure.BaseExtensions.Collections;
using Infrastructure.Phrases;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы-расширения для <see cref="string" />.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class StringExtensions
    {
        /// <summary>
        /// Добавление символов до указанной длины строки.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <param name="appendSymbol">Добавляемые символы.</param>
        /// <param name="length">Конечная длина строки.</param>
        public static string Append(this string value, int length, char appendSymbol = ' ')
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var sourceLength = value.Length;

            if (length < sourceLength)
                return value[..length];

            var appendString = new string(appendSymbol, length - sourceLength);

            return $"{value}{appendString}";
        }

        /// <summary>
        /// Укорачивание строки до заданной длины с учетом delimiter.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="maxLength">Максимальная длина строки.</param>
        /// <param name="delimiter">Разделитель, до которого укорачивается строка (если их несколько, - то до последнего;
        /// например DirectorySeparatorChar.</param>
        /// <returns>Укороченная строка.</returns>
        public static string EllipsisString(this string text, int maxLength, char delimiter)
        {
            if (text.Length <= maxLength)
                return text;
            var parts = text.Split(delimiter).ToList();
            if (parts.Count > 1)
            {
                var builder = new StringBuilder();
                var lastPart = parts.Last();
                var max = maxLength - lastPart.Length - 3;
                for (var i = 0; i < parts.Count - 1; i++)
                {
                    if (builder.Length < max)
                    {
                        builder.AppendFormat($"{parts[i]}{delimiter}");
                    }
                }

                builder.AppendFormat($"...{delimiter}{lastPart}");
                return builder.ToString();
            }

            return text.Truncate(maxLength);
        }

        /// <summary>
        /// Замена пустой строки на null.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static string EmptyToNull(this string text)
        {
            return text != null && text.IsEmpty() ? null : text;
        }

        /// <summary>
        /// Получение форматированной строки, путем замены элементов формата в строке format
        /// соответствующими элементами массива args. 
        /// </summary>
        public static string Format(this string format, params object[] args)
        {
            if (format == null || args == null)
                throw new ArgumentNullException(format == null ? nameof(format) : nameof(args));

            return string.Format(format, args);
        }

        /// <summary>
        /// Получение форматированной строки, путем замены элементов формата в строке format
        /// соответствующими элементами массива args. 
        /// </summary>
        /// <remarks>
        /// 1) В отформатированной строке удаляются все пустые подстроки (т.е. идущие подряд кавычки).
        /// <para>2) Если параметр isTrim == true, то в отформатированной строке удаляются незначащие пробелы.</para>
        /// </remarks>
        public static string FormatWithQuotes(this string format, bool isTrim = true, params object[] args)
        {
            if (format == null || args == null)
                throw new ArgumentNullException(format == null ? nameof(format) : nameof(args));

            var builder = new StringBuilder(format.Length + args.Length * 8);
            builder.AppendFormat(format, args);
            builder.Replace(" \"\"", string.Empty);
            builder.Replace("\"\" ", string.Empty);
            builder.Replace("\"\"", string.Empty);
            builder.Replace(" «»", string.Empty);
            builder.Replace("«» ", string.Empty);
            builder.Replace("«»", string.Empty);

            return isTrim
                ? builder.ToString().Trim()
                : builder.ToString();
        }

        /// <summary>
        /// Выполняет делегат <see cref="Func{T, TResult}"/> над исходной строкой.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="func">Делегат.</param>
        /// <returns>Результирующая строка.</returns>
        public static string Func(this string text, Func<string, string> func = null)
        {
            return (func == null)
                ? text
                : func(text);
        }

        /// <summary>
        /// Выполняет делегат <see cref="Func{T, TResult}"/> над исходной строкой,
        /// при условии установленного признака <paramref name="isExecute"/>.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="isExecute">Признак необходимости выполнения делегата.</param>
        /// <param name="func">Делегат.</param>
        /// <returns>Результирующая строка.</returns>
        public static string Func(this string text, bool isExecute, Func<string, string> func = null)
        {
            return (func == null || !isExecute)
                ? text
                : func(text);
        }

        /// <summary>
        /// Ищет <paramref name="startText"/> в исходной строке <paramref name="text"/>
        /// и если поиск удачен - возвращает из исходной строки все, что после <paramref name="startText"/>;
        /// иначе - возвращает null.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="startText">Искомая строка.</param>
        /// <returns>Остаток исходной строки.</returns>
        public static string GetEnd(this string text, string startText)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (startText == null)
                throw new ArgumentNullException(nameof(startText));

            return text.ToPairedValue(startText).Value;
        }
        
        /// <summary>
        /// Получить количество найденных в исходной строке повторений строки <paramref name="searchString"/>.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="searchString">Искомая строка.</param>
        public static int GetSubstringCount(this string text, string searchString)
        {
            if (text.IsNullOrEmpty() || searchString.IsNullOrEmpty())
                return 0;

            var index = 0;
            var subStrCount = 0;
            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            do
            {
                index = compareInfo.IndexOf(text, searchString, index, CompareOptions.Ordinal);
                if (index == -1)
                    break;

                subStrCount++;
                index += searchString.Length;
            } while (index < text.Length);

            return subStrCount;
        }

        /// <summary>
        /// Поиск совпадений в строках с учетом StringComparison.
        /// </summary>
        /// <param name="source">Исходная строка.</param>
        /// <param name="toCheck">Сравниваемая строка.</param>
        /// <param name="comp">Способ (компаратор) сравнения.</param>
        public static bool IsContains(this string source, string toCheck,
            StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            if (source.IsNullOrEmpty())
                return toCheck.IsNullOrEmpty();

            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Проверка строки на пустоту.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsEmpty(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.Length == 0;
        }

        /// <summary>
        /// Проверка строк с учетом StringComparison.
        /// </summary>
        /// <param name="source">Исходная строка.</param>
        /// <param name="other">Сравниваемая строка.</param>
        /// <param name="comp">Способ (компаратор) сравнения.</param>
        public static bool IsEquals(this string source, string other,
            StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            if (ReferenceEquals(source, null))
                return ReferenceEquals(other, null);

            if (ReferenceEquals(other, null))
                return false;

            return source.Equals(other, comp);
        }

        /// <summary>
        /// Проверка строки на null.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsNull(this string text)
        {
            return text is null;
        }

        /// <summary>
        /// Проверка строки на null и пустоту.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Признак того, что строка содержит только пробельные (white space) символы.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsWhiteSpace(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.All(char.IsWhiteSpace);
        }

        /// <summary>
        /// Признак того, что строка есть null, или пустая, или содержит только пробельные (white space) символы.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// Вычисление размера (ширины) текста в px (или в pt, если установлен флаг isResultInPt).
        /// <para> ПРИМЕЧАНИЕ: Если использовать полученный результат для Excel XML - то будет вычислено не очень точно! </para>
        /// </summary>
        /// <param name="text"> Исходная строка. </param>
        /// <param name="font"> Шрифт исходного текста. </param>
        /// <param name="isResultInPt"> Флаг, если true - результат в px, если false - результат в pt. </param>
        /// <remarks>
        /// Поддерживается только Windows.
        /// </remarks>
        public static double MeasureText(this string text, Font font, bool isResultInPt = false)
        {
            double PxToPtX(int px, float dpiX)
            {
                return px * 72.0 / dpiX;
            }

            /* Все это, включая graphics.MeasureString, таботает не точно,
             * если использовать полученный результат для Excell XML !!! */

            using var g = Graphics.FromHwnd(IntPtr.Zero);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel; // .AntiAlias;
            var sf = new StringFormat(); // StringFormat.GenericTypographic
            var sizeF = g.MeasureString(text, font, int.MaxValue, sf);
            double measure = sizeF.Width;

            if (isResultInPt)
                // Px -> pt
                measure = PxToPtX((int)measure, g.DpiX);

            return measure;
        }
        
        /// <summary>
        /// Замена разделителя '.' или ',' в текстовом представлении числа на
        /// значение разделителя в текущей локализации ОС.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <param name="culture">Языковая культура.</param>
        public static string NormalizeForDouble(this string value, CultureInfo culture = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            culture ??= CultureInfo.CurrentCulture;

            return value.Replace(".", culture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", culture.NumberFormat.CurrencyGroupSeparator == ","
                    ? string.Empty
                    : culture.NumberFormat.NumberDecimalSeparator);
        }

        /// <summary>
        /// Замена null строки на пустую.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static string NullToEmpty(this string text)
        {
            return text ?? string.Empty;
        }

        /// <summary>
        /// Проверка строки на содержание только латинских символов.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool OnlyLatinChars(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
        }
        
        /// <summary>
        /// Преобразование строки в логическое значение. 
        /// </summary>
        /// <remarks>
        /// "true" => true, "True" => true, "false" => false и т.п.
        /// </remarks>
        /// <param name="stringValue">Исходная строка.</param>
        /// <param name="defaultValue">Значение результата в случае ошибки преобразования.</param>
        public static bool ParseToBool(this string stringValue, bool defaultValue = false)
        {
            return bool.TryParse(stringValue, out var value)
                ? value
                : defaultValue;
        }

        /// <summary>
        /// Преобразование строки в <see cref="double"/>, с заменой разделителя '.' или ','
        /// на значение разделителя в текущей локализации ОС.
        /// </summary>
        /// <param name="stringValue">Исходная строка.</param>
        /// <param name="defaultValue">Значение результата в случае ошибки преобразования.</param>
        public static double ParseToDouble(this string stringValue, double defaultValue = double.NaN)
        {
            if (stringValue == null)
                throw new ArgumentNullException(nameof(stringValue));

            CommonPhrases.Culture = CultureInfo.CurrentUICulture; // устанавливаем яз. стандарт для фраз

            var normalized = stringValue.NormalizeForDouble();
            if (!string.IsNullOrEmpty(normalized))
            {
                if (double.TryParse(normalized, out var result))
                {
                    return result;
                }
            }

            if (defaultValue.Equals(double.NaN))
            {
                throw new ArgumentException(CommonPhrases.Exception_ParamIsNotConvertToDouble.Format(stringValue));
            }

            return defaultValue;
        }

        /// <summary>
        /// Преобразование строки в число. 
        /// </summary>
        /// <param name="stringValue">Исходная строка.</param>
        /// <param name="defaultValue">Значение результата в случае ошибки преобразования.</param>
        public static int ParseToInt(this string stringValue, int defaultValue = 0)
        {
            return int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        /// <summary>
        /// Удаление из строки всех вхождений указанных символов.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="charsToRemove">Список символов для удаления.</param>
        /// <param name="charComparer">Компаратор символов в строке (по умолчанию чувствительный к регистру).</param>
        /// <returns>Строка, не содержащая указанных символов.</returns>
        public static string RemoveChars(this string text, IEnumerable<char> charsToRemove,
            IEqualityComparer<char> charComparer = null)
        {
            const int THRESHOLD = 6;

            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (charsToRemove == null)
                throw new ArgumentNullException(nameof(charsToRemove));

            var exceptArray = charsToRemove as char[] ?? charsToRemove.ToArray();

            var result = new char[text.Length];
            var wi = 0;
            if (exceptArray.Length > THRESHOLD)
            {
                var exceptSet = charComparer == null
                    ? new HashSet<char>(exceptArray)
                    : new HashSet<char>(exceptArray, charComparer);

                foreach (var c in text)
                    if (!exceptSet.Contains(c))
                        result[wi++] = c;
            }
            else
            {
                if (charComparer == null)
                {
                    charComparer = EqualityComparer<char>.Default;
                }

                foreach (var c in text)
                {
                    var keepSymbol = true;
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var exceptSymbol in exceptArray)
                        if (charComparer.Equals(c, exceptSymbol))
                        {
                            keepSymbol = false;
                            break;
                        }

                    if (keepSymbol)
                        result[wi++] = c;
                }
            }

            return new string(result, 0, wi);
        }

        /// <summary>
        /// Удаление из строки всех вхождений указанных символов.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="charsToRemove">Список символов для удаления.</param>
        /// <returns>Строка, не содержащая указанных символов.</returns>
        public static string RemoveChars(this string text, params char[] charsToRemove)
        {
            return RemoveChars(text, (IEnumerable<char>)charsToRemove);
        }
        
        /// <summary>
        /// Удаление из строки цифрового окончания.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        public static string RemoveNumericPostfix(this string value)
        {
            var builder = new StringBuilder(value);
            for (var i = builder.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(builder[i]))
                    break;

                builder.Remove(i, 1);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Удаление символов перевода строки из конца текста, если они есть.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="isAllDelete">Признак того, что нужно удалить все символы.
        /// Если равен false - удаляется только последний символ.</param>
        /// <returns>Измененный текст.</returns>
        public static string RemoveTerminatingNewlineChars(this string text, bool isAllDelete = true)
        {
            return new StringBuilder(text).RemoveTerminatingNewlineChars(isAllDelete).ToString();
        }

        /// <summary>
        /// Удаление пробелов из строки.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        public static string RemoveWhiteSpace(this string text)
        {
            var sb = new StringBuilder();
            if (text == null)
                return sb.ToString();

            foreach (var c in text.Where(c => !char.IsWhiteSpace(c)))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Получение массива байт из строки.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="encoding">Кодировка исходной строки.</param>
        public static byte[] ToByteArray(this string str, Encoding encoding = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            encoding ??= Encoding.UTF8;
            
            return encoding.GetBytes(str);
        }
        
        /// <summary>
        /// Разделяет строку на пару "ключ - значение", используя разделитель delimiter (по умолчанию - ":").
        /// </summary>
        public static KeyValuePair<string, string> ToPairedValue(this string str, char delimiter = ':')
        {
            return str.ToPairedValue(delimiter.ToString());
        }
        
        /// <inheritdoc cref="ToPairedValue(string, char)"/>
        public static KeyValuePair<string, string> ToPairedValue(this string str, string delimiter = ":")
        {
            var splitedArray = str.Split(delimiter);
            return splitedArray.Length > 1
                ? new KeyValuePair<string, string>(splitedArray[0], splitedArray[1])
                : new KeyValuePair<string, string>(null, null);
        }

        /// <summary>
        /// Получение массива ushort из строки.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public static ushort[] ToUshortArray(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var bytes = str.ToByteArray().AddEmptyToEven();
            var uhorts = new List<ushort>();
            for (var i = 0; i < bytes.Count; i += 2)
            {
                uhorts.Add((ushort)((bytes[i + 1] << 8) | bytes[i]));
            }

            return uhorts.ToArray();
        }
        
        /// <summary>
        /// Если длина строки больше <paramref name="maxLength"/> символов, метод
        /// возвращает только первые <paramref name="maxLength"/> символов строки, иначе - исходную строку.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <param name="maxLength">Максимальная допустимая длина строки.</param>
        /// <returns>Усечённая в случае превышения максимальной длины строка.</returns>
        public static string Truncate(this string text, int maxLength)
        {
            CommonPhrases.Culture = CultureInfo.CurrentUICulture; // устанавливаем яз. стандарт для фраз

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    CommonPhrases.Exception_ParamIsNegative);

            return text.IsEmpty() ? text : text.Substring(0, Math.Min(text.Length, maxLength));
        }
    }
}