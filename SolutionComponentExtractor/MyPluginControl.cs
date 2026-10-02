using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SolutionComponentExtractor.Core;
using SolutionComponentExtractor.UI;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Label = System.Windows.Forms.Label;

namespace SolutionComponentExtractor
{
    public partial class MyPluginControl : PluginControlBase
    {
        private enum LogLevel { Info, Success, Warning, Error }

        #region Help and support

        private void InitializeHelp()
        {
            // Right-aligned items are laid out from the right edge: coffee first so it ends up rightmost.
            var tsbCoffee = new ToolStripButton("Buy me a coffee", CreateCoffeeIcon())
            {
                Alignment = ToolStripItemAlignment.Right,
                ToolTipText = "Enjoying the tool? Support its development",
                Font = new Font(toolStripMenu.Font, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 73, 0),
            };
            tsbCoffee.Click += (s, e) => Process.Start(MyPlugin.BuyMeACoffeeUrl);
            toolStripMenu.Items.Add(tsbCoffee);

            var tsbHelp = new ToolStripButton("Help", Theme.Icon(Theme.Glyphs.Help, Theme.Accent))
            {
                Alignment = ToolStripItemAlignment.Right,
                ToolTipText = "How to use the tool, issues and contact",
            };
            tsbHelp.Click += (s, e) => ShowHelp();
            toolStripMenu.Items.Add(tsbHelp);
        }

        /// <summary>Coffee cup on a "Buy Me a Coffee" yellow badge.</summary>
        private static Bitmap CreateCoffeeIcon()
        {
            const int size = 18;
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            using (var badge = Theme.RoundedRectangle(new Rectangle(0, 0, size - 1, size - 1), 4))
            using (var yellow = new SolidBrush(Color.FromArgb(255, 221, 0)))
            using (var cup = new Font("Segoe UI Symbol", 12f, GraphicsUnit.Pixel))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillPath(yellow, badge);
                TextRenderer.DrawText(g, "☕", cup, new Rectangle(0, 0, size, size), Color.FromArgb(64, 40, 10),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
            return bitmap;
        }

        /// <summary>Help, issues and contact (same content as in Entity Fields Analyser).</summary>
        private void ShowHelp()
        {
            var nl = Environment.NewLine;
            var openLinkedIn = ModernDialog.Show(this, DialogKind.Info, "Help, Issue !!",
                "Solution Component Extractor creates a new solution that only contains the components you choose, ready to import into another environment.",
                sections: new[]
                {
                    new DialogSection("How to use it",
                        "1. Open or drop an unmanaged solution .zip exported from Dataverse (Solutions > Export > Unmanaged)." + nl
                        + "2. In Component types (left), tick or untick a type to keep or remove all its components; click a type to show only its components." + nl
                        + "3. In the list, tick the components to keep. Right-click selected rows to keep, remove or keep only them. Ctrl+F searches the list." + nl
                        + "4. Check the XML preview (right): customizations.xml and solution.xml exactly as they will be generated. Click a component to locate it." + nl
                        + "5. Optionally change the unique name, display name and version of the new solution." + nl
                        + "6. Click Generate solution to save the .zip, or Import into environment to import it into the connected environment."),
                    new DialogSection("Good to know",
                        "• Only unmanaged solutions can be opened." + nl
                        + "• Removing a table also removes its forms, views and charts." + nl
                        + "• Components marked \"Unmodified managed component\" have no content: they will not appear in the imported solution." + nl
                        + "• The Activity panel lists the removed files and every warning."),
                    new DialogSection("Issues and contact",
                        $"If you have any issues please contact me at {MyPlugin.AuthorEmail} or on LinkedIn."),
                    new DialogSection("Support the tool",
                        "Enjoying Solution Component Extractor? Click \"Buy me a coffee\" in the toolbar to support its development."),
                },
                primary: "Contact me on LinkedIn",
                secondary: "Close");

            if (openLinkedIn) Process.Start(MyPlugin.LinkedInUrl);
        }

        #endregion

        private const string AllTypesKey = "*";

        private readonly System.Windows.Forms.Timer searchTimer = new System.Windows.Forms.Timer { Interval = 250 };
        private Settings mySettings;
        private SolutionPackage package;
        private string selectedType = AllTypesKey;
        private bool updating;

        public MyPluginControl()
        {
            InitializeComponent();
            ApplyIcons();
            InitializePreview();
            InitializeHelp();

            Theme.SetPlaceholder(txtSearch, "Search by name, schema name, id or type...");
            Theme.SetPlaceholder(txtUniqueName, "Same as the source solution");
            Theme.SetPlaceholder(txtDisplayName, "Same as the source solution");
            Theme.SetPlaceholder(txtVersion, "1.0.0.0");

            searchTimer.Tick += (s, e) =>
            {
                searchTimer.Stop();
                BuildComponentList();
            };

            foreach (var control in new Control[] { this, pnlEmpty, dropZone, btnBrowse, pnlContent, splitContent, lvTypes, lvComponents })
            {
                control.AllowDrop = true;
                control.DragEnter += Solution_DragEnter;
                control.DragLeave += (s, e) => dropZone.Highlighted = false;
                control.DragDrop += Solution_DragDrop;
            }

            ShowPackage(null);
        }

        #region XrmToolBox lifecycle

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
            }
            UpdateConnectionInfo();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
            UpdateConnectionInfo();
        }

