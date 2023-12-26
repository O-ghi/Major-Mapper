using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameConvert
{
	public static string StringConvert(object value)
	{
		if (value == null)
			return string.Empty;
		return value.ToString();
	}
	#region bool
	public static bool BoolConvert(string value, bool defaultValue = false)
	{
		if (string.IsNullOrEmpty(value))
			return defaultValue;

		if (value.Equals("1") || value.ToLower().Equals("true"))
			return true;

		return false;
	}

	public static bool BoolConvert(object value, bool defaultValue = false)
	{
		if (value == null)
			return defaultValue;

		return BoolConvert(value.ToString());
	}

	public static bool BoolConvert(int value)
	{
		if (value == 0)
			return false;
		return true;
	}
	#endregion

	#region int
	public static int IntConvert(string value, int defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
			return defaultValue;

		int relust = 0;
		int.TryParse(value, out relust);
		return relust;
	}

	public static int IntConvert(object value, int defaultValue = 0)
	{
		if (value == null)
			return defaultValue;

		if (value is string)
		{
			return IntConvert(value.ToString());
		}
		return System.Convert.ToInt32(value);
	}

	public static int IntConvert(float value)
	{
		return System.Convert.ToInt32(value);
	}

	public static int IntConvert(long value)
	{
		return System.Convert.ToInt32(value);
	}

	public static int IntConvert(double value)
	{
		return System.Convert.ToInt32(value);
	}

	public static int IntConvert(int value)
	{
		return value;
	}
	#endregion

	#region float
	public static float FloatConvert(string value, float defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
			return defaultValue;

		float relust = 0;
		float.TryParse(value, out relust);

		return relust;
	}

	public static float FloatConvert(object value, float defaultValue = 0)
	{
		if (value == null)
			return defaultValue;

		if (value is string)
		{
			return FloatConvert(value.ToString());
		}

		return System.Convert.ToSingle(value);
	}

	public static float FloatConvert(int value)
	{
		return System.Convert.ToSingle(value);
	}

	public static float FloatConvert(long value)
	{
		return System.Convert.ToSingle(value);
	}

	public static float FloatConvert(double value)
	{
		return System.Convert.ToSingle(value);
	}

	public static float FloatConvert(float value)
	{
		return value;
	}
	#endregion

	#region long
	public static long LongConvert(string value, long defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
			return defaultValue;

		long relust = 0;
		long.TryParse(value, out relust);

		return relust;
	}

	public static long LongConvert(object value, long defaultValue = 0)
	{
		if (value == null)
			return defaultValue;

		if (value is string)
		{
			return LongConvert(value.ToString());
		}

		return System.Convert.ToInt64(value);
	}

	public static long LongConvert(int value)
	{
		return System.Convert.ToInt64(value);
	}

	public static long LongConvert(long value)
	{
		return value;
	}

	public static long LongConvert(float value)
	{
		return System.Convert.ToInt64(value);
	}

	public static long LongConvert(double value)
	{
		return System.Convert.ToInt64(value);
	}
	#endregion

	#region double
	public static double DoubleConvert(string value, double defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
			return defaultValue;

		double relust = 0;
		double.TryParse(value, out relust);

		return relust;
	}

	public static double DoubleConvert(object value, double defaultValue = 0)
	{
		if (value == null)
			return defaultValue;

		if (value is string)
		{
			return DoubleConvert(value.ToString());
		}

		return System.Convert.ToDouble(value);
	}

	public static double DoubleConvert(int value)
	{
		return System.Convert.ToDouble(value);
	}

	public static double DoubleConvert(float value)
	{
		return System.Convert.ToDouble(value);
	}

	public static double DoubleConvert(long value)
	{
		return System.Convert.ToDouble(value);
	}

	public static double DoubleConvert(double value)
	{
		return value;
	}
	#endregion

	public static Vector3 Vector3Convert(string value, char split = ',')
	{
		if (string.IsNullOrEmpty(value))
			return Vector3.zero;

		return Vector3Convert(value.Split(split));
	}

	public static Vector3 Vector3Convert(string[] value)
	{
		if (value == null || value.Length != 3)
			return Vector3.zero;

		float x = 0f;
		float y = 0f;
		float z = 0f;
		for (int i = 0; i < value.Length; i++)
		{
			if (i == 0) x = FloatConvert(value[i]);
			if (i == 1) y = FloatConvert(value[i]);
			if (i == 2) z = FloatConvert(value[i]);
		}
		return new Vector3(x, y, z);
	}

	public static Color ColorConvert(string value, char split = ',')
	{
		if(string.IsNullOrEmpty(value))
		{
			return Color.white;
		}
		string[] valueAry = value.Split(split);
		if(valueAry.Length != 4)
		{
			return Color.white;
		}

		float red = GameConvert.FloatConvert(valueAry[0]);
		float green = GameConvert.FloatConvert(valueAry[1]);
		float blue = GameConvert.FloatConvert(valueAry[2]);
		float alphe = GameConvert.FloatConvert(valueAry[3]);

		return new Color(red, green, blue, alphe);
	}

	public static Color ColorConvertFor16(string str)
	{
		if (string.IsNullOrEmpty(str))
			return Color.black;

		Debug.Assert(str.ToCharArray()[0] == '#');

		byte r = byte.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r, g, b, 255);
	}

	public static List<string> StringSplit(string value, char split = ',')
	{
		List<string> list = new List<string>();
		if(string.IsNullOrEmpty(value))
		{
			return list;
		}

		string[] valueAry = value.Split(split);

		list.AddRange(valueAry);

		return list;
	}
}
