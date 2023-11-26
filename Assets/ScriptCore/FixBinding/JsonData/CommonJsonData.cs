using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryParabolaParams
{
    public float Speed = 1;
    public float VAcceleration = -10;
}

public class TrajectoryBezierParams
{
    public float Speed = 1;
    public Vector3 p0;
    public Vector3 p1 = Vector3.one;
    public Vector3 p2 = Vector3.one;
    public Vector3 p3;
}


public class TrajectoryMixParams
{
	public float Speed = 1;
	public float VAcceleration = -2;
	public float Time = 1.0f;
}

public class RequestInfo
{
    public string appversion;
    public int packversion;
    public List<RequestFileInfo> filelist;
}
public class TrajectoryLineParams
{
    public float Speed = 1;
}

public class RequestFileInfo
{
    public string name;
    public long crc;
    public long size;
}
