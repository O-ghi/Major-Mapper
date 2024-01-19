using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VariantType
{
	NONE,
	BOOL,
	INT8,
	INT16,
	INT32,
	INT64,
	UINT8,
	UINT16,
	UINT32,
	UINT64,
	FLOAT,
	DOUBLE,
	STRING,
	OBJECT,
	POINT,
}

public struct VariantLogic
{
	public VariantType type;
	private long n_value;
	private double d_value;
	private string str_value;
	private LejiPoint point_value;
	private object obj_value;

	public static VariantLogic None = new VariantLogic();
	//public static VariantLogic None= new VariantLogic();

	//public VariantLogic()
	//{

	//}
	public VariantLogic(bool value)
	{
		//
		type = VariantType.BOOL;
		n_value = value ? 1 : 0;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(sbyte value)
	{
		//

		type = VariantType.INT8;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(short value)
	{
		//

		type = VariantType.INT16;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(int value)
	{


		type = VariantType.INT32;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(long value)
	{


		type = VariantType.INT64;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(byte value)
	{


		type = VariantType.UINT8;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(ushort value)
	{


		type = VariantType.UINT16;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(uint value)
	{


		type = VariantType.UINT32;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(ulong value)
	{


		type = VariantType.UINT64;
		n_value = (long)value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(float value)
	{


		type = VariantType.FLOAT;
		n_value = 0;
		d_value = value;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(double value)
	{


		type = VariantType.DOUBLE;
		n_value = 0;
		d_value = value;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(string value)
	{


		type = VariantType.STRING;
		n_value = 0;
		d_value = 0.0;

		if (string.IsNullOrEmpty(value))
			str_value = string.Empty;
		else
			str_value = value;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public VariantLogic(LejiPoint value)
	{


		type = VariantType.STRING;
		n_value = 0;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = value;
		obj_value = null;
	}

	public static VariantLogic MakeForBool(bool value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.BOOL;
		v.n_value = value ? 1 : 0;
		return v;
	}

	public static VariantLogic MakeForInt8(sbyte value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT8;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForInt16(short value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT16;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForInt32(int value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT32;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForInt(int value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT32;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForInt64(long value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT64;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForLong(long value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.INT64;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForByte(byte value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.UINT8;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForUint8(byte value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.UINT8;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForUint16(ushort value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.UINT16;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForUint32(uint value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.UINT32;
		v.n_value = value;
		return v;
	}

	public static VariantLogic MakeForUint64(ulong value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.UINT64;
		v.n_value = (long)value;
		return v;
	}

	public static VariantLogic MakeForFloat(float value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.DOUBLE;
		v.d_value = value;
		return v;
	}

	public static VariantLogic MakeForDouble(double value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.DOUBLE;
		v.d_value = value;
		return v;
	}

	public static VariantLogic MakeForString(string value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.STRING;
		v.str_value = string.IsNullOrEmpty(value) ? string.Empty : value;
		return v;
	}

	public static VariantLogic MakeForPoint(LejiPoint value)
	{
		VariantLogic v = new VariantLogic();
		v.type = VariantType.POINT;
		v.point_value = value;
		return v;
	}

	public static VariantLogic MakeForObject(object obj)
	{
		VariantLogic v = new VariantLogic();

		if (obj is int)
		{
			v.type = VariantType.INT32;
			v.n_value = (int)obj;
		}
		else if (obj is uint)
		{
			v.type = VariantType.UINT32;
			v.n_value = (uint)obj;
		}
		else if (obj is byte)
		{
			v.type = VariantType.INT8;
			v.n_value = (byte)obj;
		}
		else if (obj is sbyte)
		{
			v.type = VariantType.UINT8;
			v.n_value = (sbyte)obj;
		}
		else if (obj is bool)
		{
			v.type = VariantType.BOOL;
			v.n_value = (bool)obj ? 1 : 0;
		}
		else if (obj is short)
		{
			v.type = VariantType.INT16;
			v.n_value = (short)obj;
		}
		else if (obj is ushort)
		{
			v.type = VariantType.UINT16;
			v.n_value = (ushort)obj;
		}
		else if (obj is long)
		{
			v.type = VariantType.INT64;
			v.n_value = (long)obj;
		}
		else if (obj is ulong)
		{
			v.type = VariantType.UINT64;
			v.n_value = (long)(ulong)obj;
		}
		else if (obj is float)
		{
			v.type = VariantType.FLOAT;
			v.d_value = (float)obj;
		}
		else if (obj is double)
		{
			v.type = VariantType.DOUBLE;
			v.d_value = (double)obj;
		}
		else if (obj is string)
		{
			v.type = VariantType.STRING;
			v.str_value = (string)obj;
		}
		else if (obj is LejiPoint)
		{
			v.type = VariantType.POINT;
			v.point_value = (LejiPoint)obj;
		}
		else
		{
			v.type = VariantType.OBJECT;
			v.obj_value = obj;
		}
		return v;
	}

	public bool IsEmpty()
	{
		return VariantType.NONE == type;
	}

	public bool IsNull()
	{
		return VariantType.NONE == type || (VariantType.OBJECT == type && null == obj_value);
	}

	public bool IsValid()
	{
		return VariantType.NONE != type;
	}

	public bool IsInt()
	{
		return VariantType.INT8 == type || VariantType.INT16 == type || VariantType.INT32 == type || VariantType.INT64 == type
			|| VariantType.UINT8 == type || VariantType.UINT16 == type || VariantType.UINT32 == type || VariantType.UINT64 == type;
	}

	public bool IsInt64()
	{
		return VariantType.INT64 == type || VariantType.UINT64 == type;
	}

	public bool IsInt32()
	{
		return VariantType.INT8 == type || VariantType.INT16 == type || VariantType.INT32 == type
			|| VariantType.UINT8 == type || VariantType.UINT16 == type || VariantType.UINT32 == type;
	}

	public bool IsNumber()
	{
		return VariantType.INT8 == type || VariantType.INT16 == type || VariantType.INT32 == type || VariantType.INT64 == type
			|| VariantType.UINT8 == type || VariantType.UINT16 == type || VariantType.UINT32 == type || VariantType.UINT64 == type
			|| VariantType.FLOAT == type || VariantType.DOUBLE == type;
	}

	public bool IsFloatNumber()
	{
		return VariantType.FLOAT == type || VariantType.DOUBLE == type;
	}

	public void SetBool(bool value)
	{
		type = VariantType.BOOL;
		n_value = value ? 1 : 0;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetInt8(sbyte value)
	{
		type = VariantType.INT8;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetInt16(short value)
	{
		type = VariantType.INT16;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetInt32(int value)
	{
		type = VariantType.INT32;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetInt64(long value)
	{
		type = VariantType.INT64;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetUint8(byte value)
	{
		type = VariantType.UINT8;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetUint16(ushort value)
	{
		type = VariantType.UINT16;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetUint32(uint value)
	{
		type = VariantType.UINT32;
		n_value = value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetUint64(ulong value)
	{
		type = VariantType.UINT64;
		n_value = (long)value;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetFloat(float value)
	{
		type = VariantType.DOUBLE;
		n_value = 0;
		d_value = value;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetDouble(double value)
	{
		type = VariantType.DOUBLE;
		n_value = 0;
		d_value = value;
		str_value = string.Empty;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetString(string value)
	{
		type = VariantType.STRING;
		n_value = 0;
		d_value = 0.0;

		if (string.IsNullOrEmpty(value))
			str_value = string.Empty;
		else
			str_value = value;
		point_value = new LejiPoint();
		obj_value = null;
	}

	public void SetPoint(LejiPoint value)
	{
		type = VariantType.POINT;
		n_value = 0;
		d_value = 0.0;
		str_value = string.Empty;
		point_value = value;
		obj_value = null;
	}

	public bool GetBool()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return n_value != 0;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return d_value != 0;
			case VariantType.STRING:
				return GameConvert.BoolConvert(str_value);
			default:
				return false;
		}
	}

	public double GetDouble()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return d_value;
			case VariantType.STRING:
				return GameConvert.DoubleConvert(str_value);
			default:
				return 0.0;
		}
	}

	public float GetFloat()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				// Debuger.Log(string.Format("GetFloat: {0:F3}", (float)n_value));
				return (float)d_value;
			case VariantType.STRING:
				return GameConvert.FloatConvert(str_value);
			default:
				return 0.0f;
		}
	}

	public short GetInt16()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (short)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (short)d_value;
			case VariantType.STRING:
				return (short)GameConvert.IntConvert(str_value);
			default:
				return (short)0;
		}
	}

	public int GetInt32()
	{
		//Debuger.Log("1.GetInt32 ");
		//Debuger.Log("1.GetInt32 type is: " + type);

		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (int)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (int)d_value;
			case VariantType.STRING:
				return (int)GameConvert.IntConvert(str_value);
			default:
				return (int)0;
		}
	}

	public int GetInt()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (int)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (int)d_value;
			case VariantType.STRING:
				return (int)GameConvert.IntConvert(str_value);
			default:
				return (int)0;
		}
	}

	public long GetLong()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (long)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (long)d_value;
			case VariantType.STRING:
				return (long)GameConvert.IntConvert(str_value);
			default:
				return (long)0;
		}
	}

	public long GetInt64()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (long)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (long)d_value;
			case VariantType.STRING:
				return (long)GameConvert.IntConvert(str_value);
			default:
				return (long)0;
		}
	}

	public sbyte GetInt8()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (sbyte)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (sbyte)d_value;
			case VariantType.STRING:
				return (sbyte)GameConvert.IntConvert(str_value);
			default:
				return (sbyte)0;
		}
	}

	public object GetNull()
	{
		return obj_value;
	}

	public LejiPoint GetPoint()
	{
		return point_value;
	}

	public string GetString()
	{
		return str_value;
	}

	public ushort GetUInt16()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (ushort)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (ushort)d_value;
			case VariantType.STRING:
				return (ushort)GameConvert.IntConvert(str_value);
			default:
				return (ushort)0;
		}
	}

	public uint GetUInt32()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (uint)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (uint)d_value;
			case VariantType.STRING:
				return (uint)GameConvert.IntConvert(str_value);
			default:
				return (uint)0;
		}
	}

	public ulong GetUInt64()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (ulong)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (ulong)d_value;
			case VariantType.STRING:
				return (ulong)GameConvert.IntConvert(str_value);
			default:
				return (ulong)0;
		}
	}

	public byte GetUInt8()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (byte)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (byte)d_value;
			case VariantType.STRING:
				return (byte)GameConvert.IntConvert(str_value);
			default:
				return (byte)0;
		}
	}

	public byte GetByte()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return (byte)n_value;
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return (byte)d_value;
			case VariantType.STRING:
				return (byte)GameConvert.IntConvert(str_value);
			default:
				return (byte)0;
		}
	}

	public override string ToString()
	{
		switch (type)
		{
			case VariantType.BOOL:
			case VariantType.INT8:
			case VariantType.INT16:
			case VariantType.INT32:
			case VariantType.INT64:
			case VariantType.UINT8:
			case VariantType.UINT16:
			case VariantType.UINT32:
			case VariantType.UINT64:
				return n_value.ToString();
			case VariantType.FLOAT:
			case VariantType.DOUBLE:
				return d_value.ToString();
			case VariantType.STRING:
				return str_value;
			case VariantType.POINT:
				return point_value.ToString();
			default:
				if (null != obj_value)
					return obj_value.ToString();
				else
					return string.Empty;
		}
	}

	//public static bool operator !=(VariantLogic a, VariantLogic b)
	//{
	//	return !VariantEqual(a, b);
	//}

	//public static bool operator ==(VariantLogic a, VariantLogic b)
	//{
	//	return VariantEqual(a, b);
	//}
	public static bool CompareVariant(VariantLogic a, VariantLogic b)
	{
		return VariantEqual(a, b);
	}

	static bool VariantEqual(VariantLogic a, VariantLogic b)
	{
		if (a.type == b.type)
		{
			switch (a.type)
			{
				case VariantType.BOOL:
				case VariantType.INT8:
				case VariantType.INT16:
				case VariantType.INT32:
				case VariantType.INT64:
				case VariantType.UINT8:
				case VariantType.UINT16:
				case VariantType.UINT32:
				case VariantType.UINT64:
					return a.n_value == b.n_value;
				case VariantType.FLOAT:
				case VariantType.DOUBLE:
					return a.d_value == b.d_value;
				case VariantType.STRING:
					return a.str_value == b.str_value;
				case VariantType.OBJECT:
					return a.obj_value == b.obj_value;
				case VariantType.POINT:
					return LejiPoint.Compare( a.point_value , b.point_value);
				default:
					return false;
			}
		}

		if (a.IsInt() && b.IsInt())
		{
			return a.n_value == b.n_value;
		}

		if (a.IsFloatNumber() && b.IsFloatNumber())
		{
			return a.d_value == b.d_value;
		}

		if (a.type == VariantType.BOOL && b.type == VariantType.BOOL)
		{
			return a.n_value == b.n_value;
		}

		if (a.type == VariantType.OBJECT && b.type == VariantType.OBJECT)
		{
			return a.obj_value == b.obj_value;
		}

		if (a.type == VariantType.STRING && b.type == VariantType.STRING)
		{
			return a.str_value == b.str_value;
		}

		if (a.type == VariantType.POINT && b.type == VariantType.POINT)
		{
			return LejiPoint.Compare(a.point_value, b.point_value);
		}

		return false;
	}

	public override bool Equals(object obj)
	{
		VariantLogic b = (VariantLogic)obj;

		return VariantEqual(this, b);
	}

	public override int GetHashCode()
	{
		if (type == VariantType.BOOL || IsInt32())
		{
			return (int)n_value;
		}

		return base.GetHashCode();
	}
}
