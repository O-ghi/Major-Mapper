using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ILRTIInputActionCollection2ExtendAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(IInputActionCollection2Extend);
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

	public class Adaptor : IInputActionCollection2Extend, CrossBindingAdaptorType
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


        IMethod mtd_Contains16;
        bool called_Contains16;
        public System.Boolean Contains(UnityEngine.InputSystem.InputAction action)
        {
            if (mtd_Contains16 == null)
                mtd_Contains16 = instance.Type.GetMethod("Contains", 1);
            System.Boolean ret = default(System.Boolean);
            if (mtd_Contains16 != null && !called_Contains16)
            {
                called_Contains16 = true;
                ret = (System.Boolean)appdomain.Invoke(mtd_Contains16, instance, action);
                called_Contains16 = false;
            }
            return ret;
        }
        
        IMethod mtd_Disable06;
        bool called_Disable06;
        public void Disable()
        {
            if (mtd_Disable06 == null)
                mtd_Disable06 = instance.Type.GetMethod("Disable", 0);
            
            if (mtd_Disable06 != null && !called_Disable06)
            {
                called_Disable06 = true;
                appdomain.Invoke(mtd_Disable06, instance, null);
                called_Disable06 = false;
            }
            
        }
        
        IMethod mtd_Enable03;
        bool called_Enable03;
        public void Enable()
        {
            if (mtd_Enable03 == null)
                mtd_Enable03 = instance.Type.GetMethod("Enable", 0);
            
            if (mtd_Enable03 != null && !called_Enable03)
            {
                called_Enable03 = true;
                appdomain.Invoke(mtd_Enable03, instance, null);
                called_Enable03 = false;
            }
            
        }
        
        IMethod mtd_FindAction25;
        bool called_FindAction25;
        public UnityEngine.InputSystem.InputAction FindAction(System.String actionNameOrId, System.Boolean throwIfNotFound)
        {
            if (mtd_FindAction25 == null)
                mtd_FindAction25 = instance.Type.GetMethod("FindAction", 2);
            UnityEngine.InputSystem.InputAction ret = default(UnityEngine.InputSystem.InputAction);
            if (mtd_FindAction25 != null && !called_FindAction25)
            {
                called_FindAction25 = true;
                ret = (UnityEngine.InputSystem.InputAction)appdomain.Invoke(mtd_FindAction25, instance, actionNameOrId, throwIfNotFound);
                called_FindAction25 = false;
            }
            return ret;
        }
        
        IMethod mtd_FindBinding23;
        bool called_FindBinding23;
        public System.Int32 FindBinding(UnityEngine.InputSystem.InputBinding mask, UnityEngine.InputSystem.InputAction action)
        {
            if (mtd_FindBinding23 == null)
                mtd_FindBinding23 = instance.Type.GetMethod("FindBinding", 2);
            System.Int32 ret = default(System.Int32);
            if (mtd_FindBinding23 != null && !called_FindBinding23)
            {
                called_FindBinding23 = true;
                ret = (System.Int32)appdomain.Invoke(mtd_FindBinding23, instance, mask, action);
                called_FindBinding23 = false;
            }
            return ret;
        }

        IMethod mtd_GetEnumerator07;
        bool called_GetEnumerator07;
        public System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction> GetEnumerator()
        {
            if (mtd_GetEnumerator07 == null)
                mtd_GetEnumerator07 = instance.Type.GetMethod("GetEnumerator", 0); // Assuming 'instance' is already defined elsewhere in your code

            System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction> ret = null;

            if (mtd_GetEnumerator07 != null && !called_GetEnumerator07)
            {
                called_GetEnumerator07 = true;
                ret = (System.Collections.Generic.IEnumerator<UnityEngine.InputSystem.InputAction>)appdomain.Invoke(mtd_GetEnumerator07, instance, null);
                called_GetEnumerator07 = false;
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