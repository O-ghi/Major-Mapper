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
    unsafe class RequestFileInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::RequestFileInfo);

            field = type.GetField("size", flag);
            app.RegisterCLRFieldGetter(field, get_size_0);
            app.RegisterCLRFieldSetter(field, set_size_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_size_0, AssignFromStack_size_0);
            field = type.GetField("crc", flag);
            app.RegisterCLRFieldGetter(field, get_crc_1);
            app.RegisterCLRFieldSetter(field, set_crc_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_crc_1, AssignFromStack_crc_1);
            field = type.GetField("name", flag);
            app.RegisterCLRFieldGetter(field, get_name_2);
            app.RegisterCLRFieldSetter(field, set_name_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_name_2, AssignFromStack_name_2);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_size_0(ref object o)
        {
            return ((global::RequestFileInfo)o).size;
        }

        static StackObject* CopyToStack_size_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::RequestFileInfo)o).size;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_size_0(ref object o, object v)
        {
            ((global::RequestFileInfo)o).size = (System.Int64)v;
        }

        static StackObject* AssignFromStack_size_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @size = *(long*)&ptr_of_this_method->Value;
            ((global::RequestFileInfo)o).size = @size;
            return ptr_of_this_method;
        }

        static object get_crc_1(ref object o)
        {
            return ((global::RequestFileInfo)o).crc;
        }

        static StackObject* CopyToStack_crc_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::RequestFileInfo)o).crc;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_crc_1(ref object o, object v)
        {
            ((global::RequestFileInfo)o).crc = (System.Int64)v;
        }

        static StackObject* AssignFromStack_crc_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @crc = *(long*)&ptr_of_this_method->Value;
            ((global::RequestFileInfo)o).crc = @crc;
            return ptr_of_this_method;
        }

        static object get_name_2(ref object o)
        {
            return ((global::RequestFileInfo)o).name;
        }

        static StackObject* CopyToStack_name_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::RequestFileInfo)o).name;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_name_2(ref object o, object v)
        {
            ((global::RequestFileInfo)o).name = (System.String)v;
        }

        static StackObject* AssignFromStack_name_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::RequestFileInfo)o).name = @name;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new global::RequestFileInfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
