using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SolutionComponentExtractor.UI
{
    public enum DialogKind { Info, Success, Warning, Error, Question }

    /// <summary>A titled paragraph of a dialog.</summary>
    public sealed class DialogSection
    {
        public DialogSection(string heading, string text)
        {
            Heading = heading;
            Text = text;
        }

        public string Heading { get; }
        public string Text { get; }
    }

    /// <summary>Message box styled like the tool (replaces MessageBox.Show).</summary>
    public static class ModernDialog
    {
        /// <summary>Shows the dialog; returns true when the primary button is clicked.</summary>
        /// <param name="items">Optional list shown in a gray box under the message (components, files...).</param>
        /// <param name="sections">Optional titled paragraphs shown after the list.</param>
        /// <param name="secondary">Label of the secondary button (cancel); null shows the primary button only.</param>
        public static bool Show(IWin32Window owner, DialogKind kind, string title, string message,
            IEnumerable<string> items = null, IEnumerable<DialogSection> sections = null,
            string primary = "OK", string secondary = null)
        {
            using (var form = new ModernDialogForm(kind, title, message, items?.ToList(), sections?.ToList(), primary, secondary))
            {
                return form.ShowDialog(owner) == DialogResult.OK;
            }
        }

        private sealed class ModernDialogForm : Form
        {
            private const int WidthAt96Dpi = 560;
            private const int MaxContentHeightAt96Dpi = 460;
            private readonly string copyText;
            private readonly float scale;
            private readonly FlowLayoutPanel text;
            private readonly int maxContentHeight;

            public ModernDialogForm(DialogKind kind, string title, string message, List<string> items, List<DialogSection> sections, string primary, string secondary)
            {
                scale = DeviceDpi / 96f;
                var (color, lightColor, glyph) = Style(kind);

                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.CenterParent;
                ShowInTaskbar = false;
                KeyPreview = true;
                Font = new Font("Segoe UI", 9.5f);
                BackColor = Theme.Border; // 1px border around the white surface
                Padding = new Padding(1);
                Text = title;

                var surface = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface };
                Controls.Add(surface);

                // Footer with the buttons (docked first so the content fills what is left).
                var footer = new Panel { Dock = DockStyle.Bottom, Height = S(64), BackColor = Theme.SurfaceAlt };
                footer.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.Border });
                var buttons = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(S(16), S(13), S(16), 0),
                    WrapContents = false,
                };
                var primaryButton = CreateButton(primary, isPrimary: true, DialogResult.OK);
                buttons.Controls.Add(primaryButton);
                AcceptButton = primaryButton;
                if (secondary != null)
                {
                    var secondaryButton = CreateButton(secondary, isPrimary: false, DialogResult.Cancel);
                    buttons.Controls.Add(secondaryButton);
                    CancelButton = secondaryButton;
                }
                footer.Controls.Add(buttons);
                buttons.BringToFront();
                surface.Controls.Add(footer);

                // Colored strip on top, matching the kind of message.
                var strip = new Panel { Dock = DockStyle.Top, Height = S(4), BackColor = color };

                // Icon badge + text.
                var body = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Padding = new Padding(S(24), S(22), S(24), S(16)),
                };
                body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, S(60)));
                body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                var badge = new IconBadge(glyph, color, lightColor) { Size = new Size(S(44), S(44)), Margin = new Padding(0, S(2), 0, 0), Anchor = AnchorStyles.Top | AnchorStyles.Left };
                body.Controls.Add(badge, 0, 0);

                var textWidth = S(WidthAt96Dpi) - S(24) * 2 - S(60) - S(20);
                text = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    Margin = new Padding(0),
                };
                text.Controls.Add(CreateLabel(title, new Font("Segoe UI Semibold", 13f), Theme.TextPrimary, textWidth, bottom: 8));
                if (!string.IsNullOrWhiteSpace(message)) text.Controls.Add(CreateLabel(message, Font, Theme.TextPrimary, textWidth, bottom: 10));
                if (items != null && items.Count > 0) text.Controls.Add(CreateItemsBox(items, textWidth));
                foreach (var section in sections ?? new List<DialogSection>())
                {
                    text.Controls.Add(CreateLabel(section.Heading, new Font("Segoe UI Semibold", 9.5f), Theme.TextPrimary, textWidth, bottom: 2));
                    text.Controls.Add(CreateLabel(section.Text, Font, Theme.TextSecondary, textWidth, bottom: 10));
                }
                body.Controls.Add(text, 1, 0);

                surface.Controls.Add(body);
                surface.Controls.Add(strip);
                body.BringToFront();

                // Height follows the content, up to a maximum (then the text scrolls). This first estimate
                // is corrected in OnLoad, once the labels are laid out (see FitToContent).
                var contentHeight = text.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical);
                var screenLimit = (int)(Screen.FromPoint(Cursor.Position).WorkingArea.Height * 0.7) - S(160);
                maxContentHeight = Math.Max(S(MaxContentHeightAt96Dpi), screenLimit);
                var visibleHeight = Math.Min(Math.Max(contentHeight, S(48)), maxContentHeight);
                ClientSize = new Size(S(WidthAt96Dpi), 2 + strip.Height + body.Padding.Vertical + visibleHeight + footer.Height);

                // Drag the dialog from anywhere but the buttons.
                foreach (Control control in new Control[] { surface, body, text, strip, badge }.Concat(text.Controls.Cast<Control>()))
                {
                    control.MouseDown += DragWindow;
                }

                copyText = string.Join(Environment.NewLine + Environment.NewLine, new[] { title, message }
                    .Concat(items == null ? Enumerable.Empty<string>() : new[] { string.Join(Environment.NewLine, items.Select(i => "• " + i)) })
                    .Concat((sections ?? new List<DialogSection>()).Select(s => s.Heading + Environment.NewLine + s.Text))
                    .Where(s => !string.IsNullOrWhiteSpace(s)));
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    const int CS_DROPSHADOW = 0x20000;
                    var cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                FitToContent();
            }

            /// <summary>Resizes the dialog to the real height of its text, keeping it centered on the same point.</summary>
            private void FitToContent()
            {
                text.PerformLayout();
                var last = text.Controls.Cast<Control>().LastOrDefault();
                if (last == null) return;

                var needed = last.Bottom - text.AutoScrollPosition.Y + last.Margin.Bottom;
                var target = Math.Min(Math.Max(needed, S(48)), maxContentHeight);
                var delta = target - text.ClientSize.Height;
                if (delta == 0) return;

                Height += delta;
                Top -= delta / 2;
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetText(copyText);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = CancelButton != null ? DialogResult.Cancel : DialogResult.OK;
                }
            }

            private int S(int value) => (int)Math.Round(value * scale);

            private static (Color color, Color light, string glyph) Style(DialogKind kind)
            {
                switch (kind)
                {
                    case DialogKind.Success: return (Theme.Success, Theme.SuccessLight, Theme.Glyphs.Success);
                    case DialogKind.Warning: return (Theme.Warning, Theme.WarningLight, Theme.Glyphs.Warning);
                    case DialogKind.Error: return (Theme.Error, Theme.ErrorLight, Theme.Glyphs.Error);
                    case DialogKind.Question: return (Theme.Accent, Theme.AccentLight, Theme.Glyphs.Question);
                    default: return (Theme.Accent, Theme.AccentLight, Theme.Glyphs.Info);
                }
            }

            private Label CreateLabel(string value, Font font, Color color, int width, int bottom)
            {
                return new Label
                {
                    Text = value,
                    Font = font,
                    ForeColor = color,
                    AutoSize = true,
                    MaximumSize = new Size(width, 0),
                    Margin = new Padding(0, 0, 0, S(bottom)),
                    UseMnemonic = false,
                };
            }

            private Control CreateItemsBox(List<string> items, int width)
            {
                var box = new Panel
                {
                    BackColor = Theme.Background,
                    Padding = new Padding(S(12), S(8), S(12), S(8)),
                    Margin = new Padding(0, 0, 0, S(12)),
                    Width = width,
                };
                var label = new Label
                {
                    Text = string.Join(Environment.NewLine, items.Select(i => "•  " + i)),
                    AutoSize = true,
                    MaximumSize = new Size(width - box.Padding.Horizontal, 0),
                    ForeColor = Theme.TextPrimary,
                    Location = new Point(box.Padding.Left, box.Padding.Top),
                    UseMnemonic = false,
                };
                box.Controls.Add(label);
                box.Height = label.PreferredHeight + box.Padding.Vertical;
                label.MouseDown += DragWindow;
                return box;
            }

            private Button CreateButton(string label, bool isPrimary, DialogResult result)
            {
                var button = new Button
                {
                    Text = label,
                    DialogResult = result,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 9.5f),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowOnly,
                    MinimumSize = new Size(S(110), S(36)),
                    Padding = new Padding(S(12), 0, S(12), 0),
                    Margin = new Padding(S(8), 0, 0, 0),
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false,
                    BackColor = isPrimary ? Theme.Accent : Theme.Surface,
                    ForeColor = isPrimary ? Color.White : Theme.TextPrimary,
                };
                button.FlatAppearance.BorderSize = isPrimary ? 0 : 1;
                button.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 209);
                button.FlatAppearance.MouseOverBackColor = isPrimary ? Theme.AccentHover : Theme.Background;
                button.FlatAppearance.MouseDownBackColor = isPrimary ? Theme.AccentPressed : Theme.Border;
                return button;
            }

            [DllImport("user32.dll")]
            private static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            private void DragWindow(object sender, MouseEventArgs e)
            {
                const int WM_NCLBUTTONDOWN = 0xA1;
                const int HTCAPTION = 2;
                if (e.Button != MouseButtons.Left) return;
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            }
        }

        /// <summary>Round light badge with the icon glyph of the message kind.</summary>
        private sealed class IconBadge : Control
        {
            private readonly string glyph;
            private readonly Color color;
            private readonly Color light;

            public IconBadge(string glyph, Color color, Color light)
            {
                this.glyph = glyph;
                this.color = color;
                this.light = light;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
                BackColor = Color.Transparent;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(light))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
                }
                using (var font = Theme.IconFont(Height * 0.3f))
                {
                    if (font == null) return;
                    TextRenderer.DrawText(e.Graphics, glyph, font, ClientRectangle, color,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
            }
        }
    }
}
