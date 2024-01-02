using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ILRTPlayerInputAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(PlayerInput);
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

	public class Adaptor : PlayerInput, CrossBindingAdaptorType
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


        IMethod mtd_Dispose03;
        bool called_Dispose03;
        public void Dispose()
        {
            if (mtd_Dispose03 == null)
                mtd_Dispose03 = instance.Type.GetMethod("Dispose", 0);
            
            if (mtd_Dispose03 != null && !called_Dispose03)
            {
                called_Dispose03 = true;
                appdomain.Invoke(mtd_Dispose03, instance, null);
                called_Dispose03 = false;
            }
            
        }
        
        IMethod mtd_Contains14;
        bool called_Contains14;
        public System.Boolean Contains(UnityEngine.InputSystem.InputAction action)
        {
            if (mtd_Contains14 == null)
                mtd_Contains14 = instance.Type.GetMethod("Contains", 1);
            System.Boolean ret = default(System.Boolean);
            if (mtd_Contains14 != null && !called_Contains14)
            {
                called_Contains14 = true;
                ret = (System.Boolean)appdomain.Invoke(mtd_Contains14, instance, action);
                called_Contains14 = false;
            }
            return ret;
        }

        IMethod mtd_GetEnumerator04;
        bool called_GetEnumerator04;

        public System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction> GetEnumerator()
        {
            if (mtd_GetEnumerator04 == null)
            {
                // Assuming instance.Type represents a type that has the GetEnumerator method
                mtd_GetEnumerator04 = instance.Type.GetMethod("GetEnumerator", 0);
            }

            System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction> ret = null;

            if (mtd_GetEnumerator04 != null && !called_GetEnumerator04)
            {
                called_GetEnumerator04 = true;
                // Assuming 'appdomain' is the domain where this method is supposed to be invoked
                ret = (System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction>)appdomain.Invoke(mtd_GetEnumerator04, instance, null);
                called_GetEnumerator04 = false;
            }

            return ret;
        }


        IMethod mtd_Enable09;
        bool called_Enable09;
        public void Enable()
        {
            if (mtd_Enable09 == null)
                mtd_Enable09 = instance.Type.GetMethod("Enable", 0);
            
            if (mtd_Enable09 != null && !called_Enable09)
            {
                called_Enable09 = true;
                appdomain.Invoke(mtd_Enable09, instance, null);
                called_Enable09 = false;
            }
            
        }
        
        IMethod mtd_Disable01;
        bool called_Disable01;
        public void Disable()
        {
            if (mtd_Disable01 == null)
                mtd_Disable01 = instance.Type.GetMethod("Disable", 0);
            
            if (mtd_Disable01 != null && !called_Disable01)
            {
                called_Disable01 = true;
                appdomain.Invoke(mtd_Disable01, instance, null);
                called_Disable01 = false;
            }
            
        }
        
        IMethod mtd_FindAction22;
        bool called_FindAction22;
        public UnityEngine.InputSystem.InputAction FindAction(System.String actionNameOrId, System.Boolean throwIfNotFound)
        {
            if (mtd_FindAction22 == null)
                mtd_FindAction22 = instance.Type.GetMethod("FindAction", 2);
            UnityEngine.InputSystem.InputAction ret = default(UnityEngine.InputSystem.InputAction);
            if (mtd_FindAction22 != null && !called_FindAction22)
            {
                called_FindAction22 = true;
                ret = (UnityEngine.InputSystem.InputAction)appdomain.Invoke(mtd_FindAction22, instance, actionNameOrId, throwIfNotFound);
                called_FindAction22 = false;
            }
            return ret;
        }
        
        IMethod mtd_FindBinding27;
        bool called_FindBinding27;
        public System.Int32 FindBinding(UnityEngine.InputSystem.InputBinding bindingMask, UnityEngine.InputSystem.InputAction action)
        {
            if (mtd_FindBinding27 == null)
                mtd_FindBinding27 = instance.Type.GetMethod("FindBinding", 2);
            System.Int32 ret = default(System.Int32);
            if (mtd_FindBinding27 != null && !called_FindBinding27)
            {
                called_FindBinding27 = true;
                ret = (System.Int32)appdomain.Invoke(mtd_FindBinding27, instance, bindingMask, action);
                called_FindBinding27 = false;
            }
            return ret;
        }
        


        
        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}