using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Data;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using kawaii.ga.analyser.RevenueReport;

namespace kawaii.ga.analyser
{
	class Program
	{
		static string[] _SCOPES = { Google.Apis.AnalyticsReporting.v4.AnalyticsReportingService.ScopeConstants.AnalyticsReadonly };

		public const string ENV_VAR_JSON_SECRET_FILE = "kawaii_ga_json_secret_file";

		const double _LOW_LIMIT = 0.01;


		static void Main(string[] args)
		{
			Console.WriteLine("Google Analytics Reports and ADSense revenue analyser");

			try
			{
				DateTime startDate = new DateTime(2020, 10, 01);
				DateTime endDate = new DateTime(2020, 10, 31);

				string gaViewID = "142868091";  //View ID in Google Analytics

				string jsonAPIFile = Environment.GetEnvironmentVariable(ENV_VAR_JSON_SECRET_FILE);
				if (string.IsNullOrEmpty(jsonAPIFile))
				{
					Console.WriteLine("Environment variable not found:" + ENV_VAR_JSON_SECRET_FILE);
				}

				UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
				string startPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

				string jsonAPIAuth = Path.Combine(startPath, jsonAPIFile);

				UserCredential credential;

				using (var stream = new FileStream(jsonAPIAuth, FileMode.Open, FileAccess.Read))
				{
					//в этой подпапке будут сохраняться "подтвержденные" данные авторизации. До тех пор, пока эта вещь живая, спрашивать в браузере не будут
					//А вот если ее удалить, то запустят браузер и спросят подтверждение через gmail
					string credPath = Path.Combine(startPath, "creds.json");

					credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
						GoogleClientSecrets.Load(stream).Secrets,
						_SCOPES,
						"user",
						CancellationToken.None,
						new FileDataStore(credPath, true)).Result;
				}

				var init = new BaseClientService.Initializer
				{
					ApplicationName = "kawaiimobile-ga2",
					HttpClientInitializer = credential
				};

				//https://developers.google.com/analytics/devguides/reporting/core/v4/samples?hl=ru#c

				var service = new Google.Apis.AnalyticsReporting.v4.AnalyticsReportingService(init);

				var publisherRevenueReport = new PublisherRevenueReportBuilder(service, gaViewID);
				var revenueReport = publisherRevenueReport.Build(startDate, endDate);

				var reportRows = revenueReport.Rows;

				if (reportRows != null)
				{
					Dictionary<string, PostGroup> urlToRows = new Dictionary<string, PostGroup>();

					foreach (var item in reportRows)
					{
						if (!urlToRows.TryGetValue(item.MainPostURL, out PostGroup postGroup))
						{
							postGroup = new PostGroup(item.MainPostURL);
							urlToRows[item.MainPostURL] = postGroup;
						}

						postGroup.Add(item);
					}

					//теперь сгруппируем по доходу

					var ord = (from x in urlToRows orderby x.Value.TotalRevenue descending select x.Value).ToArray();

					string reportFileName = Path.Combine(startPath, "Revenue report full.txt");
					_SaveRevenueReport(reportFileName, ord, revenueReport.TotalRevenue, false);

					//и еще краткий отчет - где исключено все, что мало смысленно
					string reportFileNameOpt = Path.Combine(startPath, "Revenue report optimized.txt");
					_SaveRevenueReport(reportFileNameOpt, ord, revenueReport.TotalRevenue, true);

				}

				Console.WriteLine("Done");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

		}

		static void _SaveRevenueReport(string reportFileName, PostGroup[] grouped, double totalRevenue, bool excludeZeroRevenue)
		{
			using (var log = File.CreateText(reportFileName))
			{

				double accumRevenue = 0;

				foreach (var itemOrd in grouped)
				{
					if (excludeZeroRevenue && itemOrd.TotalRevenue < _LOW_LIMIT)
						continue;

					log.WriteLine("_____________________________________________");
					log.WriteLine($"{itemOrd.TotalRevenue} -  {itemOrd.MainPostURL}");

					accumRevenue += itemOrd.TotalRevenue;

					log.WriteLine();

					foreach (var urlItem in itemOrd.Rows)
					{
						if (excludeZeroRevenue && urlItem.Revenue < _LOW_LIMIT)
							continue;

						log.WriteLine($"{urlItem.Revenue}  -  {urlItem.URL}  -  Clicks: {urlItem.Clicks}  -  Impressions: {urlItem.Impressions}");
					}

					log.WriteLine();
					log.WriteLine("..accumulated revenue: {0}", accumRevenue);
				}

				log.WriteLine();
				log.WriteLine($"Total revenue: {totalRevenue}");

				log.Flush();

			}//using reportStream

		}


	}
}