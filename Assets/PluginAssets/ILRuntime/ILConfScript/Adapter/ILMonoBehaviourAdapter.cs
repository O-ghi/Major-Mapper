using UnityEngine;
using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

namespace ILRuntime
{
    public class ILMonoBehaviourAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(MonoBehaviour);
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
        public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
        {
#if UNITY_EDITOR
            public string scriptName;
#endif
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

            IMethod mAwakeMethod;
            bool mAwakeMethodGot;
            public void Awake()
            {
                //Unity会在ILRuntime准备好这个实例前调用Awake，所以这里暂时先不掉用
                if (instance != null)
                {
#if UNITY_EDITOR
                    scriptName = instance.Type.FullName;
#endif
                    if (!mAwakeMethodGot)
                    {
                        mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                        mAwakeMethodGot = true;
                    }

                    if (mAwakeMethod != null)
                    {
                        appdomain.Invoke(mAwakeMethod, instance, null);
                    }
                }
            }

            IMethod mStartMethod;
            bool mStartMethodGot;
            void Start()
            {
                if (!mStartMethodGot)
                {
                    mStartMethod = instance.Type.GetMethod("Start", 0);
                    mStartMethodGot = true;
                }

                if (mStartMethod != null)
                {
                    appdomain.Invoke(mStartMethod, instance, null);
                }
            }

            IMethod mEnableMethod;
            bool mEnableMethodGot;
            void OnEnable()
            {
                if (instance == null)
                    return;

                if (!mEnableMethodGot)
                {
                    mEnableMethod = instance.Type.GetMethod("OnEnable", 0);
                    mEnableMethodGot = true;
                }

                if (mEnableMethod != null)
                {
                    appdomain.Invoke(mEnableMethod, instance, null);
                }
            }

            IMethod mDisAblMethod;
            bool mDisAblMethodGot;
            void OnDisable()
            {
                if (!mDisAblMethodGot)
                {
                    mDisAblMethod = instance.Type.GetMethod("OnDisable", 0);
                    mDisAblMethodGot = true;
                }

                if (mDisAblMethod != null)
                {
                    appdomain.Invoke(mDisAblMethod, instance, null);
                }
            }

            /******************碰撞相关***************************/

            IMethod mOnColliderEnterMethod;
            bool mOnColliderEnterMethodGot;
            void OnCollisionEnter(Collision collision)
            {
                if (!mOnColliderEnterMethodGot)
                {
                    mOnColliderEnterMethod = instance.Type.GetMethod("OnCollisionEnter", 1);
                    mOnColliderEnterMethodGot = true;
                }

                if (mOnColliderEnterMethod != null)
                {
                    appdomain.Invoke(mOnColliderEnterMethod, instance, collision);
                }
            }

            IMethod mOnColliderExitMethod;
            bool mOnColliderExitMethodGot;
            void OnCollisionExit(Collision collision)
            {
                if (!mOnColliderExitMethodGot)
                {
                    mOnColliderExitMethod = instance.Type.GetMethod("OnCollisionExit", 1);
                    mOnColliderExitMethodGot = true;
                }

                if (mOnColliderExitMethod != null)
                {
                    appdomain.Invoke(mOnColliderExitMethod, instance, collision);
                }
            }

            IMethod mOnColliderStayMethod;
            bool mOnColliderStayMethodGot;
            void OnCollisionStay(Collision collision)
            {
                if (!mOnColliderStayMethodGot)
                {
                    mOnColliderStayMethod = instance.Type.GetMethod("OnCollisionStay", 1);
                    mOnColliderExitMethodGot = true;
                }

                if (mOnColliderStayMethod != null)
                {
                    appdomain.Invoke(mOnColliderStayMethod, instance, collision);
                }
            }
            /*------------------------------*/
            IMethod mOnTriggerEnterMethod;
            bool mOnTriggerEnterGot;
            void OnTriggerEnter(Collider other)
            {
                if (!mOnTriggerEnterGot)
                {
                    mOnTriggerEnterMethod = instance.Type.GetMethod("OnTriggerEnter", 1);
                    mOnTriggerEnterGot = true;
                }

                if (mOnTriggerEnterMethod != null)
                {
                    appdomain.Invoke(mOnTriggerEnterMethod, instance, other);
                }
            }

            IMethod mOnTriggerStayMethod;
            bool mOnTriggerStayGot;
            void OnTriggerStay(Collider other)
            {
                if (!mOnTriggerStayGot)
                {
                    mOnTriggerStayMethod = instance.Type.GetMethod("OnTriggerStay", 1);
                    mOnTriggerStayGot = true;
                }

                if (mOnTriggerStayMethod != null)
                {
                    appdomain.Invoke(mOnTriggerStayMethod, instance, other);
                }
            }

            IMethod mOnTriggerExitMethod;
            bool mOnTriggerExitGot;
            void OnTriggerExit(Collider other)
            {
                if (!mOnTriggerExitGot)
                {
                    mOnTriggerExitMethod = instance.Type.GetMethod("OnTriggerExit", 1);
                    mOnTriggerExitGot = true;
                }

                if (mOnTriggerExitMethod != null)
                {
                    appdomain.Invoke(mOnTriggerExitMethod, instance, other);
                }
            }


            /******************碰撞相关***************************/

            IMethod mPauseMethod;
            bool mPauseMethodGot;
            void OnApplicationPause(bool paused)
            {
                if (!mPauseMethodGot)
                {
                    mPauseMethod = instance.Type.GetMethod("OnApplicationPause", 1);
                    mPauseMethodGot = true;
                }

                if (mPauseMethod != null)
                {
                    appdomain.Invoke(mPauseMethod, instance, paused);
                }
            }

            IMethod mUpdateMethod;
            bool mUpdateMethodGot;
            void Update()
            {
                if (!mUpdateMethodGot)
                {
                    mUpdateMethod = instance.Type.GetMethod("Update", 0);
                    mUpdateMethodGot = true;
                }

                if (mUpdateMethod != null)
                {
                    appdomain.Invoke(mUpdateMethod, instance, null);
                }
            }

            IMethod mFixUptMethod;
            bool mFixUptMethodGot;
            void FixedUpdate()
            {
                if (!mFixUptMethodGot)
                {
                    mFixUptMethod = instance.Type.GetMethod("FixedUpdate", 0);
                    mFixUptMethodGot = true;
                }

                if (mFixUptMethod != null)
                {
                    appdomain.Invoke(mFixUptMethod, instance, null);
                }
            }

            IMethod mLateUptMethod;
            bool mLateUptMethodGot;
            void LateUpdate()
            {
                if (!mLateUptMethodGot)
                {
                    mLateUptMethod = instance.Type.GetMethod("LateUpdate", 0);
                    mLateUptMethodGot = true;
                }

                if (mLateUptMethod != null)
                {
                    appdomain.Invoke(mLateUptMethod, instance, null);
                }
            }

            IMethod monDesMethod;
            bool monDesMethodGot;
            void OnDestroy()
            {
                if (!monDesMethodGot)
                {
                    monDesMethod = instance.Type.GetMethod("OnDestroy", 0);
                    monDesMethodGot = true;
                }

                if (monDesMethod != null)
                {
                    appdomain.Invoke(monDesMethod, instance, null);
                }
            }

            private bool msgGot;
            private bool msgCalled;
            private IMethod msgMethod;
            public void OnMsg(string msg)
            {
                if (!msgGot)
                {
                    msgGot = true;
                    msgMethod = instance.Type.GetMethod("OnMsg", 1);
                }

                if (msgMethod != null && !msgCalled)
                {
                    msgCalled = true;
                    appdomain.Invoke(msgMethod, instance, msg);
                    msgCalled = false;
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
}