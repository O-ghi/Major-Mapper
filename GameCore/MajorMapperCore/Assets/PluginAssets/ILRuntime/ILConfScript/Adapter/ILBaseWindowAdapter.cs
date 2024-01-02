#if USE_HOT
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime
{
    public class ILBaseWindowAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(BaseWindow);
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

        public class Adaptor : BaseWindow, CrossBindingAdaptorType
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

            private bool openCalled;
            public override void OnOpen()
            {
                IMethod m = instance.Type.GetMethod("OnOpen", 0);
                if (m != null && m is IMethod && !openCalled)
                {
                    openCalled = true;
                    appdomain.Invoke(m, instance, null);
                    openCalled = false;
                }
                else
                {
                    base.OnOpen();
                }
            }

            private bool closeCalled;
            protected override void OnClose()
            {
                IMethod m = instance.Type.GetMethod("OnClose", 0);
                if (m != null && m is IMethod && !closeCalled)
                {
                    closeCalled = true;
                    appdomain.Invoke(m, instance, null);
                    closeCalled = false;
                }
                else
                {
                    base.OnClose();
                }
            }

            private bool addEvtCalled;
            public override void AddEventListener()
            {
                IMethod m = instance.Type.GetMethod("AddEventListener", 0);
                if (m != null && m is IMethod && !addEvtCalled)
                {
                    addEvtCalled = true;
                    appdomain.Invoke(m, instance, null);
                    addEvtCalled = false;
                }
                else
                {
                    base.AddEventListener();
                }
            }

            private bool removeEvtCalled;
            public override void RemoveEventListener()
            {
                IMethod m = instance.Type.GetMethod("RemoveEventListener", 0);
                if (m != null && m is IMethod && !removeEvtCalled)
                {
                    removeEvtCalled = true;
                    appdomain.Invoke(m, instance, null);
                    removeEvtCalled = false;
                }
                else
                {
                    base.RemoveEventListener();
                }
            }
        }
    }
}
#endif