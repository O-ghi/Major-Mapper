using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime
{
    public class ILEnumeratorAdatper : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(IEnumerator);
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
        //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
        public class Adaptor : IEnumerator, CrossBindingAdaptorType
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

            private bool curCalled;
            private bool curGot;
            private IMethod curMethod;
            public object Current
            {
                get
                {
                    if (!curGot)
                    {
                        curGot = true;
                        curMethod = instance.Type.GetMethod("Current");
                    }
                    if (curMethod != null && !curCalled)
                    {
                        curCalled = true;
                        var ret = appdomain.Invoke(curMethod, instance, null);
                        curCalled = false;
                        return ret;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            private bool mnCalled;
            private bool mnGot;
            private IMethod mnMethod;
            public bool MoveNext()
            {
                if (!mnGot)
                {
                    mnGot = true;
                    mnMethod = instance.Type.GetMethod("MoveNext");
                }

                if (mnMethod != null && !mnCalled)
                {
                    mnCalled = true;
                    var ret = (bool)appdomain.Invoke(mnMethod, instance, null);
                    mnCalled = false;
                    return ret;
                }
                return false;
            }

            private bool rstCalled;
            private bool rstGot;
            private IMethod rstMethod;

            public void Reset()
            {
                if (!rstGot)
                {
                    rstGot = true;
                    rstMethod = instance.Type.GetMethod("Reset");
                }

                if (rstMethod != null && !rstCalled)
                {
                    rstCalled = true;
                    appdomain.Invoke(rstMethod, instance, null);
                    rstCalled = false;
                }
            }
        }
    }
}