using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class QuickVarList : IDisposeExtend
{
	List<VariantLogic> bufArr;

	public int Position { set; get; }
	public int Length { get {
			if (bufArr == null)
			{
				bufArr = new List<VariantLogic>();
			}
			return bufArr.Count; } }
	public int Count { get {
			if (bufArr == null)
			{
				bufArr = new List<VariantLogic>();
			}
			return bufArr.Count; } }

	private static Stack<QuickVarList> pool;
	public static int PoolCount { get { 
			if(pool == null)
			{
				pool = new Stack<QuickVarList>();
			}
			
			return pool.Count; } }


	public static QuickVarList Get()
    {
		//Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == GameEventManager.mainThreadID);
		if (pool == null)
		{
			pool = new Stack<QuickVarList>();
		}
		if (pool.Count <= 0)
		{
			return new QuickVarList();
		}

        return pool.Pop();
    }

	public bool Contains(VariantLogic var)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		return bufArr.Contains(var);
	}

	public bool IsEmpty()
	{
		return Position >= Length;
	}

	public bool IsEnd()
	{
		return Position >= Length;
	}

    public void Clear()
    {
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		Position = 0;
		bufArr.Clear();
	}

	static long PoolFreeCount = 0;
    public void DisposeExtend()
	{
		if (pool == null)
		{
			pool = new Stack<QuickVarList>();
		}
		Clear();
		if (pool.Count < 10000)
		{
            pool.Push(this);
        }
		else
		{
			++PoolFreeCount;

			if (PoolFreeCount > 10000)
			{
				Debug.LogError("QuickVarList Pool Warnning");
				PoolFreeCount = 0;
			}

			GC.SuppressFinalize(this);
		}
	}

	public VariantType GetVarType(int index)
	{
		if(bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		if (index < 0 || index >= bufArr.Count)
			return VariantType.NONE;

		return bufArr[index].type;
	}

	public VariantLogic GetVariant(int index)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		//Debuger.Log("bufArr size: " + bufArr.Count + "  || index:" + index);
		if (index < 0 || index >= bufArr.Count)
			return VariantLogic.None;
		var va = bufArr[index];
		//Debuger.Log("bufArr bufArr[index] is:" + (va != null));

		return va;
	}

	public void Remove(int index_, int count)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.RemoveRange(index_, count);
	}

	public void Remove(int index_)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.RemoveAt(index_);
	}

	public bool ReadBool(int index)
	{
		return GetVariant(index).GetBool();
	}

	public double ReadDouble(int index)
	{
		return GetVariant(index).GetDouble();
	}

	public float ReadFloat(int index)
	{
		return GetVariant(index).GetFloat();
	}

	public short ReadInt16(int index)
	{
		return GetVariant(index).GetInt16();
	}

	public int ReadInt32(int index)
	{
		return GetVariant(index).GetInt32();
	}

	public long ReadInt64(int index)
	{
		return GetVariant(index).GetInt64();
	}

	public sbyte ReadInt8(int index)
	{
		return GetVariant(index).GetInt8();
	}

	public object ReadNull(int index)
	{
		return GetVariant(index).GetNull();
	}

	public LejiPoint ReadPoint(int index)
	{
		return GetVariant(index).GetPoint();
	}

	public string ReadString(int index)
	{
		return GetVariant(index).GetString();
	}

	public ushort ReadUInt16(int index)
	{
		return GetVariant(index).GetUInt16();
	}

	public uint ReadUInt32(int index)
	{
		return GetVariant(index).GetUInt32();
	}

	public ulong ReadUInt64(int index)
	{
		return GetVariant(index).GetUInt64();
	}

	public byte ReadUInt8(int index)
	{
		return GetVariant(index).GetUInt8();
	}

	public bool GetBool(int index)
	{
		return GetVariant(index).GetBool();
	}

	public double GetDouble(int index)
	{
		return GetVariant(index).GetDouble();
	}

	public float GetFloat(int index)
	{
		return GetVariant(index).GetFloat();
	}

	public short GetInt16(int index)
	{
		return GetVariant(index).GetInt16();
	}

	public int GetInt(int index)
	{
		var va = GetVariant(index);
		//Debuger.Log("Getin is " + (va != null));
		return va.GetInt32();
	}

	public long GetLong(int index)
	{
		return GetVariant(index).GetInt64();
	}

	public sbyte GetInt8(int index)
	{
		return GetVariant(index).GetInt8();
	}

	public object GetNull(int index)
	{
		return GetVariant(index).GetNull();
	}

	public LejiPoint GetPoint(int index)
	{
		return GetVariant(index).GetPoint();
	}

	public string GetString(int index)
	{
		return GetVariant(index).GetString();
	}

	public ushort GetUInt16(int index)
	{
		return GetVariant(index).GetUInt16();
	}

	public uint GetUInt32(int index)
	{
		return GetVariant(index).GetUInt32();
	}

	public ulong GetUInt64(int index)
	{
		return GetVariant(index).GetUInt64();
	}

	public byte GetUInt8(int index)
	{
		return GetVariant(index).GetUInt8();
	}

	public object GetObject(int index)
	{
		return GetVariant(index).GetNull();
	}

	public string GetValueToString(int index)
	{
		return GetVariant(index).ToString();
	}

	public void Reset()
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Clear();
		Position = 0;
	}

	public void AddBool(bool val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForBool(val));
	}

	public void AddDouble(double val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForDouble(val));
	}

	public void AddFloat(float val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForFloat(val));
	}

	public void AddInt16(short val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt16(val));
	}

	public void AddInt32(int val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt32(val));
	}

	public void AddInt(int val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt32(val));
	}

	public void AddLong(long val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt64(val));
	}

	public void AddInt64(long val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt64(val));
	}

	public void AddInt8(sbyte val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt8(val));
	}

	public void AddNull()
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.None);
	}

	public void AddPoint(LejiPoint val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForPoint(val));
	}

	public void AddVariant(VariantLogic val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(val);
	}

	public void AddString(string val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForString(val));
	}

	public void AddObject(object val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForObject(val));
	}

	public void AddUInt16(ushort val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint16(val));
	}

	public void AddUInt32(uint val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint32(val));
	}

	public void AddUInt64(ulong val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint64(val));
	}

	public void AddByte(byte val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint8(val));
	}

	public void AddUInt8(byte val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint8(val));
	}

	public void AddVarlist(QuickVarList val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.AddRange(val.bufArr);
	}

	public void AddList<T>(IList<T> val)
		where T : class
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		for (int i = 0; i < val.Count; ++i)
		{
			bufArr.Add(VariantLogic.MakeForObject(val[i]));
		}
	}

	public void Add(bool val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForBool(val));
	}

	public void Add(double val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForDouble(val));
	}

	public void Add(float val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForFloat(val));
	}

	public void Add(short val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt16(val));
	}

	public void Add(int val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt32(val));
	}

	public void Add(long val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt64(val));
	}

	public void Add(sbyte val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForInt8(val));
	}

	public void Add(LejiPoint val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForPoint(val));
	}

	public void Add(VariantLogic val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(val);
	}

	public void Add(string val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForString(val));
	}

	public void Add(ushort val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint16(val));
	}

	public void Add(uint val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint32(val));
	}

	public void Add(ulong val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint64(val));
	}

	public void Insert(int index, VariantLogic var)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Insert(index, var);
	}

	public void Add(byte val)
	{
		if (bufArr == null)
		{
			bufArr = new List<VariantLogic>();
		}
		bufArr.Add(VariantLogic.MakeForUint8(val));
	}
	public sbyte ReadNextInt8()
	{
		return ReadInt8(Position++);
	}

	public short ReadNextInt16()
	{
		return ReadInt16(Position++);
	}

	public int ReadNextInt32()
	{
		return ReadInt32(Position++);
	}

	public long ReadNextInt64()
	{
		return ReadInt64(Position++);
	}

	public byte ReadNextUInt8()
	{
		return ReadUInt8(Position++);
	}

	public ushort ReadNextUInt16()
	{
		return ReadUInt16(Position++);
	}

	public uint ReadNextUInt32()
	{
		return ReadUInt32(Position++);
	}

	public ulong ReadNextUInt64()
	{
		return ReadUInt64(Position++);
	}

	public float ReadNextFloat()
	{
		return ReadFloat(Position++);
	}

	public double ReadNextDouble()
	{
		return ReadDouble(Position++);
	}

	public bool ReadNextBool()
	{
		return ReadBool(Position++);
	}

	public LejiPoint ReadNextPoint()
	{
		return ReadPoint(Position++);
	}

	public string ReadNextString()
	{
		return ReadString(Position++);
	}

	public object ReadNextNull()
	{
		return ReadNull(Position++);
	}

	public VariantLogic ReadNextVariant()
	{
		return GetVariant(Position++);
	}

}

