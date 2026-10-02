using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using SolutionComponentExtractor.Core;
using SolutionComponentExtractor.UI;
using Label = System.Windows.Forms.Label;

namespace SolutionComponentExtractor
{
    /// <summary>
    /// Read-only live preview of the XML of the new solution, on the right of the component list.
    /// It is recomputed in the background each time the selection or the new solution fields change.
    /// </summary>
    public partial class MyPluginControl
    {
        private const int HighlightMarker = 3;
        private const int MaxHighlightedLines = 5000;

        private readonly Timer previewTimer = new Timer { Interval = 300 };
        private SplitContainer splitMain;
        private Scintilla previewEditor;
        private ToolStripButton tsbShowPreview;
        private ToolStripButton tsbPreviewCustomizations;
        private ToolStripButton tsbPreviewSolution;
        private ToolStripLabel tslPreviewSize;
        private Label lblPreviewStatus;

        private SolutionPreview preview;
        private int previewVersion;
        private SolutionComponent focusAfterPreview;

        private bool ShowingCustomizations => tsbPreviewCustomizations.Checked;

        private void InitializePreview()
        {
            // The component lists move to the left part of a new split; the preview takes the right part.
            splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 12,
                BackColor = Theme.Background,
            };
            splitMain.Panel2.BackColor = Theme.Surface;
            splitMain.Panel2.Padding = new Padding(1);
            pnlContent.Controls.Remove(splitContent);
            splitMain.Panel1.Controls.Add(splitContent);
            pnlContent.Controls.Add(splitMain);
            splitContent.SplitterDistance = 240;

            previewEditor = CreateEditor();
            splitMain.Panel2.Controls.Add(previewEditor);

            lblPreviewStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                Padding = new Padding(10, 0, 10, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Theme.TextSecondary,
                AutoEllipsis = true,
                BackColor = Theme.Surface,
            };
            splitMain.Panel2.Controls.Add(lblPreviewStatus);
            splitMain.Panel2.Controls.Add(CreatePreviewHeader());

            tsbShowPreview = new ToolStripButton("XML preview", Theme.Icon(Theme.Glyphs.Code, Theme.TextPrimary))
            {
                Alignment = ToolStripItemAlignment.Right,
                CheckOnClick = true,
                Checked = true,
                ToolTipText = "Show or hide the live preview of the generated XML",
            };
            tsbShowPreview.CheckedChanged += (s, e) =>
            {
                splitMain.Panel2Collapsed = !tsbShowPreview.Checked;
                if (tsbShowPreview.Checked) RefreshPreview();
            };
            toolStripMenu.Items.Add(tsbShowPreview);

            previewTimer.Tick += (s, e) => RefreshPreview();
            lvComponents.SelectedIndexChanged += (s, e) =>
            {
                if (lvComponents.SelectedItems.Count == 1) HighlightComponent((SolutionComponent)lvComponents.SelectedItems[0].Tag, scroll: true);
            };
            pnlContent.VisibleChanged += (s, e) =>
            {
                if (pnlContent.Visible && splitMain.Width > 0) splitMain.SplitterDistance = (int)(splitMain.Width * 0.56);
            };
        }

        private ToolStrip CreatePreviewHeader()
        {
            var header = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Theme.Surface,
                RenderMode = ToolStripRenderMode.System,
                Padding = new Padding(6, 6, 6, 4),
                AutoSize = false,
                Height = 40,
            };
            var title = new ToolStripLabel("PREVIEW (READ-ONLY)")
            {
                Font = new Font("Segoe UI Semibold", 8.25f),
                ForeColor = Theme.TextSecondary,
                Margin = new Padding(4, 1, 12, 2),
            };
            tsbPreviewCustomizations = new ToolStripButton("customizations.xml") { Checked = true, DisplayStyle = ToolStripItemDisplayStyle.Text };
            tsbPreviewSolution = new ToolStripButton("solution.xml") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            tsbPreviewCustomizations.Click += (s, e) => SwitchPreviewFile(customizations: true);
            tsbPreviewSolution.Click += (s, e) => SwitchPreviewFile(customizations: false);
            tslPreviewSize = new ToolStripLabel { Alignment = ToolStripItemAlignment.Right, ForeColor = Theme.TextSecondary };

