using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdapterGen
{
    public static string adpaterStr = @"using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ILRT[$ClsName]Adaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof([$ClassName]);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

	public class Adaptor : [$ClassName], CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

[$Method]

[$Property]
        
        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod(""ToString"", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}";

    [MenuItem("ILRuntime/AdapterGen")]
    public static void Gen()
    {
        //属性TODO
        //interfaceTODO，也不建议adaptinterface，而是adapt对应的类型
        //有继承关系的建议指定adapt函数
        List<Type> list = new List<Type>();
        //list.Add(typeof(AirFishLab.ScrollingList.ListBox));
        //list.Add(typeof(FairyGUI.GObject));
        //list.Add(typeof(FairyGUI.GComponent));
        list.Add(typeof(PlayerInput.IGameActions));

        foreach (var t in list)
        {
            genOne(t);
        }
    }

    private static void genOne(Type type, List<MethodInfo> methods = null, List<PropertyInfo> properties = null)
    {
        var ret = adpaterStr.Replace("[$ClsName]", type.Name);
        ret = ret.Replace("[$ClassName]", type.FullName);

        var mStr = "";
        if (methods == null)
            methods = new List<MethodInfo>(type.GetMethods());
        foreach (var me in methods)
        {
            try
            {
                if (me.IsPrivate)
                    continue;
                if (me.DeclaringType.FullName == "System.Object")
                    continue;
                if (me.Name.Contains("get_") || me.Name.Contains("set_"))
                    continue;
                mStr += genMethod(type, me);
            }
            catch 
            {
                Debug.LogError("genMethod Failed" + type);
            }
        }
        ret = ret.Replace("[$Method]", mStr);

        mStr = "";
        if (properties == null)
            properties = new List<PropertyInfo>(type.GetProperties());
        foreach(var p in properties)
        {
            if(p.CanRead || p.CanWrite)
                mStr += genProperty(type, p);
        }
        ret = ret.Replace("[$Property]", mStr);
        System.IO.File.WriteAllText(string.Format(Application.dataPath + "/PluginAssets/ILRuntime/Adapters/ILRT{0}Adaptor.cs", type.Name), ret);
    }

    private const string methodStr = @"
        IMethod mtd_[$methodName][$R];
        bool called_[$methodName][$R];
        [$visit] [$override]{0} [$methodName]({1})
        {{
            if (mtd_[$methodName][$R] == null)
                mtd_[$methodName][$R] = instance.Type.GetMethod(""[$methodName]"", {2});
            [$defRet]
            if (mtd_[$methodName][$R] != null && !called_[$methodName][$R])
            {{
                called_[$methodName][$R] = true;
                [$getRetCast]appdomain.Invoke(mtd_[$methodName][$R], instance, {3});
                called_[$methodName][$R] = false;
            }}[$virtualElse]
            [$doRet]
        }}
        ";
    private const string virtualStr = @"
            else
            {{
                [$getBaseRet]base.[$methodName]({4});
            }}";

    private static string genMethod(Type type, MethodInfo mInfo, string byName = "")
    {
        bool isVirtual = false;
        if(mInfo != null)
        {
            isVirtual = mInfo.IsVirtual;
            if (mInfo.IsFinal)
                isVirtual = false;
        }

        System.Random random = new System.Random(mInfo != null ? mInfo.GetHashCode() : byName.GetHashCode());
        var mStr = methodStr;
        var visit = "public";
        if(mInfo != null)
        {
            if (mInfo.IsFamilyAndAssembly)
                visit = "protected internal";
            else if(mInfo.IsFamily)
                visit = "protected";
            else if(mInfo.IsAssembly)
                visit = "internal";
        }

        int paramNum = mInfo != null ? mInfo.GetParameters().Length : 0;
        mStr = mStr.Replace("[$virtualElse]", isVirtual ? virtualStr : "");
        mStr = mStr.Replace("[$override]", isVirtual ? "override ": "");
        mStr = mStr.Replace("[$visit]", visit);
        mStr = mStr.Replace("[$methodName]", mInfo == null ? byName : mInfo.Name);
        mStr = mStr.Replace("[$R]", paramNum.ToString() + random.Next(0, 10));
        string returnType = mInfo == null ? "void" : mInfo.ReturnType.FullName;
        Debug.Log("returnType is: " + (returnType != null) + "  ||| mInfo " + mInfo.Name  + "  paramNum: "+ paramNum.ToString());
        if(returnType.Equals("System.Void") || returnType.Equals("void"))
        {
            mStr = mStr.Replace("[$defRet]", "");
            mStr = mStr.Replace("[$getRetCast]", "");
            mStr = mStr.Replace("[$getBaseRet]", "");
            mStr = mStr.Replace("[$doRet]", "");
            returnType = "void";
        }
        else
        {
            mStr = mStr.Replace("[$defRet]", string.Format("{0} ret = default({1});", returnType, returnType));
            mStr = mStr.Replace("[$getRetCast]", string.Format("ret = ({0})", returnType));
            mStr = mStr.Replace("[$getBaseRet]", string.Format("ret = ({0})", returnType));
            mStr = mStr.Replace("[$doRet]", "return ret;");
        }
        
        string paramsType = "";
        string paramsStr = "";
        string paramNullable = "null";
        string paramWithType = "";
        if (paramNum > 0)
        {
            paramsType = " ,";
            paramNullable = "";
            foreach (var pa in mInfo.GetParameters())
            {
                //ref out???
                paramsStr += pa.Name + ", ";
                paramNullable += pa.Name + ", ";
                paramsType += pa.ParameterType.FullName + ", ";
                paramWithType += pa.ParameterType.FullName + " " + pa.Name + ", ";
            }
            paramsStr = paramsStr.Substring(0, paramsStr.Length - 2);
            paramsType = paramsType.Substring(0, paramsType.Length - 2);
            paramNullable = paramNullable.Substring(0, paramNullable.Length - 2);
            paramWithType = paramWithType.Substring(0, paramWithType.Length - 2);
        }
        return string.Format(mStr, returnType, paramWithType, paramNum, paramNullable, paramsStr);
    }

    private const string propertyStr = @"
        Property TODO
";
    private static string genProperty(Type type, PropertyInfo property)
    {
        return "";
    }
}
