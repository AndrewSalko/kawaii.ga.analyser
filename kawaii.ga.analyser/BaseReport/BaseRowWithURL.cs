using System;
using System.Collections.Generic;
using System.Text;

namespace kawaii.ga.analyser.BaseReport
{
	public abstract class BaseRowWithURL
	{
		public string URL
		{
			get;
			protected set;
		}

		/// <summary>
		/// URL основного поста, если в URL у нас страница изображения
		/// </summary>
		public string MainPostURL
		{
			get;
			protected set;
		}

	}
}
