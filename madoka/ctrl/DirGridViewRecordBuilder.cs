using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class DirGridViewRecordBuilder
	{
		private readonly ModelMain _model;
		private readonly DataSet1  _dataSet;
		private readonly TreeModelCtrl _treeModelCtrl;
		private readonly Dir2FontCtrl  _fontCtrl;
		private readonly DataSet1.DirGridViewTableDataTable _table = new DataSet1.DirGridViewTableDataTable();

		public DirGridViewRecordBuilder(ModelMain model, DataSet1 dataSet)
		{
			_model         = model;
			_dataSet       = dataSet;
			_treeModelCtrl = new TreeModelCtrl(model);
			_fontCtrl      = new Dir2FontCtrl(model);
		}

		public DataSet1.DirGridViewTableDataTable Build(Dir selectedDir)
		{
			System.IO.DirectoryInfo parent = selectedDir.DirectoryInfo.Parent;
			string rootPath;
			if (parent != null)
			{
				rootPath = parent.FullName;
				rootPath = rootPath.TrimEnd(System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar;
			}
			else
			{   // parent == null
				rootPath = "::::::"; // use a dummy path
			}

			using (_dataSet.GetReadLocker())
			{
				Insert(selectedDir.ID, rootPath);
			}

			return _table;
		}

		private void Insert(int dirNodeID, string rootPath)
		{
			int[] childDirNodeIDList = _treeModelCtrl.GetChildIndexes(dirNodeID);
			int[] fontFileIDList = _fontCtrl.GetChildIndexes(dirNodeID);

			// === insert dir ===
			Dir dirObj = _dataSet.GetDirectory(dirNodeID);
			string fullPath = dirObj.DirectoryInfo.FullName;
			string dirName = fullPath;
			if (fullPath.StartsWith(rootPath))
			{
				dirName = fullPath.Substring(rootPath.Length);
			}

			_table.AddDirGridViewTableRow(
				IDIssuer.GridViewDataID,
				dirName,
				false,
				(int)GridViewDataType.DIRECTORY,
				dirNodeID
			);

			// === insert fonts ===
			foreach (int fontFileID in fontFileIDList)
			{
				fullPath = _dataSet.GetFontFilePath(fontFileID);
				string fileName = System.IO.Path.GetFileName(fullPath);

				_table.AddDirGridViewTableRow(
					IDIssuer.GridViewDataID,
					fileName,
					false,
					(int)GridViewDataType.FONT,
					fontFileID
				);
			}

			// == insert children ===
			foreach (int childDirNodeID in childDirNodeIDList)
			{
				Insert(childDirNodeID, rootPath);
			}
		}
	}
}
