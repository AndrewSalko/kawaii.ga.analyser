using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser.PagesReport
{
	class VisitedPagesReport
	{
		public VisitedPagesReport(VisitedPagesReportRow[] rows)
		{
			Rows = rows;
		}


		public VisitedPagesReportRow[] Rows
		{
			get;
			private set;
		}
	}
}
