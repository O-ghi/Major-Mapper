#if USE_HOT
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
namespace ILRuntime
{
    public class ILServiceAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(BaseService);
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

        public class Adaptor : BaseService, CrossBindingAdaptorType
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

            public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

            public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }


            private bool clearCalled;
            public override void ClearData()
            {
                IMethod m = instance.Type.GetMethod("ClearData", 0);
                if (m != null && m is IMethod && !clearCalled)
                {
                    clearCalled = true;
                    appdomain.Invoke(m, instance, null);
                    clearCalled = false;
                }
                else
                {
                    base.ClearData();
                }
            }

            private bool rectCalled;
            public override void OnReconnected()
            {
                IMethod m = instance.Type.GetMethod("OnReconnected", 0);
                if (m != null && m is IMethod && !rectCalled)
                {
                    rectCalled = true;
                    appdomain.Invoke(m, instance, null);
                    rectCalled = false;
                }
                else
                {
                    base.OnReconnected();
                }
            }

            private bool regCalled;
            protected override void RegisterEventListener()
            {
                IMethod m = instance.Type.GetMethod("RegisterEventListener", 0);
                if (m != null && m is IMethod && !regCalled)
                {
                    regCalled = true;
                    appdomain.Invoke(m, instance, null);
                    regCalled = false;
                }
                else
                {
                    base.RegisterEventListener();
                }
            }
        }
    }
}
#endif
