using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

namespace madoka.ctrl
{
	/*
	class DataGridViewIconTextCell : DataGridViewCell
	{
		public DataGridViewIconTextCell()
		{

		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
		}
	};
	*/

	class DirGridViewCtrl
	{
		static private readonly Padding TEXT_PADDING = new Padding { Left = 16 };

		public DataGridViewRow[] CreateRows(Dir[] dirList)
		{
			return _CreateRowsInternal(dirList).ToArray();
		}

		private IEnumerable<DataGridViewRow> _CreateRowsInternal(Dir[] dirList)
		{
			IEnumerable<DataGridViewRow> Convert(Dir d)
			{
				DataGridViewRow[] dir = { ConvertDirectory(d) };
				var fontList = d.FontFileList.AsParallel().Select(ConvertFont);

				IEnumerable<DataGridViewRow>[] both = { dir, fontList };
				return both.SelectMany((r) => r);
			}

			return dirList.SelectMany(Convert);
		}

		private DataGridViewRow ConvertFont(FileInfo font)
		{
			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell.Value = font.Name;
			cell.Style.Padding = TEXT_PADDING;

			DataGridViewRow row = new DataGridViewRow();
			row.Cells.Add(cell);
			return row;
		}

		private DataGridViewRow ConvertDirectory(Dir d)
		{
			DataGridViewIconTextCell cell = new DataGridViewIconTextCell();
			cell.Value = d.DirectoryInfo.Name;

			DataGridViewRow row = new DataGridViewRow();
			row.Cells.Add(cell);
			return row;
		}
	}
}
