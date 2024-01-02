using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class System_Net_HttpVersion_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(System.Net.HttpVersion);

            field = type.GetField("Version10", flag);
            app.RegisterCLRFieldGetter(field, get_Version10_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Version10_0, null);


        }



        static object get_Version10_0(ref object o)
        {
            return System.Net.HttpVersion.Version10;
        }

        static StackObject* CopyToStack_Version10_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = System.Net.HttpVersion.Version10;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
