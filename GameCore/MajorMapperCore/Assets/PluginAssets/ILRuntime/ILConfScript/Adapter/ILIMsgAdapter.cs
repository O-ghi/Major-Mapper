using System;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

namespace ILRuntime
{
    public class ILIMsgAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(IMsg);
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
        public class Adaptor : IMsg, CrossBindingAdaptorType
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

            IMethod mIdMethod;
            bool mIdMethodGot;
            public int GetMsgId()
            {
                if (!mIdMethodGot)
                {
                    mIdMethod = instance.Type.GetMethod("GetMsgId", 0);
                    mIdMethodGot = true;
                }
                if (mIdMethod != null)
                {
                    int ret = (int)appdomain.Invoke(mIdMethod, instance, null);
                    return ret;
                }

                Debug.LogError("获取id失败" + instance);
                return -1;
            }

            IMethod mSizeMethod;
            bool mSizeMethodGot;
            public int GetMsgSize()
            {
                if (!mSizeMethodGot)
                {
                    mSizeMethod = instance.Type.GetMethod("GetMsgSize", 0);
                    mSizeMethodGot = true;
                }
                if (mSizeMethod != null)
                {
                    int ret = (int)appdomain.Invoke(mSizeMethod, instance, null);
                    return ret;
                }
                Debug.LogError("获取size失败" + instance);
                return -1;
            }

            IMethod mDataMethod;
            bool mDataMethodGot;
            public byte[] GetMsgData()
            {
                if (!mDataMethodGot)
                {
                    mDataMethod = instance.Type.GetMethod("GetMsgData", 0);
                    mDataMethodGot = true;
                }
                if (mDataMethod != null)
                {
                    byte[] ret = (byte[])appdomain.Invoke(mDataMethod, instance, null);
                    return ret;
                }
                return null;
            }
        }
    }
}