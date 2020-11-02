using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace kawaii.ga.analyser.PagesReport
{
	class VisitedPagesReportBuilder
	{
		AnalyticsReportingService _Service;
		string _GAViewID;


		public VisitedPagesReportBuilder(AnalyticsReportingService service, string gaViewID)
		{
			_GAViewID = gaViewID ?? throw new ArgumentNullException(nameof(gaViewID));
			_Service = service ?? throw new ArgumentNullException(nameof(service));
		}

		public VisitedPagesReport Build(DateTime startDate, DateTime endDate)
		{
			// Create the DateRange object.
			DateRange dateRange = new DateRange()
			{
				StartDate = ReportDate.GetDateAsString(startDate),
				EndDate = ReportDate.GetDateAsString(endDate)
			};

			// Create the Metrics object.
			//https://ga-dev-tools.appspot.com/dimensions-metrics-explorer/

			Metric pageViewsMetric = new Metric { Expression = "ga:pageviews", Alias = "Page views" };
			Metric uniquePageViewsMetric = new Metric { Expression = "ga:uniquePageviews", Alias = "Unique page views" };
			Metric entrancesMetric = new Metric { Expression = "ga:entrances", Alias = "Entrances" };

			//Create the Dimensions object.
			Dimension dimensionPage = new Dimension { Name = "ga:pagePath" };

			OrderBy order = new OrderBy
			{
				FieldName = "ga:pageviews",
				SortOrder = "DESCENDING"
			};

			string nextPageToken = null;

			var reportRows = new List<VisitedPagesReportRow>();

			do
			{

				// Create the ReportRequest object.
				ReportRequest reportRequest = new ReportRequest
				{
					ViewId = _GAViewID,
					DateRanges = new List<DateRange>() { dateRange },
					Dimensions = new List<Dimension>() { dimensionPage },
					Metrics = new List<Metric>() { pageViewsMetric, uniquePageViewsMetric, entrancesMetric },
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
				
				var rows = report.Data.Rows;

				foreach (var row in rows)
				{
					string url = row.Dimensions[0];

					string valPageViews = row.Metrics[0].Values[0];
					string valUniqPageViews = row.Metrics[0].Values[1];
					string valEntrances = row.Metrics[0].Values[2];

					int pageViews = int.Parse(valPageViews);
					int uniqPageViews = int.Parse(valUniqPageViews);
					int entrances = int.Parse(valEntrances);

					var reportRow = new VisitedPagesReportRow(url, pageViews, uniqPageViews, entrances);
					reportRows.Add(reportRow);

				}//foreach

				nextPageToken = report.NextPageToken;
				if (nextPageToken == null)
					break;

			} while (true);

			var result = new VisitedPagesReport(reportRows.ToArray());
			return result;
		}


	}
}
