using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser.RevenueReport
{
	public class PublisherRevenueReport
	{
		public PublisherRevenueReport(double totalRevenue, RevenueReportRow[] rows)
		{
			TotalRevenue = totalRevenue;
			Rows = rows;
		}

		public double TotalRevenue
		{
			get;
			private set;
		}

		public RevenueReportRow[] Rows
		{
			get;
			private set;
		}


	}
}
