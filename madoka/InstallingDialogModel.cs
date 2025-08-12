using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace madoka
{
	class InstallingDialogModel
	{
		public readonly CancellationTokenSource cancelToken = new CancellationTokenSource();
		public IFontInstallingAPI api;
		public InstallDialogActionType actionType;
		public string specialSuffix;
		public DataSet1 dataSet;
		public int[] opFontIdList;

		public volatile int completedCount;
	}
}
