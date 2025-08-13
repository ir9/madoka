using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class FontTagRelationCtrl
	{
		class Font2TagRelationCtrl : AbtractRelationCtrl
		{
			public Font2TagRelationCtrl(ModelMain model)
				: base(model.font2TagModel, model.font2TagModelVersion)
			{

			}
		}

		private readonly ModelMain _model;
		private readonly DataSet1  _dataSet;
		private readonly Font2TagRelationCtrl _f2tCtrl;

		public FontTagRelationCtrl(ModelMain model, DataSet1 dataSet)
		{
			_model   = model;
			_dataSet = dataSet;

			_f2tCtrl = new Font2TagRelationCtrl(model);
		}

		public int[] GetTagIDs(int fontID)
		{
			return _f2tCtrl.GetChildIndexes(fontID);
		}

		public void AddRelation(Tag tag, IEnumerable<int> joinFontIdList)
		{
			_f2tCtrl.AddRelation(joinFontIdList.Select((id) => new RelationPair(id, tag.ID)));
			tag.FontIdList.UnionWith(joinFontIdList);
		}

		public void RemoveRelation(Tag tag, IEnumerable<int> removeFontIdList)
		{
			Font2TagRelationCtrl f2tCtrl = _f2tCtrl;
			foreach (int fontId in removeFontIdList)
			{
				f2tCtrl.RemoveNode(fontId);
			}
			tag.FontIdList.ExceptWith(removeFontIdList);
		}
	}
}
