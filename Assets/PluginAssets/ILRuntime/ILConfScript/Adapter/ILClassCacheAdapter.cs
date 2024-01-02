using System;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

namespace ILRuntime
{
    public class ILClassCacheAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(IClassCache);
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
        public class Adaptor : IClassCache, CrossBindingAdaptorType
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

            IMethod mCtrMethod;
            bool mCtrMethodGot;
            bool mCtrCalled;
            public override void FakeCtr(IParam param)
            {
                if (!mCtrMethodGot)
                {
                    mCtrMethod = instance.Type.GetMethod("FakeCtr", 1);
                    mCtrMethodGot = true;
                }

                if (mCtrMethod != null && !mCtrCalled)
                {
                    mCtrCalled = true;
                    appdomain.Invoke(mCtrMethod, instance, param);
                    mCtrCalled = false;
                }
            }

            IMethod mDtrMethod;
            bool mDtrMethodGot;
            bool mDtrCalled;
            public override void FakeDtr()
            {
                if (!mDtrMethodGot)
                {
                    mDtrMethod = instance.Type.GetMethod("FakeDtr", 0);
                    mDtrMethodGot = true;
                }

                if (mDtrMethod != null && !mDtrCalled)
                {
                    mDtrCalled = true;
                    appdomain.Invoke(mDtrMethod, instance, null);
                    mDtrCalled = false;
                }
            }
        }
    }

}