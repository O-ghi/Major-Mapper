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
    unsafe class UnityEngine_EventSystems_TriggerListener_Binding_Entry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.EventSystems.TriggerListener.Entry);
            args = new Type[]{typeof(UnityEngine.EventSystems.EventTriggerType), typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.EventTriggerType, UnityEngine.GameObject>)};
            method = type.GetMethod("AddEventListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddEventListener_0);
            args = new Type[]{};
            method = type.GetMethod("RemoveAllEventListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RemoveAllEventListener_1);

            field = type.GetField("eventID", flag);
            app.RegisterCLRFieldGetter(field, get_eventID_0);
            app.RegisterCLRFieldSetter(field, set_eventID_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_eventID_0, AssignFromStack_eventID_0);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* AddEventListener_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<UnityEngine.EventSystems.EventTriggerType, UnityEngine.GameObject> @call = (UnityEngine.Events.UnityAction<UnityEngine.EventSystems.EventTriggerType, UnityEngine.GameObject>)typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.EventTriggerType, UnityEngine.GameObject>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.EventTriggerType @eventID = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.EventSystems.TriggerListener.Entry instance_of_this_method = (UnityEngine.EventSystems.TriggerListener.Entry)typeof(UnityEngine.EventSystems.TriggerListener.Entry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddEventListener(@eventID, @call);

            return __ret;
        }

        static StackObject* RemoveAllEventListener_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.EventSystems.TriggerListener.Entry instance_of_this_method = (UnityEngine.EventSystems.TriggerListener.Entry)typeof(UnityEngine.EventSystems.TriggerListener.Entry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveAllEventListener();

            return __ret;
        }


        static object get_eventID_0(ref object o)
        {
            return ((UnityEngine.EventSystems.TriggerListener.Entry)o).eventID;
        }

        static StackObject* CopyToStack_eventID_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.EventSystems.TriggerListener.Entry)o).eventID;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_eventID_0(ref object o, object v)
        {
            ((UnityEngine.EventSystems.TriggerListener.Entry)o).eventID = (UnityEngine.EventSystems.EventTriggerType)v;
        }

        static StackObject* AssignFromStack_eventID_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.EventSystems.EventTriggerType @eventID = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            ((UnityEngine.EventSystems.TriggerListener.Entry)o).eventID = @eventID;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new UnityEngine.EventSystems.TriggerListener.Entry();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}