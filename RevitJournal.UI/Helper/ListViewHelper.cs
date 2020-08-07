using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevitJournalUI.Helper
{
    public static class ListViewHelper
    {
        public const double ReduceFactor = 0.95;

        public static void SizeColumns(ListView listView)
        {
            if (listView is null || !(listView.View is GridView gridView)) { return; }

            var columns = gridView.Columns;
            var actualViewWidth = listView.ActualWidth;
            var currentWidth = new double[columns.Count];
            var percentage = new double[columns.Count];
            var summeWidth = 0.0;
            var summeWidthAfter = 0.0;

            for (int idx = 0; idx < columns.Count; idx++)
            {
                var column = columns[idx];
                var width = column.ActualWidth;
                currentWidth[idx] = width;
                summeWidth += width;
            }

            for (int idx = 0; idx < currentWidth.Length; idx++)
            {
                percentage[idx] = currentWidth[idx] * 100 / summeWidth;
            }

            var reducedActualWidth = actualViewWidth * ReduceFactor;
            for (int idx = 0; idx < gridView.Columns.Count; idx++)
            {
                var column = columns[idx];
                var width = reducedActualWidth * percentage[idx] / 100;
                summeWidthAfter += width;
                column.Width = width;
            }
        }
    }
}
