using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace kawaii.ga.analyser.RevenueReport
{
	class PublisherRevenueReportBuilder
	{
		AnalyticsReportingService _Service;
		string _GAViewID;


		public PublisherRevenueReportBuilder(AnalyticsReportingService service, string gaViewID)
		{
			_GAViewID = gaViewID ?? throw new ArgumentNullException(nameof(gaViewID));
			_Service = service ?? throw new ArgumentNullException(nameof(service));
		}

		public PublisherRevenueReport Build(DateTime startDate, DateTime endDate)
		{
			// Create the DateRange object.
			DateRange dateRange = new DateRange()
			{
				StartDate = ReportDate.GetDateAsString(startDate),
				EndDate = ReportDate.GetDateAsString(endDate)
			};

			// Create the Metrics object.
			//https://ga-dev-tools.appspot.com/dimensions-metrics-explorer/

			Metric publisherImpressionsMetric = new Metric { Expression = "ga:totalPublisherImpressions", Alias = "Publisher Impressions" };
			Metric publisherClicksMetric = new Metric { Expression = "ga:totalPublisherClicks", Alias = "Publisher Clicks" };
			Metric publisherRevenueMetric = new Metric { Expression = "ga:totalPublisherRevenue", Alias = "Publisher Revenue" };

			//Create the Dimensions object.
			Dimension dimensionPage = new Dimension { Name = "ga:pagePath" };

			OrderBy order = new OrderBy
			{
				FieldName = "ga:totalPublisherRevenue", //ga:totalPublisherImpressions
				SortOrder = "DESCENDING"
			};
			//https://developers.google.com/analytics/devguides/reporting/core/v4/rest/v4/reports/batchGet?hl=ru#SortOrder

			string nextPageToken = null;
			double revenueTotal = 0;

			var reportRows = new List<RevenueReportRow>();

			do
			{

				// Create the ReportRequest object.
				ReportRequest reportRequest = new ReportRequest
				{
					ViewId = _GAViewID,
					DateRanges = new List<DateRange>() { dateRange },
					Dimensions = new List<Dimension>() { dimensionPage },
					Metrics = new List<Metric>() { publisherImpressionsMetric, publisherClicksMetric, publisherRevenueMetric },
					OrderBys = new List<OrderBy> { order },
					PageToken = nextPageToken
				};

				List<ReportRequest> requests = new List<ReportRequest>();
				requests.Add(reportRequest);

				// Create the GetReportsRequest object.
				GetReportsRequest getReport = new GetReportsRequest() { ReportRequests = requests };

				// Call the batchGet method.
				GetReportsResponse response = _Service.Reports.BatchGet(getReport).Execute();

				var report = response.Reports[0];
				int rowCount = 0;
				if (report.Data.RowCount != null)
					rowCount = report.Data.RowCount.Value;

				var rows = report.Data.Rows;

				foreach (var row in rows)
				{
					string url = row.Dimensions[0];

					string valImpressions = row.Metrics[0].Values[0];
					string valClicks = row.Metrics[0].Values[1];
					string valRevenue = row.Metrics[0].Values[2];

					var rev = double.Parse(valRevenue, CultureInfo.InvariantCulture);

					int impressions = int.Parse(valImpressions);
					int clicks = int.Parse(valClicks);

					RevenueReportRow reportRow = new RevenueReportRow(rev, impressions, clicks, url);
					reportRows.Add(reportRow);

					revenueTotal += rev;

				}//foreach

				nextPageToken = report.NextPageToken;
				if (nextPageToken == null)
					break;

			} while (true);

			PublisherRevenueReport result = new PublisherRevenueReport(revenueTotal, reportRows.ToArray());
			return result;
		}

	}
}
