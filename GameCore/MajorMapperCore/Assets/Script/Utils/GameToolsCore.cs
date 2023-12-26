using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameToolsCore : MonoBehaviour
{
	public static void SetField(FieldInfo field, object obj, string strVal)
	{
		var fieldType = FieldInfoFix.FieldType(field);

		if (fieldType == typeof(System.String))
		{
			GameTools2.SetFieldInfoValueString(field, obj, strVal);
		}
		else if (fieldType == typeof(System.Int32))
		{
			//int intVal = 0;
			//System.Int32.TryParse(strVal, out intVal);
			GameTools2.SetFieldInfoValueInt(field, obj, strVal);
		}
		else if (fieldType == typeof(System.Int64))
		{
			long longVal = 0;
			System.Int64.TryParse(strVal, out longVal);
			GameTools2.SetFieldInfoValue(field, obj, longVal);
		}
		else if (fieldType == typeof(System.Single))
		{
			float floatVal = 0;
			System.Single.TryParse(strVal, out floatVal);
			GameTools2.SetFieldInfoValue(field, obj, floatVal);
		}
		else if (fieldType == typeof(System.Double))
		{
			double doubleVal = 0;
			System.Double.TryParse(strVal, out doubleVal);
			GameTools2.SetFieldInfoValue(field, obj, doubleVal);
		}
		else if (fieldType == typeof(System.UInt32))
		{
			uint uintVal = 0;
			System.UInt32.TryParse(strVal, out uintVal);
			GameTools2.SetFieldInfoValue(field, obj, uintVal);
		}
		else if (fieldType == typeof(System.UInt64))
		{
			ulong ulongVal = 0;
			System.UInt64.TryParse(strVal, out ulongVal);
			GameTools2.SetFieldInfoValue(field, obj, ulongVal);
		}
		else if (fieldType == typeof(System.Boolean))
		{
			bool boolVal = false;
			System.Boolean.TryParse(strVal, out boolVal);
			GameTools2.SetFieldInfoValue(field, obj, boolVal);
		}
		else
		{
			Debug.LogErrorFormat("Unsupported config type {0}", fieldType);
		}
	}


	/// <summary>
	/// 设置整型的某一位
	/// </summary>
	/// <param name="value">被设置的整型值</param>
	/// <param name="index">需要设置的位索引</param>
	/// <returns>被设置后的值</returns>
	public static int BitSet(int value, int index)
	{
		return value | (1 << index);
	}

	/// <summary>
	/// 清理整型的某一位
	/// </summary>
	/// <param name="value">被清理的整型值</param>
	/// <param name="index">需要清理的位索引</param>
	/// <returns>被清理后的值</returns>
	public static int BitClear(int value, int index)
	{
		return value & ~(1 << index);
	}

	/// <summary>
	/// 测试整型的某一位是否被设置
	/// </summary>
	/// <param name="value">被测试的整型值</param>
	/// <param name="index">需要测试的位索引</param>
	/// <returns>是否被设置</returns>
	public static bool BitTest(int value, int index)
	{
		return 0 != (value & (1 << index));
	}

	/// <summary>
	/// 设置整型的某一位
	/// </summary>
	/// <param name="value">被设置的整型值</param>
	/// <param name="index">需要设置的位索引</param>
	/// <returns>被设置后的值</returns>
	public static long BitSet(long value, int index)
	{
		return value | (1L << index);
	}

	/// <summary>
	/// 清理整型的某一位
	/// </summary>
	/// <param name="value">被清理的整型值</param>
	/// <param name="index">需要清理的位索引</param>
	/// <returns>被清理后的值</returns>
	public static long BitClear(long value, int index)
	{
		return value & ~(1L << index);
	}


	/// <summary>
	/// 测试整型的某一位是否被设置
	/// </summary>
	/// <param name="value">被测试的整型值</param>
	/// <param name="index">需要测试的位索引</param>
	/// <returns>是否被设置</returns>
	public static bool BitTest(long value, int index)
	{
		return 0 != (value & (1L << index));
	}
}
