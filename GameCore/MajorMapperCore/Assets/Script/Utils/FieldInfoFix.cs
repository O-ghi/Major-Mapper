using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FieldInfoFix
{
    public static Type FieldType(FieldInfo feild)
    {
        return feild.FieldType;
    }
    public static bool IsFieldTypeNull(FieldInfo feild)
    {
        return feild == null;
    }
}
