using System;
using System.Collections.Generic;
using System.Text;
using kawaii.ga.analyser.BaseReport;

namespace kawaii.ga.analyser.RevenueReport
{
	public class RevenueReportRow: BaseRowWithURL
	{
		public RevenueReportRow(double revenue, int impressions, int clicks, string url)
		{
			Revenue = revenue;
			Impressions = impressions;
			Clicks = clicks;

			URL = url;

			MainPostURL = _GetMainPostURL(url);
		}

		string _GetMainPostURL(string url)
		{
			//от аналитики основной пост будет иметь 4 слеша "/2019/05/kaguya-sama-wa-kokurasetai/"

			int count = 0;
			int index = 0;

			for (int i = 0; i < url.Length; i++)
			{
				if (url[i] == '/')
				{
					count++;
					if (count == 4)
					{
						index = i;
						break;
					}
				}
			}//for

			if (count == url.Length)
				return url;	//этот урл сам по себе основной урл поста

			string mainURL = url.Substring(0, index + 1);
			return mainURL;
		}


		public double Revenue
		{
			get;
			private set;
		}

		public int Impressions
		{
			get;
			private set;
		}

		public int Clicks
		{
			get;
			private set;
		}

	}
}
