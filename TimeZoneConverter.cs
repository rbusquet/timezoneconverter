using System;
using System.Collections.Generic;
using System.Xml;

namespace TZConverter
{
	public static class TimeZoneConverter
	{
		/// <summary>
		/// The XmlDocument containing the file opened.
		/// </summary>
		static XmlDocument xdTimeZoneFile;

		private static Dictionary<String, TimeZoneInfo> _TimeZoneFromOlson;
		public static Dictionary<String, TimeZoneInfo> TimeZoneFromOlson
		{
			get
			{
				if (_TimeZoneFromOlson == null)
					_TimeZoneFromOlson = new Dictionary<string, TimeZoneInfo>();
				if (_TimeZoneFromOlson.Count == 0)
					LoadTZMap();

				return _TimeZoneFromOlson;
			}
		}

		/// <summary>
		/// Loads a dictionary that maps Olson Database timezone ids to C# TimeZoneInfo objects, based on information on windowsZones file.
		/// See http://stackoverflow.com/a/9530993
		/// </summary>
		private static void LoadTZMap()
		{
			xdTimeZoneFile = new XmlDocument();
			xdTimeZoneFile.LoadXml(TZConverter.Properties.Resources.windowsZones);

			foreach (XmlElement mapZone in xdTimeZoneFile.SelectNodes("//supplementalData/windowsZones/mapTimezones/mapZone"))
			{
				string[] olsonNames = mapZone.GetAttribute("type", "").Split(' ');
				string windowsName = mapZone.GetAttribute("other", "");
				foreach (string olsonName in olsonNames)
				{
					if (!_TimeZoneFromOlson.ContainsKey(olsonName))
					{
						_TimeZoneFromOlson.Add(olsonName, TimeZoneInfo.FindSystemTimeZoneById(windowsName));
					}
				}
			}
		}

		public static DateTime ConvertToTimeZone(DateTime datetime, string olsonTimeZone)
		{
			TimeZoneInfo tz;
			if (TimeZoneConverter.TimeZoneFromOlson.TryGetValue(olsonTimeZone, out tz))
			{
				return datetime.ToUniversalTime().Add(tz.BaseUtcOffset);
			}
			else
			{
				return datetime;
			}
		}
	}
}
