using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum MSG_NUMBER_TYPE
{
	MSG_NUMBER_UNKNOWN,         // 未知
	MSG_NUMBER_TRUE,            // 布尔 true
	MSG_NUMBER_FALSE,           // 布尔 false

	MSG_NUMBER_INT_0,           // 0
	MSG_NUMBER_INT_1,           // 1
	MSG_NUMBER_INT__1,          // -1

	MSG_NUMBER_INT8,            // 1字节
	MSG_NUMBER_INT16,           // 2字节
	MSG_NUMBER_INT32,           // 4字节

	MSG_NUMBER_UINT8,           // 1字节
	MSG_NUMBER_UINT16,          // 2字节
	MSG_NUMBER_UINT32,          // 4字节

	MSG_NUMBER_INT64,           // 8字节
	MSG_NUMBER_UINT64,          // 8字节

	MSG_NUMBER_FLOAT_0,         // 单精度浮点数 0
	MSG_NUMBER_FLOAT_1,         // 单精度浮点数 1
	MSG_NUMBER_FLOAT__1,        // 单精度浮点数 -1
	MSG_NUMBER_FLOAT,           // 单精度浮点数
	MSG_NUMBER_DOUBLE_0,        // 双精度浮点数 0
	MSG_NUMBER_DOUBLE_1,        // 双精度浮点数 1
	MSG_NUMBER_DOUBLE__1,       // 双精度浮点数 -1
	MSG_NUMBER_DOUBLE,          // 双精度浮点数

	MSG_NUMBER_ORIENT,          // 方向
	MSG_NUMBER_XZ,              // 坐标
	MSG_NUMBER_XYZ,             // 坐标
	MSG_NUMBER_XZO,             // 坐标方向
	MSG_NUMBER_XYZO,            // 坐标方向

	MSG_NUMBER_MAX = 0x1F,      // 不能大于31, 最大31
};

enum MSG_HURT_INDEX
{
	MSG_HURT_VAL = 7,
	MSG_HURT_PAR = 6,
	MSG_HURT_CIR = 5,
	MSG_HURT_HAS_EXE = 4,
	MSG_HURT_REVERSE = 3,
};

enum MSG_DAMAGE_TYPE
{
	MSG_DAMAGE_0 = 1,
	MSG_DAMAGE_1,
	MSG_DAMAGE_INT8,
	MSG_DAMAGE_INT16,
	MSG_DAMAGE_INT32,
	MSG_DAMAGE_UINT8,
	MSG_DAMAGE_UINT16,
};


public enum MSG_BASE_TYPE
{
	MSG_BASE_INT,
	MSG_BASE_OBJ,
	MSG_BASE_STR,
	MSG_BASE_USE,
};
public class FloatObject
{
	private float value; // Trường kiểu float
	private bool hasValue; // Trường để xác định xem giá trị có phải là null hay không
	public FloatObject()
	{
		value = float.MinValue;
		hasValue = false;
	}
	// Thuộc tính để truy xuất và gán giá trị
	public float Value
	{
		get { return value; }
		set
		{
			this.value = value;
			hasValue = true;
		}
	}

	// Phương thức để kiểm tra xem giá trị có phải là null hay không
	public bool HasValue
	{
		get { return hasValue; }
	}
	public float GetValueOrDefault()
	{
		if (hasValue)
		{
			return this.value;
		}
		return 0;
	}
}
public class LejiPoint
{
	public FloatObject x;
	public FloatObject y;
	public FloatObject z;
	public FloatObject o;
	public LejiPoint()
	{
		x = new FloatObject();
		y = new FloatObject();
		z = new FloatObject();
		o = new FloatObject();
	}
	public override bool Equals(object obj)
	{
		LejiPoint b = (LejiPoint)obj;
		return x.Value == b.x.Value && y.Value == b.y.Value && z.Value == b.z.Value && o.Value == b.o.Value;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	//public static bool operator !=(LejiPoint a, LejiPoint b)
	//{
	//	return a.x != b.x || a.y != b.y || a.z != b.z || a.o != b.o;
	//}

	//public static bool operator ==(LejiPoint a, LejiPoint b)
	//{
	//	return a.x == b.x && a.y == b.y && a.z == b.z && a.o == b.o;
	//}
	public static bool Compare(LejiPoint a, LejiPoint b)
	{
		return a.x.Value == b.x.Value && a.y.Value == b.y.Value && a.z.Value == b.z.Value && a.o.Value == b.o.Value;
	}

	public override string ToString()
	{
		if (x.HasValue)
			return string.Format("x:{0}", x.Value);
		if (y.HasValue)
			return string.Format("y:{0}", y.Value);
		if (z.HasValue)
			return string.Format("z:{0}", z.Value);
		if (o.HasValue)
			return string.Format("o:{0}", o.Value);

		if (x.HasValue && y.HasValue)
			return string.Format("x:{0} y:{0}", x.Value, y.Value);
		if (x.HasValue && z.HasValue)
			return string.Format("x:{0} z:{0}", x.Value, z.Value);
		if (x.HasValue && o.HasValue)
			return string.Format("x:{0} o:{0}", x.Value, o.Value);
		if (y.HasValue && z.HasValue)
			return string.Format("y:{0} z:{0}", y.Value, z.Value);
		if (y.HasValue && o.HasValue)
			return string.Format("y:{0} o:{0}", y.Value, o.Value);
		if (z.HasValue && o.HasValue)
			return string.Format("z:{0} o:{0}", z.Value, o.Value);

		if (x.HasValue && y.HasValue && z.HasValue)
			return string.Format("x:{0} y:{0} z:{0}", x.Value, y.Value, z.Value);
		if (x.HasValue && y.HasValue && o.HasValue)
			return string.Format("x:{0} y:{0} o:{0}", x.Value, y.Value, o.Value);
		if (x.HasValue && z.HasValue && o.HasValue)
			return string.Format("x:{0} z:{0} o:{0}", x.Value, z.Value, o.Value);
		if (y.HasValue && z.HasValue && o.HasValue)
			return string.Format("y:{0} z:{0} o:{0}", y.Value, z.Value, o.Value);

		if (x.HasValue && y.HasValue && z.HasValue && o.HasValue)
			return string.Format("x:{0:F3} y:{0:F3} z:{0:F3} o:{0}", x.Value, y.Value, z.Value, o.Value);

		return "none";
	}
}
