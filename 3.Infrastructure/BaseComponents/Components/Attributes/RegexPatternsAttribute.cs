using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Infrastructure.BaseExtensions.Collections;

namespace Infrastructure.BaseComponents.Components.Attributes
{
    /// <summary>
    /// Атрибут шаблонов регулярных выражений. 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RegexPatternsAttribute : Attribute
    {
        /// <inheritdoc />
        public RegexPatternsAttribute(params string [] patterns)
        {
            Patterns = patterns.Select(pattern => new Regex(pattern)).AsList();
        }

        /// <summary>
        /// Регулярные выражения.
        /// </summary>
        public List<Regex> Patterns { get; }

        public const string IPv4 = @"^([0-9]{1,2}|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]{1,2}|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]{1,2}|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]{1,2}|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        public const string IPv6 = @"^([0-9a-f]{1,4}:){7}[0-9a-f]{1,4}$";
        public const string IPv6_Subnet = @"^/[1-9]|/[1-9][0-9]|/1[0-1][0-9]|/12[0-7]$";
        public const string Values0_255 = @"^[0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]$";
        public const string Values1025_65535 = @"^102[5-9]|10[3-9][0-9]|1[1-9][0-9][0-9]|[2-9][0-9]{3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]$";
    }
}
