using System;
using System.Diagnostics.CodeAnalysis;
using Infrastructure.BaseExtensions;
using DrawingColor = System.Drawing.Color;

namespace Infrastructure.BaseComponents.Components.Colors
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ColorExtensions
    {
        /// <summary>
        /// Получение цвета из строки.
        /// </summary>
        /// <remarks>
        /// В качестве строки colorStr может выступать как название цвета,
        /// так и html-код цвета (в формате #AARRGGBB или #RRGGBB).<br/>
        /// Если признак isCheckOnlyKnownColors = true,
        /// то colorStr должна совпадать только с известным (предопределенным) цветом.
        /// </remarks>
        /// <returns>
        /// Экземпляр типа Color.<br/>
        /// В случае, если цвет с данным именем отсутствует, или имя цвета = "", или неправильный формат html-кода цвета,
        /// возвращается цвет colorIfError.
        /// </returns>
        /// <param name="colorStr">Строка, идентифицирующая цвет.</param>
        /// <param name="colorIfError">Возвращаемый цвет при ошибке.</param>
        /// <param name="isCheckOnlyKnownColors">Признак проверки только на известные цвета.</param>
        public static DrawingColor StrToDrawingColor(this string colorStr, 
            DrawingColor colorIfError, bool isCheckOnlyKnownColors = true)
        {
            var color = DrawingColor.FromName(colorStr);

            if (color.IsKnownColor)
                return color;

            // Выходим, если установлен признак проверки только известных цветов, или название цвета - пустая строка
            if (isCheckOnlyKnownColors || colorStr.IsEmpty())
                return colorIfError;
            
            // Пытаемся получить цвет по html-коду
            try
            {
                return ColorTranslator.FromHtml(colorStr);
            }
            catch
            {
                return colorIfError;
            }
        }
        
        /// <summary>
        /// Получение цвета из строки.
        /// </summary>
        /// <remarks>
        /// В качестве строки colorStr может выступать как название цвета,
        /// так и html-код цвета (в формате #AARRGGBB или #RRGGBB).<br/>
        /// Если признак isCheckOnlyKnownColors = true,
        /// то colorStr должна совпадать только с известным (предопределенным) цветом.
        /// </remarks>
        /// <returns>
        /// Экземпляр типа Color.<br/>
        /// В случае, если цвет с данным именем отсутствует, или имя цвета = "", или неправильный формат html-кода цвета,
        /// возвращается "пустой" цвет.
        /// </returns>
        /// <param name="colorStr">Строка, идентифицирующая цвет.</param>
        /// <param name="isCheckOnlyKnownColors">Признак проверки только на известные цвета.</param>
        public static DrawingColor StrToDrawingColor(this string colorStr, bool isCheckOnlyKnownColors = true)
        {
            return StrToDrawingColor(colorStr, DrawingColor.Empty, isCheckOnlyKnownColors);
        }
        
        /// <summary>
        /// Преобразуем стандартный цвет (DrawingColor) в HSB-цвет.
        /// </summary>
        /// Источник: https://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part-1
        public static HsbColor DrawingColorToHsbColor(this DrawingColor color)
        {
            // Normalize red, green and blue color components
            var r = (color.R/255.0f);
            var g = (color.G/255.0f);
            var b = (color.B/255.0f);

            // Conversion start
            var max = Math.Max(r, Math.Max(g, b));      // same as brightness
            var min = Math.Min(r, Math.Min(g, b));

            // Hue
            var h = 0.0f;
            
            if (Math.Abs(max - r) < HsbColor.TOLERANCE && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (Math.Abs(max - r) < HsbColor.TOLERANCE && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (Math.Abs(max - g) < HsbColor.TOLERANCE)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (Math.Abs(max - b) < HsbColor.TOLERANCE)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            // Saturation
            var s = (max == 0) ? 0.0f : (1.0f - (min / max));

            return HsbColor.FromAhsb(color.A, h, s, max);
        }

        /// <summary>
        /// Преобразуем HSB-цвет в стандартный цвет (DrawingColor).
        /// </summary>
        /// Источник: https://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part-1
        public static DrawingColor HsbColorToDrawingColor(this HsbColor hsbColor)
        {
            int r = 0, g = 0, b = 0;
            if (hsbColor.Saturation == 0)
            {
                r = g = b = (int)(hsbColor.Brightness * 255f + 0.5f);
            }
            else
            {
                // the color wheel consists of 6 sectors. Figure out which sector
                // you're in.
                var sectorPos = hsbColor.Hue / 60.0f;
                var sectorNumber = (int)(Math.Floor(sectorPos));
                // get the fractional part of the sector
                var fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the color.
                var p = hsbColor.Brightness * (1.0f - hsbColor.Saturation);
                var q = hsbColor.Brightness * (1.0f - (hsbColor.Saturation * fractionalSector));
                var t = hsbColor.Brightness * (1.0f - (hsbColor.Saturation * (1 - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector
                // the angle is in.
                switch(sectorNumber)
                {
                    case 0:
                        r = (int)(hsbColor.Brightness * 255f + 0.5f);
                        g = (int)(t * 255f + 0.5f);
                        b = (int)(p * 255f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255f + 0.5f);
                        g = (int)(hsbColor.Brightness * 255f + 0.5f);
                        b = (int)(p * 255f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255f + 0.5f);
                        g = (int)(hsbColor.Brightness * 255f + 0.5f);
                        b = (int)(t * 255f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255f + 0.5f);
                        g = (int)(q * 255f + 0.5f);
                        b = (int)(hsbColor.Brightness * 255f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255f + 0.5f);
                        g = (int)(p * 255f + 0.5f);
                        b = (int)(hsbColor.Brightness * 255f + 0.5f);
                        break;
                    case 5:
                        r = (int)(hsbColor.Brightness * 255f + 0.5f);
                        g = (int)(p * 255f + 0.5f);
                        b = (int)(q * 255f + 0.5f);
                        break;
                }
            }
            return DrawingColor.FromArgb(hsbColor.Alpha, 
                Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        /// <summary>
        /// Коррекция HSB-цвета.
        /// </summary>
        public static HsbColor AdjustHsbColor(this HsbColor hsbColor, 
            int deltaAlpha, float deltaHue, float deltaSaturation, float deltaBrightness)
        {
            var correctedHsbColor = hsbColor;
            
            correctedHsbColor.Alpha += deltaAlpha;
            correctedHsbColor.Hue += deltaHue;
            correctedHsbColor.Saturation += deltaSaturation;
            correctedHsbColor.Brightness += deltaBrightness;
            
            return correctedHsbColor;
        }
    }
}