            header.Items.AddRange(new ToolStripItem[] { title, tsbPreviewCustomizations, tsbPreviewSolution, tslPreviewSize });
            return header;
        }

        private static Scintilla CreateEditor()
        {
            var editor = new Scintilla
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Lexer = Lexer.Xml,
                ReadOnly = true,
                WrapMode = WrapMode.None,
                ScrollWidthTracking = true,
                ScrollWidth = 1,
                CaretLineVisible = true,
                CaretLineBackColor = Color.FromArgb(246, 248, 250),
                IndentationGuides = IndentView.LookBoth,
                AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change,
            };

            editor.StyleResetDefault();
            editor.Styles[Style.Default].Font = "Consolas";
            editor.Styles[Style.Default].Size = 9;
            editor.Styles[Style.Default].ForeColor = Theme.TextPrimary;
            editor.StyleClearAll();

            var tag = Color.FromArgb(128, 0, 0);
            var attribute = Color.FromArgb(230, 0, 0);
            var value = Color.FromArgb(0, 0, 230);
            editor.Styles[Style.Xml.Tag].ForeColor = tag;
            editor.Styles[Style.Xml.TagEnd].ForeColor = tag;
            editor.Styles[Style.Xml.TagUnknown].ForeColor = tag;
            editor.Styles[Style.Xml.Attribute].ForeColor = attribute;
            editor.Styles[Style.Xml.AttributeUnknown].ForeColor = attribute;
            editor.Styles[Style.Xml.DoubleString].ForeColor = value;
            editor.Styles[Style.Xml.SingleString].ForeColor = value;
            editor.Styles[Style.Xml.Number].ForeColor = value;
            editor.Styles[Style.Xml.Comment].ForeColor = Theme.Success;
            editor.Styles[Style.Xml.CData].ForeColor = Theme.TextSecondary;
            editor.Styles[Style.Xml.XmlStart].ForeColor = Theme.TextSecondary;
            editor.Styles[Style.Xml.XmlEnd].ForeColor = Theme.TextSecondary;
            editor.Styles[Style.Xml.Entity].ForeColor = Theme.Warning;
            editor.Styles[Style.LineNumber].ForeColor = Theme.TextDisabled;
            editor.Styles[Style.LineNumber].BackColor = Theme.Surface;
            editor.SetSelectionBackColor(true, Color.FromArgb(204, 228, 247));

            // Line numbers and folding.
            editor.Margins[0].Type = MarginType.Number;
            editor.Margins[0].Width = 48;
            editor.Margins[1].Width = 0;
            editor.SetProperty("fold", "1");
            editor.SetProperty("fold.compact", "0");
            editor.SetProperty("fold.html", "1");
            editor.Margins[2].Type = MarginType.Symbol;
            editor.Margins[2].Mask = Marker.MaskFolders;
            editor.Margins[2].Sensitive = true;
            editor.Margins[2].Width = 16;
            editor.SetFoldMarginColor(true, Theme.Surface);
            editor.SetFoldMarginHighlightColor(true, Theme.Surface);
            foreach (var marker in new[] { Marker.Folder, Marker.FolderOpen, Marker.FolderEnd, Marker.FolderOpenMid, Marker.FolderMidTail, Marker.FolderSub, Marker.FolderTail })
            {
                editor.Markers[marker].SetForeColor(Theme.Surface);
                editor.Markers[marker].SetBackColor(Theme.TextDisabled);
            }
            editor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            editor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            editor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            editor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            editor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            editor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            editor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Background of the lines of the component selected in the list.
            editor.Markers[HighlightMarker].Symbol = MarkerSymbol.Background;
            editor.Markers[HighlightMarker].SetBackColor(Color.FromArgb(255, 244, 206));
            return editor;
        }

        /// <summary>Recomputes the preview shortly after the last change; <paramref name="focus"/> is shown once ready.</summary>
        private void SchedulePreview(SolutionComponent focus = null)
        {
            if (package == null || !tsbShowPreview.Checked) return;
            if (focus != null) focusAfterPreview = focus;
            lblPreviewStatus.Text = "Updating the preview...";
            previewTimer.Stop();
            previewTimer.Start();
        }

        private void RefreshPreview()
        {
            previewTimer.Stop();
            if (package == null || !tsbShowPreview.Checked) return;

            // Snapshot of the UI state: the computation runs on a background thread.
            var source = package;
            var selected = source.Components.Where(c => c.Selected).ToList();
            var options = new ExtractOptions
            {
                UniqueName = SolutionExtractor.IsValidUniqueName(txtUniqueName.Text) ? txtUniqueName.Text : null,
                DisplayName = txtDisplayName.Text,
                Version = SolutionExtractor.IsValidVersion(txtVersion.Text) ? txtVersion.Text : null,
            };
            var version = ++previewVersion;
            lblPreviewStatus.Text = "Updating the preview...";

            Task.Run(() => SolutionExtractor.Preview(source, options, selected))
                .ContinueWith(task =>
                {
                    // Ignore results of an older selection or of a solution that is no longer open.
                    if (IsDisposed || version != previewVersion || source != package) return;
                    if (task.IsFaulted)
                    {
                        lblPreviewStatus.Text = "Preview failed: " + task.Exception?.GetBaseException().Message;
                        return;
                    }

                    preview = task.Result;
                    ShowPreviewText(keepScroll: true);

                    var focus = focusAfterPreview;
                    focusAfterPreview = null;
                    if (focus != null && focus.Selected) HighlightComponent(focus, scroll: true);
                    else HighlightSelectedRow(scroll: false);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ClearPreview()
        {
            previewVersion++;
            preview = null;
            focusAfterPreview = null;
            SetEditorText(string.Empty);
            tslPreviewSize.Text = string.Empty;
            lblPreviewStatus.Text = string.Empty;
        }

        private void SwitchPreviewFile(bool customizations)
        {
            tsbPreviewCustomizations.Checked = customizations;
            tsbPreviewSolution.Checked = !customizations;
            ShowPreviewText(keepScroll: false);
            HighlightSelectedRow(scroll: true);
        }

        private void SetEditorText(string text)
        {
            previewEditor.ReadOnly = false;
            previewEditor.Text = text;
            previewEditor.ReadOnly = true;
            previewEditor.EmptyUndoBuffer();
        }

        private void ShowPreviewText(bool keepScroll)
        {
            if (preview == null) return;

            var text = ShowingCustomizations ? preview.CustomizationsXml : preview.SolutionXml;
            var firstLine = previewEditor.FirstVisibleLine;
            SetEditorText(text);
            previewEditor.FirstVisibleLine = keepScroll ? firstLine : 0;

            var size = text.Length < 1024 * 1024 ? $"{text.Length / 1024.0:0.#} KB" : $"{text.Length / (1024.0 * 1024):0.##} MB";
            tslPreviewSize.Text = $"{previewEditor.Lines.Count:N0} lines · {size}";

            var result = preview.Result;
            lblPreviewStatus.ForeColor = Theme.TextSecondary;
            lblPreviewStatus.Text = $"{result.Kept.Count} kept · {result.Removed.Count} removed · {preview.FileCount} file(s) in the zip"
                                    + (result.Warnings.Count > 0 ? $" · {result.Warnings.Count} warning(s)" : string.Empty)
                                    + "   —   click a component to locate it";
        }

        private void HighlightSelectedRow(bool scroll)
        {
            previewEditor.MarkerDeleteAll(HighlightMarker);
            if (lvComponents.SelectedItems.Count == 1) HighlightComponent((SolutionComponent)lvComponents.SelectedItems[0].Tag, scroll);
        }

        /// <summary>Highlights the XML block of a component and scrolls to it, or explains why it is not in the file.</summary>
        private void HighlightComponent(SolutionComponent component, bool scroll)
        {
            if (preview == null || !tsbShowPreview.Checked) return;
            previewEditor.MarkerDeleteAll(HighlightMarker);

            var lines = ShowingCustomizations ? preview.CustomizationsLines : preview.SolutionLines;
            var file = ShowingCustomizations ? "customizations.xml" : "solution.xml";

            if (!lines.TryGetValue(component, out var range))
            {
                lblPreviewStatus.ForeColor = Theme.Warning;
                lblPreviewStatus.Text = DescribeMissing(component, file);
                return;
            }

            var start = range.Start - 1;
            var end = Math.Min(range.End - 1, Math.Min(start + MaxHighlightedLines, previewEditor.Lines.Count - 1));
            for (var line = start; line <= end; line++) previewEditor.Lines[line].MarkerAdd(HighlightMarker);

            if (scroll)
            {
                previewEditor.Lines[start].EnsureVisible();
                previewEditor.FirstVisibleLine = Math.Max(0, previewEditor.Lines[start].DisplayIndex - 3);
            }

            lblPreviewStatus.ForeColor = Theme.TextSecondary;
            lblPreviewStatus.Text = $"{component.TypeName} '{component.DisplayName}': lines {range.Start:N0}–{range.End:N0} of {file}";
        }

        private string DescribeMissing(SolutionComponent component, string file)
        {
            if (!component.Selected) return $"'{component.DisplayName}' is not kept: it is not in the new {file}.";
            if (preview.Result.Removed.Contains(component)) return $"'{component.DisplayName}' is removed because the component that contains it is not kept.";
            if (ShowingCustomizations && component.Definitions.Count == 0 && component.Folders.Count > 0)
            {
                return $"'{component.DisplayName}' is not defined in customizations.xml but in its own folder: {string.Join(", ", component.Folders)}";
            }
            if (ShowingCustomizations && !component.IsLocated) return $"'{component.DisplayName}' has no definition in customizations.xml (only its entry in solution.xml).";
            return $"'{component.DisplayName}' was not found in {file}.";
        }
    }
}
