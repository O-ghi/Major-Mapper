using System;
using System.Collections.Generic;
using System.Reflection;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Byte_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            System_NotImplementedException_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_Resources_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            LitJson_JsonMapper_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            AssetLoadManager_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            UnityEngine_Screen_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            System_Object_Binding.Register(app);
            Debuger_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Threading_Thread_Binding.Register(app);
            GameInfoManager_Binding.Register(app);
            System_IO_Directory_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            UnityEngine_Networking_UnityWebRequest_Binding.Register(app);
            UnityEngine_Networking_DownloadHandler_Binding.Register(app);
            RequestFileInfo_Binding.Register(app);
            UnityEngine_JsonUtility_Binding.Register(app);
            System_Int64_Binding.Register(app);
            System_Exception_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILRTIDisposeExtendAdaptor_Binding_Adaptor_Binding.Register(app);
            System_IO_FileStream_Binding.Register(app);
            System_IO_Stream_Binding.Register(app);
            System_Net_WebRequest_Binding.Register(app);
            System_IAsyncResult_Binding.Register(app);
            System_Net_WebResponse_Binding.Register(app);
            System_Threading_WaitHandle_Binding.Register(app);
            Unity_SharpZipLib_Checksum_Crc32_Binding.Register(app);
            System_IO_File_Binding.Register(app);
            IDisposeExtend_Binding.Register(app);
            System_IO_Path_Binding.Register(app);
            System_Net_ServicePointManager_Binding.Register(app);
            System_Uri_Binding.Register(app);
            System_Net_HttpVersion_Binding.Register(app);
            System_Net_HttpWebRequest_Binding.Register(app);
            System_Collections_Generic_Queue_1_RequestFileInfo_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            Unity_SharpZipLib_Zip_ZipConstants_Binding.Register(app);
            Unity_SharpZipLib_Zip_ZipOutputStream_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Char_Binding.Register(app);
            Unity_SharpZipLib_Zip_ZipEntry_Binding.Register(app);
            Unity_SharpZipLib_Zip_ZipInputStream_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Collections_Generic_Queue_1_Action_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding.Register(app);
            CoroutineManager_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Xml_XmlNode_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_List_1_ILTypeInstance_Binding.Register(app);
            XmlNodeUtils_Binding.Register(app);
            System_Xml_XmlElement_Binding.Register(app);
            System_Collections_Generic_List_1_List_1_ILTypeInstance_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Xml_XmlNodeList_Binding.Register(app);
            System_Action_1_Dictionary_2_String_List_1_List_1_ILTypeInstance_Binding.Register(app);
            System_Action_Binding.Register(app);
            System_Collections_Generic_List_1_Action_Binding.Register(app);
            System_Collections_Generic_List_1_MemberInfo_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Binding.Register(app);
            System_Action_1_ILTypeInstance_Binding.Register(app);
            ConfigLoaderUtils_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            System_Action_2_String_List_1_List_1_ILTypeInstance_Binding.Register(app);
            System_Activator_Binding.Register(app);
            FieldInfoFix_Binding.Register(app);
            GameTools2_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding.Register(app);
            UnityEngine_SystemInfo_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            System_Xml_XmlDocument_Binding.Register(app);
            Ink_Runtime_Story_Binding.Register(app);
            System_Collections_Generic_List_1_TriggerListener_Binding.Register(app);
            System_Collections_Generic_List_1_TextAsset_Binding.Register(app);
            System_Collections_Generic_List_1_Choice_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            TransformExtension_Binding.Register(app);
            System_Array_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            System_Collections_Generic_List_1_Choice_Binding_Enumerator_Binding.Register(app);
            Ink_Runtime_Choice_Binding.Register(app);
            UnityEngine_Behaviour_Binding.Register(app);
            UnityEngine_EventSystems_TriggerListener_Binding.Register(app);
            System_Collections_Generic_List_1_UnityEngine_EventSystems_TriggerListener_Binding_Entry_Binding.Register(app);
            UnityEngine_EventSystems_TriggerListener_Binding_Entry_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            UnityEngine_WaitForEndOfFrame_Binding.Register(app);
            UnityEngine_EventSystems_EventSystem_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Action_1_Int32_Binding.Register(app);
            PlayerInput_Binding.Register(app);
            PlayerInput_Binding_GameActions_Binding.Register(app);
            UnityEngine_InputSystem_InputAction_Binding_CallbackContext_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            GameLauncherCore_Binding.Register(app);
            ILRuntime_Runtime_Enviorment_AppDomain_Binding.Register(app);
            ILRuntime_CLR_TypeSystem_IType_Binding.Register(app);
            UnityEngine_AssetBundle_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_GameObject_List_1_IEnumerator_Binding.Register(app);
            System_Collections_Generic_List_1_IEnumerator_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            System_Double_Binding.Register(app);
            UnityEngine_UI_Slider_Binding.Register(app);
            System_Collections_Generic_List_1_UnityEngine_EventSystems_TriggerListener_Binding_Entry_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Stack_1_ILRTIDisposeExtendAdaptor_Binding_Adaptor_Binding.Register(app);
            System_GC_Binding.Register(app);
            GameConvert_Binding.Register(app);
            System_ValueType_Binding.Register(app);
            AssetLoadOperation_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            UnityEngine_Physics2D_Binding.Register(app);
            UnityEngine_Rigidbody2D_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Random_Binding.Register(app);
            System_Random_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Action_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Action_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_Action_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
