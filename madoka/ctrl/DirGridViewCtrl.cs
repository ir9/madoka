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

		private readonly ModelMy _model;
		private readonly DataSet1 _dataSet;

		private DirGridViewCtrl(ModelMy model, DataSet1 dataSet)
		{
			_model = model;
			_dataSet = dataSet;
		}

		public static DataGridViewRow[] GetRows(ModelMy model, DataSet1 dataSet, int[] srcDirList)
		{
			using (dataSet.GetReadLocker())
			{
				DirGridViewCtrl c = new DirGridViewCtrl(model, dataSet);
				return c._GetRows(srcDirList).ToArray();
			}
		}

		private IEnumerable<DataGridViewRow> _GetRows(int[] srcDirList)
		{
			return srcDirList.AsParallel().SelectMany(Convert);
		}

		private IEnumerable<DataGridViewRow> Convert(int d)
		{
			DataSet1.DirectoryTableRow dirRow = _dataSet.DirectoryTable.FindByid(d);
			Dir dirObj = (Dir)dirRow.directory;

			DataGridViewRow[] dir = { ConvertDirectory(dirObj) };
			var fontList = dirObj.FontFileID.AsParallel().Select(ConvertFont);

			IEnumerable<DataGridViewRow>[] both = { dir, fontList };
			return both.SelectMany((r) => r);
		}

		private DataGridViewRow ConvertFont(int fontFileId)
		{
			DataSet1.FontFileTableRow fontRow = _dataSet.FontFileTable.FindByid(fontFileId);
			string fileName = Path.GetFileName(fontRow.filepath);

			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell.Value = fileName;
			cell.Style.Padding = TEXT_PADDING;

			DataGridViewRow row = new DataGridViewRow();
			row.Cells.Add(cell);
			return row;
		}

		private DataGridViewRow ConvertDirectory(Dir d)
		{
			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell.Value = d.DirectoryInfo.Name;

			DataGridViewRow row = new DataGridViewRow();
			row.Cells.Add(cell);
			return row;
		}
	}
}
