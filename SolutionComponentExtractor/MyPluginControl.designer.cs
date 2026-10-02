namespace SolutionComponentExtractor
{
    partial class MyPluginControl
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbKeepAll = new System.Windows.Forms.ToolStripButton();
            this.tsbKeepNone = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGenerate = new System.Windows.Forms.ToolStripButton();
            this.tsbImport = new System.Windows.Forms.ToolStripButton();
            this.tsbShowLog = new System.Windows.Forms.ToolStripButton();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSolutionDetails = new System.Windows.Forms.Label();
            this.lblSolutionName = new System.Windows.Forms.Label();
            this.pnlBadgeHost = new System.Windows.Forms.Panel();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlHeaderBorder = new System.Windows.Forms.Panel();
            this.pnlEmpty = new System.Windows.Forms.TableLayoutPanel();
            this.dropZone = new SolutionComponentExtractor.UI.DropZonePanel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.splitContent = new System.Windows.Forms.SplitContainer();
            this.lvTypes = new SolutionComponentExtractor.UI.ModernListView();
            this.colTypeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTypeCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblTypesTitle = new System.Windows.Forms.Label();
            this.lvComponents = new SolutionComponentExtractor.UI.ModernListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSchemaName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsComponents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiKeepSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnlySelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tssCms1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.chkKeptOnly = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.lvLog = new SolutionComponentExtractor.UI.ModernListView();
            this.colLogTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLogMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imlLog = new System.Windows.Forms.ImageList(this.components);
            this.lblLogTitle = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.tlpFooter = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblUniqueName = new System.Windows.Forms.Label();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtUniqueName = new System.Windows.Forms.TextBox();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.tlpActions = new System.Windows.Forms.TableLayoutPanel();
            this.lblSelectionSummary = new System.Windows.Forms.Label();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.pnlFooterBorder = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripMenu.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlBadgeHost.SuspendLayout();
            this.pnlEmpty.SuspendLayout();
            this.dropZone.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContent)).BeginInit();
            this.splitContent.Panel1.SuspendLayout();
            this.splitContent.Panel2.SuspendLayout();
            this.splitContent.SuspendLayout();
            this.cmsComponents.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.tlpFooter.SuspendLayout();
            this.tlpFields.SuspendLayout();
            this.tlpActions.SuspendLayout();
            this.flpButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.BackColor = System.Drawing.Color.White;
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbOpen,
            this.tssSeparator2,
            this.tsbKeepAll,
            this.tsbKeepNone,
            this.tssSeparator3,
            this.tsbGenerate,
            this.tsbImport,
            this.tsbShowLog});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.toolStripMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMenu.Size = new System.Drawing.Size(1100, 27);
            this.toolStripMenu.TabIndex = 0;
            // 
            // tsbClose
            // 
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(40, 20);
            this.tsbClose.Text = "Close";
            this.tsbClose.ToolTipText = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // tsbOpen
            // 
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(86, 20);
            this.tsbOpen.Text = "Open solution";
            this.tsbOpen.ToolTipText = "Open an exported solution (.zip) - Ctrl+O";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // tssSeparator2
            // 
            this.tssSeparator2.Name = "tssSeparator2";
            this.tssSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // tsbKeepAll
            // 
            this.tsbKeepAll.Name = "tsbKeepAll";
            this.tsbKeepAll.Size = new System.Drawing.Size(52, 20);
            this.tsbKeepAll.Text = "Keep all";
            this.tsbKeepAll.ToolTipText = "Keep every component of the solution";
            this.tsbKeepAll.Click += new System.EventHandler(this.tsbKeepAll_Click);
            // 
            // tsbKeepNone
            // 
            this.tsbKeepNone.Name = "tsbKeepNone";
            this.tsbKeepNone.Size = new System.Drawing.Size(67, 20);
            this.tsbKeepNone.Text = "Keep none";
            this.tsbKeepNone.ToolTipText = "Untick every component, then pick the ones you need";
            this.tsbKeepNone.Click += new System.EventHandler(this.tsbKeepNone_Click);
            // 
            // tssSeparator3
            // 
            this.tssSeparator3.Name = "tssSeparator3";
            this.tssSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // tsbGenerate
            // 
            this.tsbGenerate.Name = "tsbGenerate";
            this.tsbGenerate.Size = new System.Drawing.Size(104, 20);
            this.tsbGenerate.Text = "Generate solution";
            this.tsbGenerate.ToolTipText = "Save the new solution (.zip) - Ctrl+S";
            this.tsbGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tsbImport
            // 
            this.tsbImport.Name = "tsbImport";
            this.tsbImport.Size = new System.Drawing.Size(142, 20);
            this.tsbImport.Text = "Import into environment";
            this.tsbImport.ToolTipText = "Import the new solution into the connected environment";
            this.tsbImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // tsbShowLog
            // 
            this.tsbShowLog.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbShowLog.CheckOnClick = true;
            this.tsbShowLog.Name = "tsbShowLog";
            this.tsbShowLog.Size = new System.Drawing.Size(51, 20);
            this.tsbShowLog.Text = "Activity";
            this.tsbShowLog.ToolTipText = "Show or hide the activity log";
            this.tsbShowLog.CheckedChanged += new System.EventHandler(this.tsbShowLog_CheckedChanged);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblSolutionDetails);
            this.pnlHeader.Controls.Add(this.lblSolutionName);
            this.pnlHeader.Controls.Add(this.pnlBadgeHost);
            this.pnlHeader.Controls.Add(this.pnlHeaderBorder);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 27);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 12, 20, 0);
            this.pnlHeader.Size = new System.Drawing.Size(1100, 76);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblSolutionDetails
            // 
            this.lblSolutionDetails.AutoEllipsis = true;
            this.lblSolutionDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSolutionDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSolutionDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblSolutionDetails.Location = new System.Drawing.Point(20, 42);
            this.lblSolutionDetails.Name = "lblSolutionDetails";
            this.lblSolutionDetails.Size = new System.Drawing.Size(930, 22);
            this.lblSolutionDetails.TabIndex = 1;
            this.lblSolutionDetails.Text = "Open or drop an unmanaged solution to list its components.";
            // 
            // lblSolutionName
            // 
            this.lblSolutionName.AutoEllipsis = true;
            this.lblSolutionName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSolutionName.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.lblSolutionName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.lblSolutionName.Location = new System.Drawing.Point(20, 12);
            this.lblSolutionName.Name = "lblSolutionName";
            this.lblSolutionName.Size = new System.Drawing.Size(930, 30);
            this.lblSolutionName.TabIndex = 0;
            this.lblSolutionName.Text = "Solution Component Extractor";
            // 
            // pnlBadgeHost
            // 
            this.pnlBadgeHost.Controls.Add(this.lblBadge);
            this.pnlBadgeHost.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBadgeHost.Location = new System.Drawing.Point(950, 12);
            this.pnlBadgeHost.Name = "pnlBadgeHost";
            this.pnlBadgeHost.Padding = new System.Windows.Forms.Padding(0, 8, 0, 26);
            this.pnlBadgeHost.Size = new System.Drawing.Size(130, 63);
            this.pnlBadgeHost.TabIndex = 2;
            // 
            // lblBadge
            // 
            this.lblBadge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBadge.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblBadge.Location = new System.Drawing.Point(0, 8);
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.Size = new System.Drawing.Size(130, 29);
            this.lblBadge.TabIndex = 0;
            this.lblBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBadge.Visible = false;
            // 
            // pnlHeaderBorder
            // 
            this.pnlHeaderBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlHeaderBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHeaderBorder.Location = new System.Drawing.Point(20, 75);
            this.pnlHeaderBorder.Name = "pnlHeaderBorder";
            this.pnlHeaderBorder.Size = new System.Drawing.Size(1060, 1);
            this.pnlHeaderBorder.TabIndex = 3;
            // 
            // pnlEmpty
            // 
            this.pnlEmpty.AllowDrop = true;
            this.pnlEmpty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlEmpty.ColumnCount = 3;
            this.pnlEmpty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEmpty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 520F));
            this.pnlEmpty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEmpty.Controls.Add(this.dropZone, 1, 1);
            this.pnlEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEmpty.Location = new System.Drawing.Point(0, 103);
            this.pnlEmpty.Name = "pnlEmpty";
            this.pnlEmpty.RowCount = 3;
            this.pnlEmpty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEmpty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.pnlEmpty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlEmpty.Size = new System.Drawing.Size(1100, 347);
            this.pnlEmpty.TabIndex = 2;
            // 
            // dropZone
            // 
            this.dropZone.AllowDrop = true;
            this.dropZone.BackColor = System.Drawing.Color.White;
            this.dropZone.Controls.Add(this.btnBrowse);
            this.dropZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dropZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropZone.Location = new System.Drawing.Point(290, 33);
            this.dropZone.Margin = new System.Windows.Forms.Padding(0);
            this.dropZone.Name = "dropZone";
            this.dropZone.Size = new System.Drawing.Size(520, 280);
            this.dropZone.Subtitle = "or browse for an unmanaged solution exported from Dynamics 365 / Dataverse";
            this.dropZone.TabIndex = 0;
            this.dropZone.Click += new System.EventHandler(this.tsbOpen_Click);
            this.dropZone.Resize += new System.EventHandler(this.dropZone_Resize);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(108)))), ((int)(((byte)(189)))));
            this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(59)))), ((int)(((byte)(94)))));
            this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(94)))), ((int)(((byte)(163)))));
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(185, 210);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(150, 36);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.AllowDrop = true;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlContent.Controls.Add(this.splitContent);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 103);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(12);
            this.pnlContent.Size = new System.Drawing.Size(1100, 347);
            this.pnlContent.TabIndex = 3;
            this.pnlContent.Visible = false;
            // 
            // splitContent
            // 
            this.splitContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContent.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContent.Location = new System.Drawing.Point(12, 12);
            this.splitContent.Name = "splitContent";
            // 
            // splitContent.Panel1
            // 
            this.splitContent.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContent.Panel1.Controls.Add(this.lvTypes);
            this.splitContent.Panel1.Controls.Add(this.lblTypesTitle);
            this.splitContent.Panel1.Padding = new System.Windows.Forms.Padding(1);
            // 
            // splitContent.Panel2
            // 
            this.splitContent.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContent.Panel2.Controls.Add(this.lvComponents);
            this.splitContent.Panel2.Controls.Add(this.pnlSearch);
            this.splitContent.Panel2.Controls.Add(this.lblListTitle);
            this.splitContent.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.splitContent.Size = new System.Drawing.Size(1076, 323);
            this.splitContent.SplitterDistance = 290;
            this.splitContent.SplitterWidth = 12;
            this.splitContent.TabIndex = 0;
            // 
            // lvTypes
            // 
            this.lvTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvTypes.CheckBoxes = true;
            this.lvTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTypeName,
            this.colTypeCount});
            this.lvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTypes.FullRowSelect = true;
            this.lvTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvTypes.HideSelection = false;
            this.lvTypes.Location = new System.Drawing.Point(1, 41);
            this.lvTypes.MultiSelect = false;
            this.lvTypes.Name = "lvTypes";
            this.lvTypes.ShowItemToolTips = true;
            this.lvTypes.Size = new System.Drawing.Size(288, 281);
            this.lvTypes.SortOnColumnClick = false;
            this.lvTypes.TabIndex = 1;
            this.lvTypes.UseCompatibleStateImageBehavior = false;
            this.lvTypes.View = System.Windows.Forms.View.Details;
            this.lvTypes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvTypes_ItemChecked);
            this.lvTypes.SelectedIndexChanged += new System.EventHandler(this.lvTypes_SelectedIndexChanged);
            this.lvTypes.Resize += new System.EventHandler(this.lvTypes_Resize);
            // 
            // colTypeName
            // 
            this.colTypeName.Text = "Type";
            this.colTypeName.Width = 200;
            // 
            // colTypeCount
            // 
            this.colTypeCount.Text = "Kept";
            this.colTypeCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colTypeCount.Width = 70;
            // 
            // lblTypesTitle
            // 
            this.lblTypesTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTypesTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblTypesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblTypesTitle.Location = new System.Drawing.Point(1, 1);
            this.lblTypesTitle.Name = "lblTypesTitle";
            this.lblTypesTitle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblTypesTitle.Size = new System.Drawing.Size(288, 40);
            this.lblTypesTitle.TabIndex = 0;
            this.lblTypesTitle.Text = "COMPONENT TYPES";
            this.lblTypesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lvComponents
            // 
            this.lvComponents.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvComponents.CheckBoxes = true;
            this.lvComponents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSchemaName,
            this.colType,
            this.colDetails});
            this.lvComponents.ContextMenuStrip = this.cmsComponents;
            this.lvComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvComponents.FullRowSelect = true;
            this.lvComponents.HideSelection = false;
            this.lvComponents.Location = new System.Drawing.Point(1, 85);
            this.lvComponents.Name = "lvComponents";
            this.lvComponents.ShowItemToolTips = true;
            this.lvComponents.Size = new System.Drawing.Size(772, 237);
            this.lvComponents.SortOnColumnClick = true;
            this.lvComponents.TabIndex = 2;
            this.lvComponents.UseCompatibleStateImageBehavior = false;
            this.lvComponents.View = System.Windows.Forms.View.Details;
            this.lvComponents.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvComponents_ItemChecked);
            this.lvComponents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvComponents_KeyDown);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 260;
            // 
            // colSchemaName
            // 
            this.colSchemaName.Text = "Schema name / Id";
            this.colSchemaName.Width = 240;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 170;
            // 
            // colDetails
            // 
            this.colDetails.Text = "Details";
            this.colDetails.Width = 260;
            // 
            // cmsComponents
            // 
            this.cmsComponents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiKeepSelected,
            this.tsmiRemoveSelected,
            this.tsmiKeepOnlySelected,
            this.tssCms1,
            this.tsmiCopyName});
            this.cmsComponents.Name = "cmsComponents";
            this.cmsComponents.Size = new System.Drawing.Size(222, 98);
            this.cmsComponents.Opening += new System.ComponentModel.CancelEventHandler(this.cmsComponents_Opening);
            // 
            // tsmiKeepSelected
            // 
            this.tsmiKeepSelected.Name = "tsmiKeepSelected";
            this.tsmiKeepSelected.Size = new System.Drawing.Size(221, 22);
            this.tsmiKeepSelected.Text = "Keep selected rows";
            this.tsmiKeepSelected.Click += new System.EventHandler(this.tsmiKeepSelected_Click);
            // 
            // tsmiRemoveSelected
            // 
            this.tsmiRemoveSelected.Name = "tsmiRemoveSelected";
            this.tsmiRemoveSelected.Size = new System.Drawing.Size(221, 22);
            this.tsmiRemoveSelected.Text = "Remove selected rows";
            this.tsmiRemoveSelected.Click += new System.EventHandler(this.tsmiRemoveSelected_Click);
            // 
            // tsmiKeepOnlySelected
            // 
            this.tsmiKeepOnlySelected.Name = "tsmiKeepOnlySelected";
            this.tsmiKeepOnlySelected.Size = new System.Drawing.Size(221, 22);
            this.tsmiKeepOnlySelected.Text = "Keep only the selected rows";
            this.tsmiKeepOnlySelected.Click += new System.EventHandler(this.tsmiKeepOnlySelected_Click);
            // 
            // tssCms1
            // 
            this.tssCms1.Name = "tssCms1";
            this.tssCms1.Size = new System.Drawing.Size(218, 6);
            // 
            // tsmiCopyName
            // 
            this.tsmiCopyName.Name = "tsmiCopyName";
            this.tsmiCopyName.ShortcutKeyDisplayString = "Ctrl+C";
            this.tsmiCopyName.Size = new System.Drawing.Size(221, 22);
            this.tsmiCopyName.Text = "Copy schema name";
            this.tsmiCopyName.Click += new System.EventHandler(this.tsmiCopyName_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.chkKeptOnly);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(1, 41);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            this.pnlSearch.Size = new System.Drawing.Size(772, 44);
            this.pnlSearch.TabIndex = 1;
            // 
            // chkKeptOnly
            // 
            this.chkKeptOnly.AutoSize = true;
            this.chkKeptOnly.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkKeptOnly.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.chkKeptOnly.Location = new System.Drawing.Point(583, 6);
            this.chkKeptOnly.Name = "chkKeptOnly";
            this.chkKeptOnly.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.chkKeptOnly.Size = new System.Drawing.Size(179, 28);
            this.chkKeptOnly.TabIndex = 1;
            this.chkKeptOnly.Text = "Only components to keep";
            this.chkKeptOnly.UseVisualStyleBackColor = true;
            this.chkKeptOnly.CheckedChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.Location = new System.Drawing.Point(10, 6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(752, 25);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // lblListTitle
            // 
            this.lblListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblListTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblListTitle.Location = new System.Drawing.Point(1, 1);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblListTitle.Size = new System.Drawing.Size(772, 40);
            this.lblListTitle.TabIndex = 0;
            this.lblListTitle.Text = "ALL COMPONENTS";
            this.lblListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlLog
            // 
            this.pnlLog.BackColor = System.Drawing.Color.White;
            this.pnlLog.Controls.Add(this.lvLog);
            this.pnlLog.Controls.Add(this.lblLogTitle);
            this.pnlLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLog.Location = new System.Drawing.Point(0, 450);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Padding = new System.Windows.Forms.Padding(12, 0, 12, 6);
            this.pnlLog.Size = new System.Drawing.Size(1100, 150);
            this.pnlLog.TabIndex = 4;
            this.pnlLog.Visible = false;
            // 
            // lvLog
            // 
            this.lvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogTime,
            this.colLogMessage});
            this.lvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLog.FullRowSelect = true;
            this.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvLog.HideSelection = false;
            this.lvLog.Location = new System.Drawing.Point(12, 32);
            this.lvLog.Name = "lvLog";
            this.lvLog.ShowItemToolTips = true;
            this.lvLog.Size = new System.Drawing.Size(1076, 112);
            this.lvLog.SmallImageList = this.imlLog;
            this.lvLog.SortOnColumnClick = false;
            this.lvLog.TabIndex = 1;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // colLogTime
            // 
            this.colLogTime.Text = "Time";
            this.colLogTime.Width = 90;
            // 
            // colLogMessage
            // 
            this.colLogMessage.Text = "Message";
            this.colLogMessage.Width = 960;
            // 
            // imlLog
            // 
            this.imlLog.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imlLog.ImageSize = new System.Drawing.Size(16, 16);
            this.imlLog.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lblLogTitle
            // 
            this.lblLogTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblLogTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblLogTitle.Location = new System.Drawing.Point(12, 0);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(1076, 32);
            this.lblLogTitle.TabIndex = 0;
            this.lblLogTitle.Text = "ACTIVITY";
            this.lblLogTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.White;
            this.pnlFooter.Controls.Add(this.tlpFooter);
            this.pnlFooter.Controls.Add(this.pnlFooterBorder);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 600);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1100, 92);
            this.pnlFooter.TabIndex = 5;
            // 
            // tlpFooter
            // 
            this.tlpFooter.ColumnCount = 2;
            this.tlpFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tlpFooter.Controls.Add(this.tlpFields, 0, 0);
            this.tlpFooter.Controls.Add(this.tlpActions, 1, 0);
            this.tlpFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFooter.Location = new System.Drawing.Point(0, 1);
            this.tlpFooter.Name = "tlpFooter";
            this.tlpFooter.Padding = new System.Windows.Forms.Padding(14, 10, 14, 10);
            this.tlpFooter.RowCount = 1;
            this.tlpFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFooter.Size = new System.Drawing.Size(1100, 91);
            this.tlpFooter.TabIndex = 0;
            // 
            // tlpFields
            // 
            this.tlpFields.ColumnCount = 3;
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tlpFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpFields.Controls.Add(this.lblUniqueName, 0, 0);
            this.tlpFields.Controls.Add(this.lblDisplayName, 1, 0);
            this.tlpFields.Controls.Add(this.lblVersion, 2, 0);
            this.tlpFields.Controls.Add(this.txtUniqueName, 0, 1);
            this.tlpFields.Controls.Add(this.txtDisplayName, 1, 1);
            this.tlpFields.Controls.Add(this.txtVersion, 2, 1);
            this.tlpFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFields.Location = new System.Drawing.Point(14, 10);
            this.tlpFields.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFields.Name = "tlpFields";
            this.tlpFields.RowCount = 2;
            this.tlpFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFields.Size = new System.Drawing.Size(622, 71);
            this.tlpFields.TabIndex = 0;
            // 
            // lblUniqueName
            // 
            this.lblUniqueName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUniqueName.AutoSize = true;
            this.lblUniqueName.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblUniqueName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblUniqueName.Location = new System.Drawing.Point(3, 13);
            this.lblUniqueName.Name = "lblUniqueName";
            this.lblUniqueName.Size = new System.Drawing.Size(165, 13);
            this.lblUniqueName.TabIndex = 0;
            this.lblUniqueName.Text = "NEW SOLUTION UNIQUE NAME";
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblDisplayName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblDisplayName.Location = new System.Drawing.Point(239, 13);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(81, 13);
            this.lblDisplayName.TabIndex = 1;
            this.lblDisplayName.Text = "DISPLAY NAME";
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblVersion.Location = new System.Drawing.Point(500, 13);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "VERSION";
            // 
            // txtUniqueName
            // 
            this.txtUniqueName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtUniqueName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUniqueName.Location = new System.Drawing.Point(3, 30);
            this.txtUniqueName.Margin = new System.Windows.Forms.Padding(3, 4, 22, 3);
            this.txtUniqueName.Name = "txtUniqueName";
            this.txtUniqueName.Size = new System.Drawing.Size(211, 25);
            this.txtUniqueName.TabIndex = 3;
            this.toolTip.SetToolTip(this.txtUniqueName, "Letters, digits and underscores only");
            this.txtUniqueName.TextChanged += new System.EventHandler(this.Fields_TextChanged);
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtDisplayName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDisplayName.Location = new System.Drawing.Point(239, 30);
            this.txtDisplayName.Margin = new System.Windows.Forms.Padding(3, 4, 22, 3);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(236, 25);
            this.txtDisplayName.TabIndex = 4;
            this.txtDisplayName.TextChanged += new System.EventHandler(this.Fields_TextChanged);
            // 
            // txtVersion
            // 
            this.txtVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtVersion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtVersion.Location = new System.Drawing.Point(500, 30);
            this.txtVersion.Margin = new System.Windows.Forms.Padding(3, 4, 22, 3);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(100, 25);
            this.txtVersion.TabIndex = 5;
            this.toolTip.SetToolTip(this.txtVersion, "Format: major.minor[.build[.revision]], e.g. 1.0.0.0");
            this.txtVersion.TextChanged += new System.EventHandler(this.Fields_TextChanged);
            // 
            // tlpActions
            // 
            this.tlpActions.ColumnCount = 1;
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.Controls.Add(this.lblSelectionSummary, 0, 0);
            this.tlpActions.Controls.Add(this.flpButtons, 0, 1);
            this.tlpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActions.Location = new System.Drawing.Point(636, 10);
            this.tlpActions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpActions.Name = "tlpActions";
            this.tlpActions.RowCount = 2;
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.Size = new System.Drawing.Size(450, 71);
            this.tlpActions.TabIndex = 1;
            // 
            // lblSelectionSummary
            // 
            this.lblSelectionSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectionSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(97)))), ((int)(((byte)(97)))));
            this.lblSelectionSummary.Location = new System.Drawing.Point(3, 0);
            this.lblSelectionSummary.Name = "lblSelectionSummary";
            this.lblSelectionSummary.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.lblSelectionSummary.Size = new System.Drawing.Size(444, 26);
            this.lblSelectionSummary.TabIndex = 0;
            this.lblSelectionSummary.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnGenerate);
            this.flpButtons.Controls.Add(this.btnImport);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 26);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(450, 45);
            this.flpButtons.TabIndex = 1;
            this.flpButtons.WrapContents = false;
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(108)))), ((int)(((byte)(189)))));
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(59)))), ((int)(((byte)(94)))));
            this.btnGenerate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(94)))), ((int)(((byte)(163)))));
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerate.Location = new System.Drawing.Point(256, 6);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnGenerate.Size = new System.Drawing.Size(190, 38);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate solution";
            this.btnGenerate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.btnImport.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnImport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.btnImport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(19, 6);
            this.btnImport.Margin = new System.Windows.Forms.Padding(8, 6, 4, 0);
            this.btnImport.Name = "btnImport";
            this.btnImport.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnImport.Size = new System.Drawing.Size(225, 38);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import into environment";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // pnlFooterBorder
            // 
            this.pnlFooterBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlFooterBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFooterBorder.Location = new System.Drawing.Point(0, 0);
            this.pnlFooterBorder.Name = "pnlFooterBorder";
            this.pnlFooterBorder.Size = new System.Drawing.Size(1100, 1);
            this.pnlFooterBorder.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // MyPluginControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlEmpty);
            this.Controls.Add(this.pnlLog);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.toolStripMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1100, 692);
            this.OnCloseTool += new System.EventHandler(this.MyPluginControl_OnCloseTool);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlBadgeHost.ResumeLayout(false);
            this.pnlEmpty.ResumeLayout(false);
            this.dropZone.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.splitContent.Panel1.ResumeLayout(false);
            this.splitContent.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContent)).EndInit();
            this.splitContent.ResumeLayout(false);
            this.cmsComponents.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.tlpFooter.ResumeLayout(false);
            this.tlpFields.ResumeLayout(false);
            this.tlpFields.PerformLayout();
            this.tlpActions.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripSeparator tssSeparator2;
        private System.Windows.Forms.ToolStripButton tsbKeepAll;
        private System.Windows.Forms.ToolStripButton tsbKeepNone;
        private System.Windows.Forms.ToolStripSeparator tssSeparator3;
        private System.Windows.Forms.ToolStripButton tsbGenerate;
        private System.Windows.Forms.ToolStripButton tsbImport;
        private System.Windows.Forms.ToolStripButton tsbShowLog;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSolutionDetails;
        private System.Windows.Forms.Label lblSolutionName;
        private System.Windows.Forms.Panel pnlBadgeHost;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.Panel pnlHeaderBorder;
        private System.Windows.Forms.TableLayoutPanel pnlEmpty;
        private UI.DropZonePanel dropZone;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.SplitContainer splitContent;
        private UI.ModernListView lvTypes;
        private System.Windows.Forms.ColumnHeader colTypeName;
        private System.Windows.Forms.ColumnHeader colTypeCount;
        private System.Windows.Forms.Label lblTypesTitle;
        private UI.ModernListView lvComponents;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSchemaName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colDetails;
        private System.Windows.Forms.ContextMenuStrip cmsComponents;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepSelected;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveSelected;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnlySelected;
        private System.Windows.Forms.ToolStripSeparator tssCms1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyName;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.CheckBox chkKeptOnly;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.Panel pnlLog;
        private UI.ModernListView lvLog;
        private System.Windows.Forms.ColumnHeader colLogTime;
        private System.Windows.Forms.ColumnHeader colLogMessage;
        private System.Windows.Forms.ImageList imlLog;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.TableLayoutPanel tlpFooter;
        private System.Windows.Forms.TableLayoutPanel tlpFields;
        private System.Windows.Forms.Label lblUniqueName;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtUniqueName;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TableLayoutPanel tlpActions;
        private System.Windows.Forms.Label lblSelectionSummary;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Panel pnlFooterBorder;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
