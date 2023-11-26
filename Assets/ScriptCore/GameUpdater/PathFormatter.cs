using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFormatter : IFormatProvider, ICustomFormatter
{
	public object GetFormat(Type formatType)
	{
		if (formatType == typeof(ICustomFormatter))
			return this;
		else
			return null;
	}

	public string Format(string fmt, object arg, IFormatProvider formatProvider)
	{
		if ("ticks" == fmt)
			return DateTime.Now.Ticks.ToString();

		if (arg == null)
			return "";

		Dictionary<string, string> dic = arg as Dictionary<string, string>;
		if (dic == null)
			return arg.ToString();

		if (string.IsNullOrEmpty(fmt))
			return "";

		string result = "";

		if (!dic.TryGetValue(fmt, out result))
		{
			return "";
		}

		return result;
	}
}
