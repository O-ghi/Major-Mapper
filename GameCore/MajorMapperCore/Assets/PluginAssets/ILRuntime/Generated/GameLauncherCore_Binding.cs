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
    unsafe class GameLauncherCore_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::GameLauncherCore);

            field = type.GetField("ilrtApp", flag);
            app.RegisterCLRFieldGetter(field, get_ilrtApp_0);
            app.RegisterCLRFieldSetter(field, set_ilrtApp_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ilrtApp_0, AssignFromStack_ilrtApp_0);


        }



        static object get_ilrtApp_0(ref object o)
        {
            return global::GameLauncherCore.ilrtApp;
        }

        static StackObject* CopyToStack_ilrtApp_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::GameLauncherCore.ilrtApp;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ilrtApp_0(ref object o, object v)
        {
            global::GameLauncherCore.ilrtApp = (ILRuntime.Runtime.Enviorment.AppDomain)v;
        }

        static StackObject* AssignFromStack_ilrtApp_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ILRuntime.Runtime.Enviorment.AppDomain @ilrtApp = (ILRuntime.Runtime.Enviorment.AppDomain)typeof(ILRuntime.Runtime.Enviorment.AppDomain).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            global::GameLauncherCore.ilrtApp = @ilrtApp;
            return ptr_of_this_method;
        }



    }
}
