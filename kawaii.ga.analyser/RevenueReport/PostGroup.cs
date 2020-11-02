using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser.RevenueReport
{
	class PostGroup
	{
		public PostGroup(string mainPostURL)
		{
			Rows = new List<RevenueReportRow>();
			MainPostURL = mainPostURL;
		}

		public string MainPostURL
		{
			get;
			private set;
		}

		public void Add(RevenueReportRow row)
		{
			Rows.Add(row);
			TotalRevenue += row.Revenue;
		}

		public List<RevenueReportRow> Rows
		{
			get;
			private set;
		}

		public double TotalRevenue
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return MainPostURL;
		}

	}
}
