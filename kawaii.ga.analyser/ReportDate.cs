using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser
{
	class ReportDate
	{
		public static string GetDateAsString(DateTime date)
		{
			return string.Format("{0:yyyy-MM-dd}", date);
		}

	}
}
