using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SolutionComponentExtractor.UI
{
    /// <summary>ListView with the Explorer visual style, no flicker and click-to-sort columns.</summary>
    public class ModernListView : ListView
    {
        private int sortColumn = -1;
        private SortOrder sortOrder = SortOrder.None;

        public ModernListView()
        {
            DoubleBuffered = true;
            View = View.Details;
            FullRowSelect = true;
        }

        /// <summary>Allows sorting by clicking a column header.</summary>
        public bool SortOnColumnClick { get; set; } = true;

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(Handle, "Explorer", null);
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);
            if (!SortOnColumnClick) return;

            sortOrder = e.Column == sortColumn && sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            sortColumn = e.Column;
            ApplySort();
        }

        /// <summary>Re-applies the current sort after the items were reloaded.</summary>
        public void ApplySort()
        {
            if (sortColumn < 0) return;
            ListViewItemSorter = new Comparer(sortColumn, sortOrder);
            Sort();
        }

        private sealed class Comparer : IComparer
        {
            private readonly int column;
            private readonly int direction;

            public Comparer(int column, SortOrder order)
            {
                this.column = column;
                direction = order == SortOrder.Descending ? -1 : 1;
            }

            public int Compare(object x, object y)
            {
                var a = (ListViewItem)x;
                var b = (ListViewItem)y;
                var textA = column < a.SubItems.Count ? a.SubItems[column].Text : string.Empty;
                var textB = column < b.SubItems.Count ? b.SubItems[column].Text : string.Empty;
                return direction * string.Compare(textA, textB, StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}
