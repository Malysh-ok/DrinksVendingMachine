using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Infrastructure.BaseExtensions;

namespace Infrastructure.BaseComponents.Components.Colors
{
    /// <summary>
    /// Преобразует цвета в структуры GDI+ (System.Drawing.Color) и из них.
    /// </summary>
    /// <remarks>
    /// На основе декомпиляции ColorTranslator в Net Framework.
    /// </remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class ColorTranslator 
    {
        #region [---------- Вспомогательное ----------]

        private static readonly Dictionary<string, Color> HtmlSysColorTable = 
            new Dictionary<string, Color>(28, StringComparer.OrdinalIgnoreCase)
            {
                ["ActiveBorder"] = Color.FromKnownColor(KnownColor.ActiveBorder),      
                ["ActiveCaption"] = Color.FromKnownColor(KnownColor.ActiveCaption),    
                ["AppWorkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace),      
                ["Background"] = Color.FromKnownColor(KnownColor.Desktop),
                ["ButtonFace"] = Color.FromKnownColor(KnownColor.ButtonFace),               // было Control
                ["ButtonHighlight"] = Color.FromKnownColor(KnownColor.ButtonHighlight),     // было ControlLightLight
                ["ButtonShadow"] = Color.FromKnownColor(KnownColor.ButtonShadow),           // было ControlDark
                ["ButtonText"] = Color.FromKnownColor(KnownColor.ControlText),
                ["CaptionText"] = Color.FromKnownColor(KnownColor.ActiveCaptionText),
                ["GrayText"] = Color.FromKnownColor(KnownColor.GrayText),
                ["Highlight"] = Color.FromKnownColor(KnownColor.Highlight),
                ["HighlightText"] = Color.FromKnownColor(KnownColor.HighlightText),
                ["InactiveBorder"] = Color.FromKnownColor(KnownColor.InactiveBorder),
                ["InactiveCaption"] = Color.FromKnownColor(KnownColor.InactiveCaption),
                ["InactiveCaptionText"] = Color.FromKnownColor(KnownColor.InactiveCaptionText),
                ["InfoBackground"] = Color.FromKnownColor(KnownColor.Info),
                ["InfoText"] = Color.FromKnownColor(KnownColor.InfoText),
                ["Menu"] = Color.FromKnownColor(KnownColor.Menu),
                ["MenuText"] = Color.FromKnownColor(KnownColor.MenuText),
                ["Scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar),
                ["ThreeDDarkShadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark),
                ["ThreeDFace"] = Color.FromKnownColor(KnownColor.Control),
                ["ThreeDHighlight"] = Color.FromKnownColor(KnownColor.ControlLight),
                ["ThreeDLightShadow"] = Color.FromKnownColor(KnownColor.ControlLightLight),
                ["ThreeDShadow"] = Color.FromKnownColor(KnownColor.ControlDark),            // Malysh S. V.: добавил
                ["Window"] = Color.FromKnownColor(KnownColor.Window),
                ["WindowFrame"] = Color.FromKnownColor(KnownColor.WindowFrame),
                ["WindowText"] = Color.FromKnownColor(KnownColor.WindowText)
            };

        private static readonly uint[] ColorTable = new uint[141]
        {
            16777215U,
            4293982463U,
            4294634455U,
            4278255615U,
            4286578644U,
            4293984255U,
            4294309340U,
            4294960324U,
            4278190080U,
            4294962125U,
            4278190335U,
            4287245282U,
            4289014314U,
            4292786311U,
            4284456608U,
            4286578432U,
            4291979550U,
            4294934352U,
            4284782061U,
            4294965468U,
            4292613180U,
            4278255615U,
            4278190219U,
            4278225803U,
            4290283019U,
            4289309097U,
            4278215680U,
            4290623339U,
            4287299723U,
            4283788079U,
            4294937600U,
            4288230092U,
            4287299584U,
            4293498490U,
            4287609995U,
            4282924427U,
            4281290575U,
            4278243025U,
            4287889619U,
            4294907027U,
            4278239231U,
            4285098345U,
            4280193279U,
            4289864226U,
            4294966000U,
            4280453922U,
            4294902015U,
            4292664540U,
            4294506751U,
            4294956800U,
            4292519200U,
            4286611584U,
            4278222848U,
            4289593135U,
            4293984240U,
            4294928820U,
            4291648604U,
            4283105410U,
            4294967280U,
            4293977740U,
            4293322490U,
            4294963445U,
            4286381056U,
            4294965965U,
            4289583334U,
            4293951616U,
            4292935679U,
            4294638290U,
            4292072403U,
            4287688336U,
            4294948545U,
            4294942842U,
            4280332970U,
            4287090426U,
            4286023833U,
            4289774814U,
            4294967264U,
            4278255360U,
            4281519410U,
            4294635750U,
            4294902015U,
            4286578688U,
            4284927402U,
            4278190285U,
            4290401747U,
            4287852763U,
            4282168177U,
            4286277870U,
            4278254234U,
            4282962380U,
            4291237253U,
            4279834992U,
            4294311930U,
            4294960353U,
            4294960309U,
            4294958765U,
            4278190208U,
            4294833638U,
            4286611456U,
            4285238819U,
            4294944000U,
            4294919424U,
            4292505814U,
            4293847210U,
            4288215960U,
            4289720046U,
            4292571283U,
            4294963157U,
            4294957753U,
            4291659071U,
            4294951115U,
            4292714717U,
            4289781990U,
            4286578816U,
            4294901760U,
            4290547599U,
            4282477025U,
            4287317267U,
            4294606962U,
            4294222944U,
            4281240407U,
            4294964718U,
            4288696877U,
            4290822336U,
            4287090411U,
            4285160141U,
            4285563024U,
            4294966010U,
            4278255487U,
            4282811060U,
            4291998860U,
            4278222976U,
            4292394968U,
            4294927175U,
            4282441936U,
            4293821166U,
            4294303411U,
            uint.MaxValue,
            4294309365U,
            4294967040U,
            4288335154U
        };
        
        private static uint ColorRefToArgb(uint value) =>
            (uint)(((int)value & (int)byte.MaxValue) << 16 |
                   ((int)(value >> 8) & (int)byte.MaxValue) << 8 |
                   (int)(value >> 16) & (int)byte.MaxValue |
                   -16777216);

        private static Color ArgbToKnownColor(uint argb)
        {
            for (var index = 1; index < ColorTable.Length; ++index)
            {
                if ((int)ColorTable[index] == (int)argb)
                    return Color.FromKnownColor((KnownColor)(index + 27));
            }

            return Color.FromArgb((int)argb);
        }

        #endregion
        
        
        /// <summary>
        /// Преобразует представление цвета HTML в структуру Color (GDI+).
        /// </summary>
        public static Color FromHtml(string htmlColor)
        {
            // Сначала находим системные html-цвета
            // (их названия отличаются от KnownColor, поэтому используем словарь)
            if (HtmlSysColorTable.TryGetValue(htmlColor, out var color))
                return color;

            // Если не нашли - ищем при помощи конвертера
            // (либо в формате #AARRGGBB/#RRGGBB, либо по названию в KnownColor)
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color));
            color = converter.IsValid(htmlColor)
                ? (Color)converter.ConvertFromString(htmlColor)!
                : Color.Empty;

            // Если системный цвет, но при этом не был найден в словаре HtmlSysColorTable - возвращаем пустой цвет
            if (color.IsSystemColor)
                color = Color.Empty;

            return color;
        }
        
        /// <summary>
        /// Преобразует значение цвета в формате OLE в структуру Color (GDI+).
        /// </summary>
        /// <param name="oleColor">Цвет в формате OLE, который необходимо преобразовать.</param>
        public static Color FromOle(int oleColor)
        {
            if ((oleColor & int.MinValue) == 0) 
                return ArgbToKnownColor(ColorRefToArgb((uint)oleColor));

            return oleColor switch
            {
                int.MinValue => Color.FromKnownColor(KnownColor.ScrollBar),
                -2147483647 => Color.FromKnownColor(KnownColor.Desktop),
                -2147483646 => Color.FromKnownColor(KnownColor.ActiveCaption),
                -2147483645 => Color.FromKnownColor(KnownColor.InactiveCaption),
                -2147483644 => Color.FromKnownColor(KnownColor.Menu),
                -2147483643 => Color.FromKnownColor(KnownColor.Window),
                -2147483642 => Color.FromKnownColor(KnownColor.WindowFrame),
                -2147483641 => Color.FromKnownColor(KnownColor.MenuText),
                -2147483640 => Color.FromKnownColor(KnownColor.WindowText),
                -2147483639 => Color.FromKnownColor(KnownColor.ActiveCaptionText),
                -2147483638 => Color.FromKnownColor(KnownColor.ActiveBorder),
                -2147483637 => Color.FromKnownColor(KnownColor.InactiveBorder),
                -2147483636 => Color.FromKnownColor(KnownColor.AppWorkspace),
                -2147483635 => Color.FromKnownColor(KnownColor.Highlight),
                -2147483634 => Color.FromKnownColor(KnownColor.HighlightText),
                -2147483633 => Color.FromKnownColor(KnownColor.Control),
                -2147483632 => Color.FromKnownColor(KnownColor.ControlDark),
                -2147483631 => Color.FromKnownColor(KnownColor.GrayText),
                -2147483630 => Color.FromKnownColor(KnownColor.ControlText),
                -2147483629 => Color.FromKnownColor(KnownColor.InactiveCaptionText),
                -2147483628 => Color.FromKnownColor(KnownColor.ControlLightLight),
                -2147483627 => Color.FromKnownColor(KnownColor.ControlDarkDark),
                -2147483626 => Color.FromKnownColor(KnownColor.ControlLight),
                -2147483625 => Color.FromKnownColor(KnownColor.InfoText),
                -2147483624 => Color.FromKnownColor(KnownColor.Info),
                -2147483622 => Color.FromKnownColor(KnownColor.HotTrack),
                -2147483621 => Color.FromKnownColor(KnownColor.GradientActiveCaption),
                -2147483620 => Color.FromKnownColor(KnownColor.GradientInactiveCaption),
                -2147483619 => Color.FromKnownColor(KnownColor.MenuHighlight),
                -2147483618 => Color.FromKnownColor(KnownColor.MenuBar),
                _ => ArgbToKnownColor(ColorRefToArgb((uint)oleColor))
            };
        }
        
        /// <summary>
        /// Преобразует значение цвета Windows в структуру Color (GDI+).
        /// </summary>
        /// <param name="win32Color">Цвет Windows, который необходимо преобразовать.</param>
        public static Color FromWin32(int win32Color) => FromOle(win32Color);
        
        
        /// <summary>
        /// Преобразует указанную структуру Color в строковое представление цвета HTML.
        /// </summary>
        public static string ToHtml(Color color)
        {
            var html = string.Empty;
            
            if (color.IsEmpty)
                return html;
            
            if (color.IsSystemColor)
            {
                html = HtmlSysColorTable.FirstOrDefault(c => c.Value == color).Key;
                if (!html.IsNullOrEmpty())
                    return html;
                
                html = color.ToKnownColor() switch
                {
                    KnownColor.GradientActiveCaption => "ActiveCaption",
                    KnownColor.GradientInactiveCaption => "InactiveCaption",
                    KnownColor.HotTrack => "Highlight",
                    KnownColor.MenuHighlight => "HighlightText",
                    KnownColor.MenuBar => "Menu",
                    _ => html
                };
            }
            else if (color.IsNamedColor)
            {
                html = color.Name;
            }
            else
            {
                var num = color.R;
                var str1 = num.ToString("X2", null);
                num = color.G;
                var str2 = num.ToString("X2", null);
                num = color.B;
                var str3 = num.ToString("X2", null);
                html = "#" + str1 + str2 + str3;
            }

            return html;
        }

        /// <summary>
        /// Преобразует указанную структуру Color в цвет OLE.
        /// </summary>
        /// <param name="color">Структура Color, которую необходимо преобразовать.</param>
        public static int ToOle(Color color)
        {
            if (color.IsKnownColor && color.IsSystemColor)
            {
                switch (color.ToKnownColor())
                {
                    case KnownColor.ActiveBorder:
                        return -2147483638;
                    case KnownColor.ActiveCaption:
                        return -2147483646;
                    case KnownColor.ActiveCaptionText:
                        return -2147483639;
                    case KnownColor.AppWorkspace:
                        return -2147483636;
                    case KnownColor.Control:
                        return -2147483633;
                    case KnownColor.ControlDark:
                        return -2147483632;
                    case KnownColor.ControlDarkDark:
                        return -2147483627;
                    case KnownColor.ControlLight:
                        return -2147483626;
                    case KnownColor.ControlLightLight:
                        return -2147483628;
                    case KnownColor.ControlText:
                        return -2147483630;
                    case KnownColor.Desktop:
                        return -2147483647;
                    case KnownColor.GrayText:
                        return -2147483631;
                    case KnownColor.Highlight:
                        return -2147483635;
                    case KnownColor.HighlightText:
                        return -2147483634;
                    case KnownColor.HotTrack:
                        return -2147483622;
                    case KnownColor.InactiveBorder:
                        return -2147483637;
                    case KnownColor.InactiveCaption:
                        return -2147483645;
                    case KnownColor.InactiveCaptionText:
                        return -2147483629;
                    case KnownColor.Info:
                        return -2147483624;
                    case KnownColor.InfoText:
                        return -2147483625;
                    case KnownColor.Menu:
                        return -2147483644;
                    case KnownColor.MenuText:
                        return -2147483641;
                    case KnownColor.ScrollBar:
                        return int.MinValue;
                    case KnownColor.Window:
                        return -2147483643;
                    case KnownColor.WindowFrame:
                        return -2147483642;
                    case KnownColor.WindowText:
                        return -2147483640;
                    case KnownColor.ButtonFace:
                        return -2147483633;
                    case KnownColor.ButtonHighlight:
                        return -2147483628;
                    case KnownColor.ButtonShadow:
                        return -2147483632;
                    case KnownColor.GradientActiveCaption:
                        return -2147483621;
                    case KnownColor.GradientInactiveCaption:
                        return -2147483620;
                    case KnownColor.MenuBar:
                        return -2147483618;
                    case KnownColor.MenuHighlight:
                        return -2147483619;
                }
            }

            return ToWin32(color);
        }

        /// <summary>
        /// Преобразует указанную структуру Color в цвет Windows.
        /// </summary>
        /// <param name="color">Структура Color, которую необходимо преобразовать.</param>
        public static int ToWin32(Color color) => (int)color.R | (int)color.G << 8 | (int)color.B << 16;
    }
}