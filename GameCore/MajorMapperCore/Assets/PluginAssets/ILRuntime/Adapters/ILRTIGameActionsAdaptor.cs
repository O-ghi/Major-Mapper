using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ILRTIGameActionsAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(PlayerInput.IGameActions);
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

	public class Adaptor : PlayerInput.IGameActions, CrossBindingAdaptorType
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


        IMethod mtd_OnMove13;
        bool called_OnMove13;
        public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (mtd_OnMove13 == null)
                mtd_OnMove13 = instance.Type.GetMethod("OnMove", 1);
            
            if (mtd_OnMove13 != null && !called_OnMove13)
            {
                called_OnMove13 = true;
                appdomain.Invoke(mtd_OnMove13, instance, context);
                called_OnMove13 = false;
            }
           
            
        }
        
        IMethod mtd_OnInteract14;
        bool called_OnInteract14;
        public void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (mtd_OnInteract14 == null)
                mtd_OnInteract14 = instance.Type.GetMethod("OnInteract", 1);
            
            if (mtd_OnInteract14 != null && !called_OnInteract14)
            {
                called_OnInteract14 = true;
                appdomain.Invoke(mtd_OnInteract14, instance, context);
                called_OnInteract14 = false;
            }
            
            
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