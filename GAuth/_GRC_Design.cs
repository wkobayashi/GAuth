using System;
using System.Drawing;

namespace GRC
{
    /// <summary>
    /// Designクラス
    /// </summary>
    /// <remarks>
    /// 配置・配色に関するユーティリティクラスです。
    /// 配置・配色以外の意匠に関する注意点としては『マウスカーソルやタブストップの動線』や『Windowsの慣習にしたがったUI』があります。
    /// このクラスを継承することはできませんし, インスタンスを生成することもできません。
    /// </remarks>
    public sealed class Design
    {
        private Design() { }
        /// <summary>
        /// 黄金比
        /// </summary>
        /// <remarks>
        /// http://ja.wikipedia.org/wiki/%E9%BB%84%E9%87%91%E6%AF%94
        /// Lを11とl2に黄金分割するならl1=L/τ, l2=L/(τ^2)で分割可能です。
        /// </remarks>
        public static double GoldenRatio
        {
            get
            {
                return (1 + Math.Sqrt(5)) / 2;
            }
        }
        /// <summary>
        /// 白銀比
        /// </summary>
        /// <remarks>
        /// http://ja.wikipedia.org/wiki/%E7%99%BD%E9%8A%80%E6%AF%94
        /// </remarks>
        public static double SilverRatio
        {
            get
            {
                return Math.Sqrt(2);
            }
        }
        /// <summary>
        /// 青銅比
        /// </summary>
        /// <remarks>
        /// http://ja.wikipedia.org/wiki/%E9%9D%92%E9%8A%85%E6%AF%94
        /// </remarks>
        public static double BronzeRatio
        {
            get
            {
                return (3 + Math.Sqrt(13)) / 2;
            }
        }
        /// <summary>
        /// SystemColors.ActiveCaptionを基準に彩度1・明度1・色相0度の色
        /// </summary>
        /// <remarks>
        /// 通常, SystemColors.ActiveCaptionは青なので, 一般的なウィンドウでは5%程度含まれます。
        /// これを軽い注意を促したい箇所で使用してください。
        /// この色がウィンドウを占める割合は25%程度を目安にしてください。
        /// </remarks>
        public static Color Color000deg
        {
            get
            {
                return MakeColorFromHSV(SystemColors.ActiveCaption.GetHue(), 1, 1);
            }
        }
        /// <summary>
        /// SystemColors.ActiveCaptionを基準に彩度1・明度1・色相90度の色
        /// </summary>
        /// <remarks>
        /// 通常, SystemColors.ActiveCaptionは青なので, 90度は緑になります。
        /// これをブリンキングなど変化する箇所で補色などとともに使用してください。
        /// この色がウィンドウを占める割合は5%程度(または色相0度と合計して25%)を目安にしてください。
        /// </remarks>
        public static Color Color090deg
        {
            get
            {
                return MakeColorFromHSV(SystemColors.ActiveCaption.GetHue() + 90, 1, 1);
            }
        }
        /// <summary>
        /// SystemColors.ActiveCaptionを基準に彩度1・明度1・色相180度の色
        /// </summary>
        /// <remarks>
        /// 通常, SystemColors.ActiveCaptionは青なので, 180度(補色)は橙になります。
        /// これを重い注意を促したい箇所で使用してください。
        /// この色がウィンドウを占める割合は5%程度を目安にしてください。
        /// </remarks>
        public static Color Color180deg
        {
            get
            {
                return MakeColorFromHSV(SystemColors.ActiveCaption.GetHue() + 180, 1, 1);
            }
        }
        /// <summary>
        /// SystemColors.ActiveCaptionを基準に彩度1・明度1・色相270度の色
        /// </summary>
        /// <remarks>
        /// 通常, SystemColors.ActiveCaptionは青なので, 270度は紫になります。
        /// これを警告など非常に重い注意を促したい箇所で使用してください。
        /// この色を使用するときは1箇所(ひとつの文・アイコンひとつ)を目安にしてください。
        /// </remarks>
        public static Color Color270deg
        {
            get
            {
                return MakeColorFromHSV(SystemColors.ActiveCaption.GetHue() + 270, 1, 1);
            }
        }
        /// <summary>
        /// HSVモデルで指定したColorクラスを生成します。
        /// </summary>
        /// <param name="hue">色相(Hue)</param>
        /// <param name="saturation">彩度(Saturation・Chroma)</param>
        /// <param name="value">明度(Value・Lightness・Brightness)</param>
        /// <returns>
        /// Colorクラスが返ります。
        /// </returns>
        /// <remarks>
        /// http://ja.wikipedia.org/wiki/HSV%E8%89%B2%E7%A9%BA%E9%96%93
        /// HSV円錐空間の座標からColorクラスを生成します。
        /// </remarks>
        public static Color MakeColorFromHSV(float hue, float saturation, float value)
        {
            if ((saturation < 0) || (1 < saturation) || (value < 0) || (1 < value)) return Color.Black;
            while ((hue < 0) || (360 < hue)) hue += (hue < 0) ? 360 : -360;
            int Hi = (int)Math.Truncate(hue / 60) % 6;
            float f = (hue / 60) - Hi;
            float p = value * (1 -  1      * saturation);
            float q = value * (1 -  f      * saturation);
            float t = value * (1 - (1 - f) * saturation);
            float r = 0, g = 0, b = 0;
            switch (Hi)
            {
                case 0: r = value; g = t    ; b = p    ; break;
                case 1: r = q    ; g = value; b = p    ; break;
                case 2: r = p    ; g = value; b = t    ; break;
                case 3: r = p    ; g = q    ; b = value; break;
                case 4: r = t    ; g = p    ; b = value; break;
                case 5: r = value; g = p    ; b = q    ; break;
            }
            return ColorTranslator.FromWin32((255 << 24) + ((int)(b * 255) << 16) + ((int)(g * 255) << 8) + ((int)(r * 255) << 0));
        }
    }
}
