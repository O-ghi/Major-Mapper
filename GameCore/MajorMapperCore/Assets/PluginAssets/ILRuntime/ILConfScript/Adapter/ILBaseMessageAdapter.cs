using System;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

namespace ILRuntime
{
    public class ILBaseMessageAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(BaseMessage);
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
        public class Adaptor : BaseMessage, CrossBindingAdaptorType
        {
            public ILTypeInstance instance;
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

            IMethod mMsgMethod;
            bool mMsgMethodGot;
            bool mMsgCalled;
            public override int GetMsgId()
            {
                if (!mMsgMethodGot)
                {
                    mMsgMethod = instance.Type.GetMethod("GetMsgId", 0);
                    mMsgMethodGot = true;
                }

                if (mMsgMethod != null && !mMsgCalled)
                {
                    mMsgCalled = true;
                    int ret = (int)appdomain.Invoke(mMsgMethod, instance, null);
                    mMsgCalled = false;
                    return ret;
                }
                else
                {
                    return base.GetMsgId();
                }
            }

            IMethod mReadMethod;
            bool mReadMethodGot;
            bool mReadCalled;
            public override int Read(byte[] buffer, int offset)
            {
                if (!mReadMethodGot)
                {
                    mReadMethod = instance.Type.GetMethod("Read", 2);
                    mReadMethodGot = true;
                }

                if (mReadMethod != null && !mReadCalled)
                {
                    mReadCalled = true;
                    offset = (int)appdomain.Invoke(mReadMethod, instance, buffer, offset);
                    mReadCalled = false;
                }
                else
                {
                    offset = base.Read(buffer, offset);
                }
                return offset;
            }

            IMethod mRstMethod;
            bool mRstMethodGot;
            bool mRstCalled;
            public override void Reset()
            {
                if (!mRstMethodGot)
                {
                    mRstMethod = instance.Type.GetMethod("Reset", 0);
                    mRstMethodGot = true;
                }

                if (mRstMethod != null && !mRstCalled)
                {
                    mRstCalled = true;
                    appdomain.Invoke(mRstMethod, instance, null);
                    mRstCalled = false;
                }
                else
                {
                    base.Reset();
                }
            }

            IMethod mWriteMethod;
            bool mWriteMethodGot;
            bool mWriteCalled;
            public override int Write(byte[] buffer, int offset)
            {
                if (!mWriteMethodGot)
                {
                    mWriteMethod = instance.Type.GetMethod("Write", 2);
                    mWriteMethodGot = true;
                }

                if (mWriteMethod != null && !mWriteCalled)
                {
                    mWriteCalled = true;
                    offset = (int)appdomain.Invoke(mWriteMethod, instance, buffer, offset);
                    mWriteCalled = false;
                }
                else
                {
                    offset = base.Write(buffer, offset);
                }
                return offset;
            }

            IMethod mWriteTypeMethod;
            bool mWriteTypeMethodGot;
            bool mWriteTypeCalled;
            public override int WriteWithType(byte[] buffer, int offset)
            {
                if (!mWriteTypeMethodGot)
                {
                    mWriteTypeMethod = instance.Type.GetMethod("WriteWithType", 2);
                    mWriteTypeMethodGot = true;
                }

                if (mWriteTypeMethod != null && !mWriteTypeCalled)
                {
                    mWriteTypeCalled = true;
                    offset = (int)appdomain.Invoke(mWriteTypeMethod, instance, buffer, offset);
                    mWriteTypeCalled = false;
                }
                else
                {
                    offset = base.WriteWithType(buffer, offset);
                }
                return offset;
            }
        }
    }
}