        private string EnvironmentName => ConnectionDetail?.ConnectionName ?? ConnectionDetail?.Organization;

        private void UpdateConnectionInfo()
        {
            var tooltip = EnvironmentName == null
                ? "Import the new solution into an environment (you will be asked to connect)"
                : $"Import the new solution into {EnvironmentName}";
            tsbImport.ToolTipText = tooltip;
            toolTip.SetToolTip(btnImport, tooltip);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.O:
                    OpenSolution();
                    return true;
                case Keys.Control | Keys.S when package != null:
                    GenerateSolution();
                    return true;
                case Keys.Control | Keys.F when package != null:
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Appearance

        private void ApplyIcons()
        {
            tsbClose.Image = Theme.Icon(Theme.Glyphs.Close, Theme.TextSecondary);
            tsbOpen.Image = Theme.Icon(Theme.Glyphs.OpenFile, Theme.Accent);
            tsbKeepAll.Image = Theme.Icon(Theme.Glyphs.CheckAll, Theme.TextPrimary);
            tsbKeepNone.Image = Theme.Icon(Theme.Glyphs.ClearAll, Theme.TextPrimary);
            tsbGenerate.Image = Theme.Icon(Theme.Glyphs.Save, Theme.Accent);
            tsbImport.Image = Theme.Icon(Theme.Glyphs.Upload, Theme.Accent);
            tsbShowLog.Image = Theme.Icon(Theme.Glyphs.Log, Theme.TextPrimary);
            btnGenerate.Image = Theme.Icon(Theme.Glyphs.Save, Color.White);
            btnImport.Image = Theme.Icon(Theme.Glyphs.Upload, Theme.Accent);
            tsmiKeepSelected.Image = Theme.Icon(Theme.Glyphs.Accept, Theme.Success);
            tsmiRemoveSelected.Image = Theme.Icon(Theme.Glyphs.Remove, Theme.Error);
            tsmiKeepOnlySelected.Image = Theme.Icon(Theme.Glyphs.Filter, Theme.Accent);
            tsmiCopyName.Image = Theme.Icon(Theme.Glyphs.Copy, Theme.TextPrimary);

            AddLogImage(LogLevel.Info, Theme.Glyphs.Info, Theme.Accent);
            AddLogImage(LogLevel.Success, Theme.Glyphs.Success, Theme.Success);
            AddLogImage(LogLevel.Warning, Theme.Glyphs.Warning, Theme.Warning);
            AddLogImage(LogLevel.Error, Theme.Glyphs.Error, Theme.Error);
        }

        private void AddLogImage(LogLevel level, string glyph, Color color)
        {
            var image = Theme.Icon(glyph, color);
            if (image != null) imlLog.Images.Add(level.ToString(), image);
        }

        private void dropZone_Resize(object sender, EventArgs e)
        {
            btnBrowse.Location = new Point((dropZone.Width - btnBrowse.Width) / 2, dropZone.Height - btnBrowse.Height - 36);
        }

        private void lvTypes_Resize(object sender, EventArgs e)
        {
            colTypeName.Width = Math.Max(80, lvTypes.ClientSize.Width - colTypeCount.Width - 4);
        }

        private void tsbShowLog_CheckedChanged(object sender, EventArgs e)
        {
            pnlLog.Visible = tsbShowLog.Checked;
        }

        private void Log(LogLevel level, string message)
        {
            var item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"))
            {
                ImageKey = level.ToString(),
                ToolTipText = message,
                ForeColor = level == LogLevel.Error ? Theme.Error : level == LogLevel.Warning ? Theme.Warning : Theme.TextPrimary,
            };
            item.SubItems.Add(message);
            lvLog.Items.Add(item);
            item.EnsureVisible();

            // Also write to the XrmToolBox log file.
            if (level == LogLevel.Error) LogError(message);
            else if (level == LogLevel.Warning) LogWarning(message);
            else LogInfo(message);
        }

        #endregion

        #region Opening a solution

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            OpenSolution();
        }

