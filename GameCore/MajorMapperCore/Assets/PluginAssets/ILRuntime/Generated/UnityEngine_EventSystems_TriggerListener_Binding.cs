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
    unsafe class UnityEngine_EventSystems_TriggerListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.EventSystems.TriggerListener);
            args = new Type[]{};
            method = type.GetMethod("get_triggers", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_triggers_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_disabled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_disabled_1);
            args = new Type[]{typeof(UnityEngine.EventSystems.EventTriggerType), typeof(System.Action)};
            method = type.GetMethod("SetDisableAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetDisableAction_2);

            field = type.GetField("triggerArg", flag);
            app.RegisterCLRFieldGetter(field, get_triggerArg_0);
            app.RegisterCLRFieldSetter(field, set_triggerArg_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_triggerArg_0, AssignFromStack_triggerArg_0);


        }


        static StackObject* get_triggers_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.EventSystems.TriggerListener instance_of_this_method = (UnityEngine.EventSystems.TriggerListener)typeof(UnityEngine.EventSystems.TriggerListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.triggers;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_disabled_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.TriggerListener instance_of_this_method = (UnityEngine.EventSystems.TriggerListener)typeof(UnityEngine.EventSystems.TriggerListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.disabled = value;

            return __ret;
        }

        static StackObject* SetDisableAction_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @action = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.EventTriggerType @type = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.EventSystems.TriggerListener instance_of_this_method = (UnityEngine.EventSystems.TriggerListener)typeof(UnityEngine.EventSystems.TriggerListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetDisableAction(@type, @action);

            return __ret;
        }


        static object get_triggerArg_0(ref object o)
        {
            return ((UnityEngine.EventSystems.TriggerListener)o).triggerArg;
        }

        static StackObject* CopyToStack_triggerArg_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.EventSystems.TriggerListener)o).triggerArg;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_triggerArg_0(ref object o, object v)
        {
            ((UnityEngine.EventSystems.TriggerListener)o).triggerArg = (System.Collections.Generic.List<System.Object>)v;
        }

        static StackObject* AssignFromStack_triggerArg_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Object> @triggerArg = (System.Collections.Generic.List<System.Object>)typeof(System.Collections.Generic.List<System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((UnityEngine.EventSystems.TriggerListener)o).triggerArg = @triggerArg;
            return ptr_of_this_method;
        }



    }
}
