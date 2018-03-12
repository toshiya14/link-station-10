using System;
using GDI = System.Drawing;
using D2D = System.Windows.Media;
using System.Text.RegularExpressions;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public static class ColorStringHelper
    {
        /// <summary>
        /// 用于表示颜色的透明度、红色、绿色、蓝色分量的结构体。
        /// </summary>
        internal struct ARGB {
            public int A;
            public int R;
            public int G;
            public int B;
        }

        /// <summary>
        /// 用于匹配 `rgb(r,g,b)` 或者 `rgba(r,g,b,a)` 的颜色表达式。
        /// </summary>
        private static readonly Regex _syntaxRGBA = new Regex(@"rgba?\s*\(\s*(?:(?<r>[0-9.]+)\s*(?:,|\))\s*)(?:(?<g>[0-9.]+)\s*(?:,|\))\s*)(?:(?<b>[0-9.]+)\s*(?:,|\))\s*)(?:(?<a>[0-9.]+)\s*(?:,|\))\s*)?");

        /// <summary>
        /// 用于匹配 `#AARRGGBB` 或者 `#RRGGBB` 的颜色表达式。
        /// </summary>
        private static readonly Regex _syntaxHex = new Regex(@"#(?<a>[0-9a-fA-F]{2})?(?<r>[0-9a-fA-F]{2})(?<g>[0-9a-fA-F]{2})(?<b>[0-9a-fA-F]{2})");

        /// <summary>
        /// 将颜色字符串表达式转换成 ARGB 结构体。
        /// </summary>
        /// <param name="input">颜色字符表达式</param>
        private static ARGB ConvertStr2ARGB(string input) {
            var match = _syntaxHex.Match(input);
            if (match.Success) {
                var a = match.Groups["a"].Success ?
                          Convert.ToInt32(match.Groups["a"].Value, 16) :
                          255;
                var r = Convert.ToInt32(match.Groups["r"].Value, 16);
                var g = Convert.ToInt32(match.Groups["g"].Value, 16);
                var b = Convert.ToInt32(match.Groups["b"].Value, 16);
                return new ARGB { A = a, R = r, G = g, B = b };
            }
            match = _syntaxRGBA.Match(input);
            if (match.Success) {
                var a = match.Groups["a"].Success ?
                         Convert.ToInt32(Convert.ToDouble(match.Groups["a"].Value) * 255) :
                         255;
                var r = Convert.ToInt32(match.Groups["r"].Value);
                var g = Convert.ToInt32(match.Groups["g"].Value);
                var b = Convert.ToInt32(match.Groups["b"].Value);
                return new ARGB { A = a, R = r, G = g, B = b };
            }
            return new ARGB();
        }

        /// <summary>
        /// 将颜色字符串表达式转换成 <see cref="GDI.Color"/> 颜色对象。
        /// </summary>
        /// <param name="input">颜色字符表达式</param>
        public static GDI.Color GDIParse(string input) {
            var argb = ConvertStr2ARGB(input);
            return GDI.Color.FromArgb(argb.A, argb.R, argb.G, argb.B);
        }

        /// <summary>
        /// 将颜色字符串表达式转换成 <see cref="D2D.Color"/> 颜色对象。
        /// </summary>
        /// <param name="input">颜色字符表达式</param>
        public static D2D.Color Parse(string input) {
            var argb = ConvertStr2ARGB(input);
            return D2D.Color.FromArgb((byte)argb.A, (byte)argb.R, (byte)argb.G, (byte)argb.B);
        }
    }
}
