using System;
using UnityEngine;

namespace ILRuntime
{
    public class ILScriptBinder
    {
        public static Mono.Cecil.ArrayDimension[] arr;
        public static void Bind(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            appdomain.RegisterCrossBindingAdaptor(new ILCoroutineAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILMonoBehaviourAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILPlayableAssetAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILPlayableBehaviourAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILBasePlayableBehaviourAdapter());

            appdomain.RegisterCrossBindingAdaptor(new ILIMsgAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILBaseMessageAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILBaseBehaviourAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILClassCacheAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILRTIDisposeExtendAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new ILRTIInputActionCollection2ExtendAdaptor());
            //appdomain.RegisterCrossBindingAdaptor(new ILRTPlayerInputAdaptor());
            appdomain.RegisterCrossBindingAdaptor(new ILRTIGameActionsAdaptor());



#if USE_HOT
            appdomain.RegisterCrossBindingAdaptor(new ILServiceAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ILBaseWindowAdapter());
#endif
            //ILGSliderAdatper

            //arr = Mono.Empty<Mono.Cecil.ArrayDimension>.Array;
            if (Application.isPlaying)
            {
#if !ONLY_PGAME
                ILGetAddComponent.Bind(appdomain);
                float time = Time.realtimeSinceStartup;
                //ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
                Debug.Log("Binding Wrap time > " + (Time.realtimeSinceStartup - time) + "s");
#endif

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS)
                appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            }

            //委托
            //appdomain.DelegateManager.RegisterMethodDelegate<GameEvent>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, UnityEngine.Font>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Color>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.String, UnityEngine.AssetBundle>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String, System.Type>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Type, System.Object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILBaseMessageAdapter.Adaptor, ILBaseMessageAdapter.Adaptor, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Color>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int64, System.Int64, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform, System.Int32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<System.Object>>();






            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILBaseMessageAdapter.Adaptor>>((act) =>
            {
                return new System.Comparison<ILBaseMessageAdapter.Adaptor>((x, y) =>
                {
                    return ((Func<ILBaseMessageAdapter.Adaptor, ILBaseMessageAdapter.Adaptor, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.String>>((act) =>
            {
                return new System.Comparison<System.String>((x, y) =>
                {
                    return ((Func<System.String, System.String, System.Int32>)act)(x, y);
                });
            });


            

           

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Video.VideoPlayer.EventHandler>((act) =>
            {
                return new UnityEngine.Video.VideoPlayer.EventHandler((source) =>
                {
                    ((Action<UnityEngine.Video.VideoPlayer>)act)(source);
                });
            });

           

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });



            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
            {
                return new System.Comparison<System.Int32>((x, y) =>
                {
                    return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int64>>((act) =>
            {
                return new System.Comparison<System.Int64>((x, y) =>
                {
                    return ((Func<System.Int64, System.Int64, System.Int32>)act)(x, y);
                });
            });

          
            /*appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.UIPackage.LoadResource>((act) =>
            {
                return new FairyGUI.UIPackage.LoadResource((name, extension, type) =>
                {
                    return ((Func<System.String, System.String, System.Type, System.Object>)act)(name, extension, type);
                });
            });*/
            
        }
    }
}