using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser.PagesReport
{
	class VisitedPagesReportRow: BaseReport.BaseRowWithURL
	{
		public VisitedPagesReportRow(string url, int pageViews, int uniqPageViews, int entrances)
		{
			URL = url;
			PageViews = pageViews;
			UniquePageViews = uniqPageViews;
			Entrances = entrances;
		}

		public int PageViews
		{
			get;
			private set;
		}

		public int UniquePageViews
		{
			get;
			private set;
		}

		/// <summary>
		/// Landing page (count)
		/// </summary>
		public int Entrances
		{
			get;
			private set;
		}

	}
}
