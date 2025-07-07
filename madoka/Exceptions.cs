using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	class TableVersionMismatchException : Exception
	{
		public TableVersionMismatchException(int current, int except)
			: base($"current: {current} / except: {except}")
		{
			Current = current;
			Except = except;
		}

		public int Current { get; }
		public int Except { get; }
	}
}
