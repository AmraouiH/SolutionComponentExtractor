using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SolutionComponentExtractor.UI
{
    /// <summary>Rounded, dashed "drop your file here" area.</summary>
    public class DropZonePanel : Panel
    {
        private bool highlighted;
        private string title = "Drop a solution .zip here";
        private string subtitle = "or browse for an unmanaged solution exported from Dynamics 365 / Dataverse";

        public DropZonePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Theme.Surface;
            Cursor = Cursors.Hand;
        }

        [Category("Appearance"), DefaultValue("Drop a solution .zip here")]
        public string Title
        {
            get => title;
            set { title = value; Invalidate(); }
        }

        [Category("Appearance")]
        public string Subtitle
        {
            get => subtitle;
            set { subtitle = value; Invalidate(); }
        }

        /// <summary>True while a file is dragged over the zone or the mouse hovers it.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Highlighted
        {
            get => highlighted;
            set
            {
                if (highlighted == value) return;
                highlighted = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            Highlighted = true;
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!ClientRectangle.Contains(PointToClient(MousePosition))) Highlighted = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Theme.Background);

            var bounds = new Rectangle(1, 1, Width - 3, Height - 3);
            using (var path = Theme.RoundedRectangle(bounds, 10))
            using (var fill = new SolidBrush(highlighted ? Theme.AccentLight : Theme.Surface))
            using (var pen = new Pen(highlighted ? Theme.Accent : Color.FromArgb(189, 189, 189), 1.6f) { DashStyle = DashStyle.Dash })
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            var y = (int)(Height * 0.16);
            using (var iconFont = Theme.IconFont(30f))
            {
                if (iconFont != null)
                {
                    TextRenderer.DrawText(g, Theme.Glyphs.Package, iconFont, new Rectangle(0, y, Width, 56), Theme.Accent,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            y += 62;

            using (var titleFont = new Font("Segoe UI Semibold", 13f))
            {
                TextRenderer.DrawText(g, title, titleFont, new Rectangle(16, y, Width - 32, 30), Theme.TextPrimary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            y += 32;

            using (var subtitleFont = new Font("Segoe UI", 9f))
            {
                TextRenderer.DrawText(g, subtitle, subtitleFont, new Rectangle(24, y, Width - 48, 40), Theme.TextSecondary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }
        }
    }
}
