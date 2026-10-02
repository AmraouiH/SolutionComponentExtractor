using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SolutionComponentExtractor.UI
{
    /// <summary>Colors, fonts and icons shared by the tool UI (Fluent-like palette).</summary>
    internal static class Theme
    {
        public static readonly Color Accent = Color.FromArgb(15, 108, 189);
        public static readonly Color AccentHover = Color.FromArgb(17, 94, 163);
        public static readonly Color AccentPressed = Color.FromArgb(12, 59, 94);
        public static readonly Color AccentLight = Color.FromArgb(235, 243, 252);
        public static readonly Color Surface = Color.White;
        public static readonly Color Background = Color.FromArgb(245, 245, 245);
        public static readonly Color Border = Color.FromArgb(224, 224, 224);
        public static readonly Color TextPrimary = Color.FromArgb(36, 36, 36);
        public static readonly Color TextSecondary = Color.FromArgb(97, 97, 97);
        public static readonly Color TextDisabled = Color.FromArgb(160, 160, 160);
        public static readonly Color Success = Color.FromArgb(16, 124, 16);
        public static readonly Color SuccessLight = Color.FromArgb(223, 246, 221);
        public static readonly Color Warning = Color.FromArgb(188, 75, 9);
        public static readonly Color WarningLight = Color.FromArgb(255, 244, 206);
        public static readonly Color Error = Color.FromArgb(196, 43, 28);
        public static readonly Color ErrorLight = Color.FromArgb(253, 231, 233);
        public static readonly Color SurfaceAlt = Color.FromArgb(250, 250, 250);

        /// <summary>Segoe Fluent Icons / Segoe MDL2 Assets code points.</summary>
        public static class Glyphs
        {
            public const string Close = "";
            public const string OpenFile = "";
            public const string CheckAll = "";
            public const string ClearAll = "";
            public const string Save = "";
            public const string Upload = "";
            public const string Log = "";
            public const string Package = "";
            public const string Info = "";
            public const string Warning = "";
            public const string Error = "";
            public const string Success = "";
            public const string Copy = "";
            public const string Accept = "";
            public const string Remove = "";
            public const string Filter = "";
            public const string Code = "";
            public const string Question = "";
            public const string Help = "";
        }

        private static string iconFontName;

        private static string IconFontName
        {
            get
            {
                if (iconFontName != null) return iconFontName;
                using (var fonts = new InstalledFontCollection())
                {
                    foreach (var name in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
                    {
                        foreach (var family in fonts.Families)
                        {
                            if (family.Name == name) return iconFontName = name;
                        }
                    }
                }
                return iconFontName = string.Empty;
            }
        }

        /// <summary>Renders an icon-font glyph into a bitmap; returns null when no icon font is installed.</summary>
        public static Bitmap Icon(string glyph, Color color, int size = 16)
        {
            if (IconFontName.Length == 0) return null;

            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            using (var font = new Font(IconFontName, size * 0.75f, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), format);
            }
            return bitmap;
        }

        public static Font IconFont(float size)
        {
            return IconFontName.Length == 0 ? null : new Font(IconFontName, size);
        }

        public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        /// <summary>Gray placeholder text shown in an empty text box.</summary>
        public static void SetPlaceholder(TextBox textBox, string text)
        {
            const int EM_SETCUEBANNER = 0x1501;
            if (textBox.IsHandleCreated) SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
            else textBox.HandleCreated += (s, e) => SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }
    }
}