        private void OpenSolution()
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select an exported solution",
                Filter = "Solution zip (*.zip)|*.zip",
                InitialDirectory = mySettings?.LastFolder ?? string.Empty,
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK) LoadSolution(dialog.FileName);
            }
        }

        private static string GetDroppedZip(DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] files) || files.Length != 1) return null;
            return string.Equals(Path.GetExtension(files[0]), ".zip", StringComparison.OrdinalIgnoreCase) ? files[0] : null;
        }

        private void Solution_DragEnter(object sender, DragEventArgs e)
        {
            var file = GetDroppedZip(e);
            e.Effect = file != null ? DragDropEffects.Copy : DragDropEffects.None;
            dropZone.Highlighted = file != null;
        }

        private void Solution_DragDrop(object sender, DragEventArgs e)
        {
            dropZone.Highlighted = false;
            var file = GetDroppedZip(e);
            if (file != null) LoadSolution(file);
        }

        private void LoadSolution(string path)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Reading {Path.GetFileName(path)}...",
                Work = (worker, args) => args.Result = SolutionPackage.Load(path),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        Log(LogLevel.Error, $"Unable to read {Path.GetFileName(path)}: {args.Error.Message}");
                        ModernDialog.Show(this, DialogKind.Error, "Unable to read the solution",
                            $"{Path.GetFileName(path)} could not be opened.",
                            sections: new[]
                            {
                                new DialogSection("Details", args.Error.Message),
                                new DialogSection("What to do", "Open the .zip file exported from Dataverse as is (Solutions > Export), without unzipping or editing it."),
                            });
                        return;
                    }

                    if (mySettings != null) mySettings.LastFolder = Path.GetDirectoryName(path);

                    var loaded = (SolutionPackage)args.Result;
                    if (loaded.IsManaged)
                    {
                        // The tool is limited to unmanaged solutions; the solution already open (if any) stays as is.
                        Log(LogLevel.Warning, $"{Path.GetFileName(path)} was not opened: {loaded.UniqueName} {loaded.Version} is a managed solution.");
                        ModernDialog.Show(this, DialogKind.Warning, "Managed solution not supported",
                            $"{Path.GetFileName(path)} ({loaded.UniqueName} {loaded.Version}) is a managed solution. "
                            + "Solution Component Extractor only works with unmanaged solutions.",
                            sections: new[]
                            {
                                new DialogSection("Why?", "A managed solution is locked: it cannot be split into a new solution."),
                                new DialogSection("What to do", "Export the solution as Unmanaged from the source environment (Solutions > Export > Unmanaged) and open that file instead."),
                            },
                            primary: "Got it");
                        return;
                    }
                    ShowPackage(loaded);
                },
            });
        }

        private void ShowPackage(SolutionPackage loaded)
        {
            package = loaded;
            var hasPackage = package != null;

            pnlEmpty.Visible = !hasPackage;
            pnlContent.Visible = hasPackage;
            pnlFooter.Visible = hasPackage;
            foreach (var item in new ToolStripItem[] { tsbKeepAll, tsbKeepNone, tsbGenerate, tsbImport }) item.Enabled = hasPackage;

            ClearPreview();
            if (!hasPackage)
            {
                lblBadge.Visible = false;
                lblSelectionSummary.Text = string.Empty;
                return;
            }

            lblSolutionName.Text = package.DisplayName;
            lblSolutionDetails.Text = string.Join("   ·   ", new[]
            {
                package.UniqueName,
                $"Version {package.Version}",
                package.PublisherName == null ? null : $"Publisher {package.PublisherName}",
                $"{package.Components.Count} components",
                Path.GetFileName(package.SourcePath),
            }.Where(s => !string.IsNullOrEmpty(s)));

            // Only unmanaged solutions are opened (see LoadSolution).
            lblBadge.Visible = true;
            lblBadge.Text = "UNMANAGED";
            lblBadge.BackColor = Theme.SuccessLight;
            lblBadge.ForeColor = Theme.Success;

            updating = true;
            txtUniqueName.Text = package.UniqueName;
            txtDisplayName.Text = package.DisplayName;
            txtVersion.Text = package.Version;
            txtSearch.Text = string.Empty;
            chkKeptOnly.Checked = false;
            updating = false;
            ValidateFields();

            selectedType = AllTypesKey;
            BuildTypeList();
            BuildComponentList();
            RefreshPreview();

            Log(LogLevel.Info, $"Loaded {package.UniqueName} {package.Version}: {package.Components.Count} components.");

            var notLocated = package.Components.Count(c => !c.IsLocated);
            if (notLocated > 0)
            {
                Log(LogLevel.Warning, $"{notLocated} component(s) have no definition in customizations.xml: removing them only removes their entry in solution.xml.");
            }
        }

        #endregion

        #region Lists

        private void BuildTypeList()
        {
            updating = true;
            lvTypes.BeginUpdate();
            lvTypes.Items.Clear();

            var all = new ListViewItem("All components") { Name = AllTypesKey, Tag = AllTypesKey, Font = new Font(lvTypes.Font, FontStyle.Bold) };
            all.SubItems.Add(string.Empty);
            lvTypes.Items.Add(all);

            foreach (var typeName in package.Components.Select(c => c.TypeName).Distinct().OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase))
            {
                var item = new ListViewItem(typeName) { Name = typeName, Tag = typeName };
                item.SubItems.Add(string.Empty);
                lvTypes.Items.Add(item);
            }

            lvTypes.Items[AllTypesKey].Selected = true;
            lvTypes.EndUpdate();
            updating = false;
            lvTypes_Resize(lvTypes, EventArgs.Empty);
        }

        private IEnumerable<SolutionComponent> ComponentsOfType(string typeKey)
        {
            return typeKey == AllTypesKey ? package.Components : package.Components.Where(c => c.TypeName == typeKey);
        }

        private void BuildComponentList()
        {
            if (package == null) return;

            var filter = txtSearch.Text.Trim();
            var visible = ComponentsOfType(selectedType)
                .Where(c => !chkKeptOnly.Checked || c.Selected)
                .Where(c => filter.Length == 0
                            || Contains(c.DisplayName, filter) || Contains(c.SchemaName, filter)
                            || Contains(c.Id, filter) || Contains(c.TypeName, filter))
                .OrderBy(c => c.TypeName, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(c => c.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            updating = true;
            lvComponents.BeginUpdate();
            lvComponents.Items.Clear();
            lvComponents.Groups.Clear();
            lvComponents.ShowGroups = selectedType == AllTypesKey;

            var groups = new Dictionary<string, ListViewGroup>();
            var items = new List<ListViewItem>(visible.Count);
            foreach (var component in visible)
            {
                if (!groups.TryGetValue(component.TypeName, out var group))
                {
                    group = new ListViewGroup(component.TypeName, component.TypeName);
                    groups.Add(component.TypeName, group);
                    lvComponents.Groups.Add(group);
                }
                items.Add(CreateComponentItem(component, group));
            }
            lvComponents.Items.AddRange(items.ToArray());
            lvComponents.ApplySort();
            lvComponents.EndUpdate();
            updating = false;

            lblListTitle.Text = (selectedType == AllTypesKey ? "ALL COMPONENTS" : selectedType.ToUpperInvariant())
                                + $"   ·   {visible.Count} shown";
            RefreshChecks();
        }

        private static ListViewItem CreateComponentItem(SolutionComponent component, ListViewGroup group)
        {
            var details = new List<string>();
            var behavior = ComponentTypes.GetBehaviorLabel(component.Type, component.Behavior);
            if (behavior != null) details.Add(char.ToUpperInvariant(behavior[0]) + behavior.Substring(1));
            if (component.Parent != null) details.Add($"Inside {component.Parent.DisplayName}");
            if (component.Children.Count > 0) details.Add($"Contains {component.Children.Count} listed component(s)");
            if (!component.IsLocated) details.Add("Not found in customizations.xml");
            if (component.IsUnmodified) details.Add("Unmodified managed component: reference only, nothing to import");

            var item = new ListViewItem(component.DisplayName ?? component.Identifier, group)
            {
                Tag = component,
                Checked = component.Selected,
                ToolTipText = string.Join(Environment.NewLine, new[]
                {
                    $"{component.TypeName} (type {component.Type})",
                    $"Schema name: {component.SchemaName ?? "-"}",
                    $"Id: {component.Id ?? "-"}",
                }.Concat(details)),
            };
            item.SubItems.Add(component.SchemaName ?? component.Id ?? string.Empty);
            item.SubItems.Add(component.TypeName);
            item.SubItems.Add(string.Join(" · ", details));
            return item;
        }

        private static bool Contains(string value, string filter)
        {
            return value != null && value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>Synchronizes check boxes, colors and counters with the component selection.</summary>
        private void RefreshChecks()
        {
            if (package == null) return;

            updating = true;
            lvComponents.BeginUpdate();
            foreach (ListViewItem item in lvComponents.Items)
            {
                var component = (SolutionComponent)item.Tag;
                if (item.Checked != component.Selected) item.Checked = component.Selected;
                var color = !component.Selected ? Theme.TextDisabled : component.IsLocated ? Theme.TextPrimary : Theme.TextSecondary;
                if (item.ForeColor != color) item.ForeColor = color;
            }
            lvComponents.EndUpdate();

            lvTypes.BeginUpdate();
            foreach (ListViewItem item in lvTypes.Items)
            {
                var ofType = ComponentsOfType((string)item.Tag).ToList();
                var kept = ofType.Count(c => c.Selected);
                item.SubItems[1].Text = $"{kept} / {ofType.Count}";
                item.Checked = kept == ofType.Count;
                item.ForeColor = kept == 0 ? Theme.TextDisabled : Theme.TextPrimary;
            }
            lvTypes.EndUpdate();
            updating = false;

            var total = package.Components.Count;
            var keptCount = package.Components.Count(c => c.Selected);
            lblSelectionSummary.Text = $"{keptCount} of {total} components kept   ·   {total - keptCount} removed";
            lblSelectionSummary.ForeColor = keptCount == 0 ? Theme.Error : Theme.TextSecondary;
            btnGenerate.Enabled = btnImport.Enabled = tsbGenerate.Enabled = tsbImport.Enabled = keptCount > 0;
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            if (updating) return;
            searchTimer.Stop();
            if (sender == txtSearch) searchTimer.Start();
            else BuildComponentList();
        }

        private void lvTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating || lvTypes.SelectedItems.Count == 0) return;
            selectedType = (string)lvTypes.SelectedItems[0].Tag;
            BuildComponentList();
        }

        #endregion

        #region Selection

        private void lvTypes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (updating) return;
            foreach (var component in ComponentsOfType((string)e.Item.Tag).ToList()) SetSelected(component, e.Item.Checked);
            SelectionChanged();
        }

        private void lvComponents_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (updating) return;
            var component = (SolutionComponent)e.Item.Tag;
            SetSelected(component, e.Item.Checked);
            // A ticked component is shown in the preview as soon as it is recomputed.
            SelectionChanged(e.Item.Checked ? component : null);
        }

        private static void SetSelected(SolutionComponent component, bool selected)
        {
            if (selected)
            {
                // A sub-component cannot be kept without the component that contains its definition.
                for (var c = component; c != null; c = c.Parent) c.Selected = true;
            }
            else
            {
                component.Selected = false;
                foreach (var child in component.Children) SetSelected(child, false);
            }
        }

        private void SetSelected(IEnumerable<SolutionComponent> components, bool selected)
        {
            foreach (var component in components.ToList()) SetSelected(component, selected);
            SelectionChanged();
        }

        private void SelectionChanged(SolutionComponent focus = null)
        {
            // "Only components to keep" hides rows as soon as they are unticked.
            if (chkKeptOnly.Checked) BeginInvoke((Action)BuildComponentList);
            else RefreshChecks();
            SchedulePreview(focus);
        }

        private IEnumerable<SolutionComponent> SelectedRows()
        {
            return lvComponents.SelectedItems.Cast<ListViewItem>().Select(i => (SolutionComponent)i.Tag);
        }

        private void tsbKeepAll_Click(object sender, EventArgs e)
        {
            SetSelected(package.Components, true);
        }

        private void tsbKeepNone_Click(object sender, EventArgs e)
        {
            SetSelected(package.Components, false);
        }

        private void cmsComponents_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var count = lvComponents.SelectedItems.Count;
            if (count == 0)
            {
                e.Cancel = true;
                return;
            }
            var suffix = count == 1 ? "row" : $"{count} rows";
            tsmiKeepSelected.Text = $"Keep selected {suffix}";
            tsmiRemoveSelected.Text = $"Remove selected {suffix}";
            tsmiKeepOnlySelected.Text = $"Keep only the selected {suffix}";
            tsmiCopyName.Text = count == 1 ? "Copy schema name" : "Copy schema names";
        }

        private void tsmiKeepSelected_Click(object sender, EventArgs e)
        {
            SetSelected(SelectedRows(), true);
        }

        private void tsmiRemoveSelected_Click(object sender, EventArgs e)
        {
            SetSelected(SelectedRows(), false);
        }

        private void tsmiKeepOnlySelected_Click(object sender, EventArgs e)
        {
            var rows = SelectedRows().ToList();
            foreach (var component in package.Components) component.Selected = false;
            SetSelected(rows, true);
        }

        private void tsmiCopyName_Click(object sender, EventArgs e)
        {
            var names = SelectedRows().Select(c => c.Identifier).ToList();
            if (names.Count > 0) Clipboard.SetText(string.Join(Environment.NewLine, names));
        }

        private void lvComponents_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                lvComponents.BeginUpdate();
                foreach (ListViewItem item in lvComponents.Items) item.Selected = true;
                lvComponents.EndUpdate();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                tsmiCopyName_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && lvComponents.SelectedItems.Count > 0)
            {
                tsmiRemoveSelected_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Insert && lvComponents.SelectedItems.Count > 0)
            {
                tsmiKeepSelected_Click(sender, e);
                e.Handled = true;
            }
        }

        #endregion

        #region New solution fields

        private void Fields_TextChanged(object sender, EventArgs e)
        {
            if (updating) return;
            ValidateFields();
            SchedulePreview(); // name and version are part of solution.xml
        }

        private bool ValidateFields()
        {
            var uniqueNameValid = SolutionExtractor.IsValidUniqueName(txtUniqueName.Text);
            var versionValid = SolutionExtractor.IsValidVersion(txtVersion.Text);
            errorProvider.SetError(txtUniqueName, uniqueNameValid ? string.Empty : SolutionExtractor.InvalidUniqueNameMessage);
            errorProvider.SetError(txtVersion, versionValid ? string.Empty : SolutionExtractor.InvalidVersionMessage);
            return uniqueNameValid && versionValid;
        }

        private ExtractOptions GetOptions()
        {
            return new ExtractOptions
            {
                UniqueName = txtUniqueName.Text,
                DisplayName = txtDisplayName.Text,
                Version = txtVersion.Text,
            };
        }

        private bool CanGenerate(ExtractOptions options)
        {
            var errors = SolutionExtractor.Validate(package, options).ToList();
            if (errors.Count > 0)
            {
                ValidateFields();
                ModernDialog.Show(this, DialogKind.Warning, "Check the new solution",
                    errors.Count == 1 ? errors[0] : "Fix the following before continuing:",
                    items: errors.Count == 1 ? null : errors);
                return false;
            }

            return ConfirmUnmodifiedComponents();
        }

        /// <summary>
        /// Explains what happens to kept components exported with unmodified="1" (managed components never customized),
        /// and lets the user cancel. Returns true when there are none or the user continues.
        /// </summary>
        private bool ConfirmUnmodifiedComponents()
        {
            var kept = package.Components.Where(c => c.Selected).ToList();
            var unmodified = kept.Where(c => c.IsUnmodified).ToList();
            if (unmodified.Count == 0) return true;

            var allUnmodified = unmodified.Count == kept.Count;
            var items = unmodified.Take(6).Select(c => $"{c.TypeName}: {c.DisplayName}").ToList();
            if (unmodified.Count > 6) items.Add($"... and {unmodified.Count - 6} more");

            return ModernDialog.Show(this,
                allUnmodified ? DialogKind.Warning : DialogKind.Info,
                allUnmodified ? "Nothing to import" : "Some components have no content",
                (unmodified.Count == 1
                    ? "1 component you kept was exported unmodified (unmodified=\"1\")."
                    : $"{unmodified.Count} components you kept were exported unmodified (unmodified=\"1\").")
                + " " + (allUnmodified
                    ? "All the components you kept are in this case: the imported solution will be EMPTY."
                    : $"The other {kept.Count - unmodified.Count} component(s) will be imported normally."),
                items,
                new[]
                {
                    new DialogSection("What does it mean?",
                        "These components belong to another managed solution (for example a Microsoft solution) and were never customized in the source environment. "
                        + "Dataverse exported them as a simple reference, without any content: no definition, no file (no DLL, no script)."),
                    new DialogSection("What happens on import?",
                        "Dataverse has nothing to create or update for them: they will not appear in the imported solution."),
                    new DialogSection("What to do",
                        "To move a component to another environment, keep the components you created or customized "
                        + "(for example your own plug-in assembly, with its DLL and its steps)."),
                },
                primary: "Continue anyway",
                secondary: "Cancel");
        }

        #endregion

        #region Generate / import

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateSolution();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (package == null) return;

            // ExecuteMethod asks for a connection when XrmToolBox is not connected yet; ImportSolution validates the selection.
            ExecuteMethod(ImportSolution);
        }

        private void LogResult(ExtractResult result)
        {
            Log(LogLevel.Info, $"Kept {result.Kept.Count} component(s), removed {result.Removed.Count} component(s) and {result.RemovedFiles.Count} file(s).");
            foreach (var file in result.RemovedFiles) Log(LogLevel.Info, $"Removed file {file}");
            foreach (var warning in result.Warnings) Log(LogLevel.Warning, warning);
            if (result.Warnings.Count > 0) tsbShowLog.Checked = true;
        }

        private void GenerateSolution()
        {
            if (package == null) return;
            var options = GetOptions();
            if (!CanGenerate(options)) return;

            var uniqueName = string.IsNullOrWhiteSpace(options.UniqueName) ? package.UniqueName : options.UniqueName.Trim();
            var version = string.IsNullOrWhiteSpace(options.Version) ? package.Version : options.Version.Trim();

            using (var dialog = new SaveFileDialog
            {
                Title = "Save the new solution",
                Filter = "Solution zip (*.zip)|*.zip",
                FileName = $"{uniqueName}_{version.Replace('.', '_')}.zip",
                InitialDirectory = mySettings?.LastFolder ?? Path.GetDirectoryName(package.SourcePath),
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var path = dialog.FileName;
                if (string.Equals(Path.GetFullPath(path), Path.GetFullPath(package.SourcePath), StringComparison.OrdinalIgnoreCase))
                {
                    ModernDialog.Show(this, DialogKind.Warning, "Choose another file",
                        "The new solution cannot replace the source solution. Save it under a different name or in another folder.");
                    return;
                }

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Generating the new solution...",
                    Work = (worker, args) =>
                    {
                        var result = SolutionExtractor.Extract(package, options);
                        File.WriteAllBytes(path, result.ZipContent);
                        args.Result = result;
                    },
                    PostWorkCallBack = args =>
                    {
                        if (args.Error != null)
                        {
                            Log(LogLevel.Error, $"Generation failed: {args.Error.Message}");
                            ModernDialog.Show(this, DialogKind.Error, "Generation failed", args.Error.Message);
                            return;
                        }

                        var result = (ExtractResult)args.Result;
                        if (mySettings != null) mySettings.LastFolder = Path.GetDirectoryName(path);
                        LogResult(result);
                        Log(LogLevel.Success, $"Solution saved to {path}");

                        var summary = new List<string>
                        {
                            $"{result.Kept.Count} component(s) kept",
                            $"{result.Removed.Count} component(s) and {result.RemovedFiles.Count} file(s) removed",
                        };
                        if (result.Warnings.Count > 0) summary.Add($"{result.Warnings.Count} warning(s): see the Activity panel");

                        if (ModernDialog.Show(this, result.Warnings.Count > 0 ? DialogKind.Warning : DialogKind.Success, "Solution generated",
                                $"{Path.GetFileName(path)} is ready to import.",
                                summary,
                                primary: "Open folder",
                                secondary: "Close"))
                        {
                            Process.Start("explorer.exe", $"/select,\"{path}\"");
                        }
                    },
                });
            }
        }

        private void ImportSolution()
        {
            if (package == null) return;
            var options = GetOptions();
            if (!CanGenerate(options)) return;

            var environment = EnvironmentName ?? "the connected environment";
            var confirmed = ModernDialog.Show(this, DialogKind.Question, $"Import into {environment}?",
                $"The new solution ({package.Components.Count(c => c.Selected)} components) will be generated and imported into {environment}.",
                sections: new[]
                {
                    new DialogSection("Good to know", "Unmanaged customizations of these components in the target environment will be overwritten."),
                },
                primary: "Import",
                secondary: "Cancel");
            if (!confirmed) return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Generating the new solution...",
                Work = (worker, args) =>
                {
                    var outcome = new ImportOutcome { Extract = SolutionExtractor.Extract(package, options) };
                    args.Result = outcome;
                    worker.ReportProgress(0, $"Importing into {environment}...");

                    var importJobId = Guid.NewGuid();
                    var response = (ExecuteAsyncResponse)Service.Execute(new ExecuteAsyncRequest
                    {
                        Request = new ImportSolutionRequest
                        {
                            CustomizationFile = outcome.Extract.ZipContent,
                            ImportJobId = importJobId,
                            OverwriteUnmanagedCustomizations = true,
                            PublishWorkflows = true,
                        },
                    });

                    var started = DateTime.Now;
                    while (true)
                    {
                        Thread.Sleep(5000);
                        var job = Service.Retrieve("asyncoperation", response.AsyncJobId, new ColumnSet("statuscode", "message", "friendlymessage"));
                        var status = job.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? 0;
                        if (status == 30) return; // Succeeded
                        if (status == 31 || status == 32) // Failed / Canceled
                        {
                            // Reported as a result rather than an exception: an import failure is an expected outcome.
                            outcome.Error = job.GetAttributeValue<string>("friendlymessage") ?? job.GetAttributeValue<string>("message") ?? "The import failed.";
                            outcome.Failures = RetrieveImportFailures(importJobId);
                            return;
                        }
                        worker.ReportProgress(0, $"Importing into {environment}... ({(int)(DateTime.Now - started).TotalSeconds}s)");
                    }
                },
                ProgressChanged = e => SetWorkingMessage(e.UserState.ToString()),
                PostWorkCallBack = args =>
                {
                    var outcome = args.Result as ImportOutcome;
                    var error = args.Error?.Message ?? outcome?.Error;
                    if (error != null)
                    {
                        ShowImportError(environment, error, outcome?.Failures ?? new List<string>());
                        return;
                    }

                    LogResult(outcome.Extract);
                    Log(LogLevel.Success, $"Solution imported into {environment}.");
                    if (ModernDialog.Show(this, DialogKind.Success, "Import completed",
                            $"The solution was imported into {environment}.",
                            sections: new[] { new DialogSection("Next step", "Publish all customizations so that users see the changes.") },
                            primary: "Publish all",
                            secondary: "Not now"))
                    {
                        PublishAll();
                    }
                },
            });
        }

        private sealed class ImportOutcome
        {
            public ExtractResult Extract;
            public string Error;
            public List<string> Failures = new List<string>();
        }

        /// <summary>Components reported as failed by the import job; empty when the job cannot be read.</summary>
        private List<string> RetrieveImportFailures(Guid importJobId)
        {
            try
            {
                var importJob = Service.Retrieve("importjob", importJobId, new ColumnSet("data"));
                return ImportDiagnostics.ParseFailures(importJob.GetAttributeValue<string>("data"));
            }
            catch (Exception)
            {
                // The job is not created when the import is rejected before it starts (e.g. missing privilege).
                return new List<string>();
            }
        }

        private void ShowImportError(string environment, string rawError, List<string> failures)
        {
            var explanation = ImportDiagnostics.Explain(rawError, environment);

            Log(LogLevel.Error, $"Import into {environment} failed: {explanation.Replace(Environment.NewLine, " ")}");
            if (explanation != rawError) Log(LogLevel.Info, $"Dataverse error: {rawError}");
            foreach (var failure in failures) Log(LogLevel.Error, $"Failed component: {failure}");
            tsbShowLog.Checked = true;

            var items = failures.Take(8).ToList();
            if (failures.Count > 8) items.Add($"... and {failures.Count - 8} more (see the Activity panel)");

            var sections = new List<DialogSection>();
            if (explanation != rawError) sections.Add(new DialogSection("Dataverse error", rawError));

            ModernDialog.Show(this, DialogKind.Error, "Import failed",
                explanation + (failures.Count > 0 ? $"{Environment.NewLine}{Environment.NewLine}Failed components:" : string.Empty),
                items,
                sections);
        }

        private void PublishAll()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Publishing all customizations...",
                Work = (worker, args) => Service.Execute(new PublishAllXmlRequest()),
                PostWorkCallBack = args =>
                {
                    if (args.Error == null) Log(LogLevel.Success, "Customizations published.");
                    else Log(LogLevel.Error, $"Publish failed: {args.Error.Message}");
                },
            });
        }

        #endregion
    }
}
