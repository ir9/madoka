using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class TreeModelCtrl : AbtractRelationCtrl
	{
		public TreeModelCtrl(ModelMain model)
			: base(model.treeRelationModel, model.treeRelationModelVersion)
		{

		}
	}
}
