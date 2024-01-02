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
    unsafe class XmlNodeUtils_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::XmlNodeUtils);
            args = new Type[]{typeof(System.Xml.XmlNodeList), typeof(System.Int32)};
            method = type.GetMethod("GetIndexXmlNodeList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetIndexXmlNodeList_0);
            args = new Type[]{typeof(System.Xml.XmlAttributeCollection), typeof(System.Int32)};
            method = type.GetMethod("GetIndexXmlAttributeCollection", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetIndexXmlAttributeCollection_1);
            args = new Type[]{typeof(System.Xml.XmlAttributeCollection)};
            method = type.GetMethod("GetCountXmlNamedNodeMap", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCountXmlNamedNodeMap_2);
            args = new Type[]{typeof(System.Xml.XmlNodeList)};
            method = type.GetMethod("GetCountXmlNodeList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCountXmlNodeList_3);


        }


        static StackObject* GetIndexXmlNodeList_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @i = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Xml.XmlNodeList @xml = (System.Xml.XmlNodeList)typeof(System.Xml.XmlNodeList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::XmlNodeUtils.GetIndexXmlNodeList(@xml, @i);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetIndexXmlAttributeCollection_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @i = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Xml.XmlAttributeCollection @xml = (System.Xml.XmlAttributeCollection)typeof(System.Xml.XmlAttributeCollection).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::XmlNodeUtils.GetIndexXmlAttributeCollection(@xml, @i);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetCountXmlNamedNodeMap_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Xml.XmlAttributeCollection @xml = (System.Xml.XmlAttributeCollection)typeof(System.Xml.XmlAttributeCollection).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::XmlNodeUtils.GetCountXmlNamedNodeMap(@xml);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetCountXmlNodeList_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Xml.XmlNodeList @xml = (System.Xml.XmlNodeList)typeof(System.Xml.XmlNodeList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::XmlNodeUtils.GetCountXmlNodeList(@xml);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
