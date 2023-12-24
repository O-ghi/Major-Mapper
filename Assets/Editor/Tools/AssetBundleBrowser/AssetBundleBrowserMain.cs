using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UnityEngine.AssetBundles
{

    public class AssetBundleBrowserMain : EditorWindow, IHasCustomMenu
    {

        public const float kButtonWidth = 150;

        enum Mode
        {
            Browser,
            Builder,
            Inspect,
            Patch,
            Export,
        }
        [SerializeField]
        Mode m_Mode;

        [SerializeField]
        public AssetBundleManageTab m_ManageTab;

        [SerializeField]
        public AssetBundleBuildTab m_BuildTab;

        [SerializeField]
        public AssetBundleInspectTab m_InspectTab;

        [SerializeField]
        public AssetBundlePatchTab m_PatchTab;

        [SerializeField]
        public ExportProjectTab m_ExportTab;

        private Texture2D m_RefreshTexture;

        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        [MenuItem("Window/AssetBundle Browser", priority = 2050)]
        static void ShowWindow()
        {
            var window = GetWindow<AssetBundleBrowserMain>();
            window.titleContent = new GUIContent("AssetBundles");
            window.Show();
        }

        [SerializeField]
        public bool multiDataSource = false;
        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            //menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Custom Sources"), multiDataSource, FlipDataSource);
        }
        public void FlipDataSource()
        {
            multiDataSource = !multiDataSource;
        }

        private void OnEnable()
        {

            Rect subPos = GetSubWindowArea();
            if(m_ManageTab == null)
                m_ManageTab = new AssetBundleManageTab();
            m_ManageTab.OnEnable(subPos, this);
            if(m_BuildTab == null)
                m_BuildTab = new AssetBundleBuildTab();
            m_BuildTab.OnEnable(subPos, this);
            if (m_InspectTab == null)
                m_InspectTab = new AssetBundleInspectTab();
            m_InspectTab.OnEnable(subPos, this);
            if (m_PatchTab == null)
                m_PatchTab = new AssetBundlePatchTab();
            m_PatchTab.OnEnable(subPos, this);
            if (m_ExportTab == null)
                m_ExportTab = new ExportProjectTab();
            m_ExportTab.OnEnable(subPos, this);

            m_RefreshTexture = EditorGUIUtility.FindTexture("Refresh");
            Debug.Log("m_RefreshTexture is " + (m_RefreshTexture != null));

            //determine if we are "multi source" or not...
            multiDataSource = false;
            List<System.Type> types = AssetBundleDataSource.ABDataSourceProviderUtility.CustomABDataSourceTypes;
            if (types.Count > 1)
                multiDataSource = true;
        }
        private void OnDisable()
        {
            if (m_BuildTab != null)
                m_BuildTab.OnDisable();
            if (m_InspectTab != null)
                m_InspectTab.OnDisable();
            if (m_PatchTab != null)
                m_PatchTab.OnDisable();
            if (m_ExportTab != null)
                m_ExportTab.OnDisable();
        }

        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
            if (multiDataSource)
                padding += k_MenubarPadding * 0.5f;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }

        private void Update()
        {
            switch (m_Mode)
            {
                case Mode.Builder:
                    break;
                case Mode.Inspect:
                    break;
                case Mode.Patch:
                    break;
                case Mode.Export:
                    break;
                case Mode.Browser:
                default:
                    m_ManageTab.Update();
                    break;
            }
        }

        private void OnGUI()
        {
            ModeToggle();

            switch(m_Mode)
            {
                case Mode.Builder:
                    m_BuildTab.OnGUI(GetSubWindowArea());
                    break;
                case Mode.Inspect:
                    m_InspectTab.OnGUI(GetSubWindowArea());
                    break;
                case Mode.Patch:
                    m_PatchTab.OnGUI(GetSubWindowArea());
                    break;
                case Mode.Export:
                    m_ExportTab.OnGUI(GetSubWindowArea());
                    break;
                case Mode.Browser:
                default:
                    m_ManageTab.OnGUI(GetSubWindowArea());
                    break;
            }
        }

        void ModeToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);
            bool clicked = false;
            switch(m_Mode)
            {
                case Mode.Browser:
                    clicked = GUILayout.Button(m_RefreshTexture);
                    if (clicked)
                        m_ManageTab.ForceReloadData();
                    break;
                case Mode.Builder:
                    GUILayout.Space(m_RefreshTexture.width + k_ToolbarPadding);
                    break;
                case Mode.Inspect:
                    clicked = GUILayout.Button(m_RefreshTexture);
                    if (clicked)
                        m_InspectTab.RefreshBundles();
                    break;
                case Mode.Patch:
                    GUILayout.Space(m_RefreshTexture.width + k_ToolbarPadding);
                    break;
                case Mode.Export:
                    GUILayout.Space(m_RefreshTexture.width + k_ToolbarPadding);
                    break;
            }

            float toolbarWidth = position.width - k_ToolbarPadding * 4 - m_RefreshTexture.width;
            ///string[] labels = new string[2] { "Configure", "Build"};
            string[] labels = new string[5] { "Configure", "Build", "Inspect","Patch","Export & OutPackage" };
            m_Mode = (Mode)GUILayout.Toolbar((int)m_Mode, labels, "LargeButton", GUILayout.Width(toolbarWidth) );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if(multiDataSource)
            {
                //GUILayout.BeginArea(r);
                GUILayout.BeginHorizontal();

                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label("Bundle Data Source:");
                    GUILayout.FlexibleSpace();
                    var c = new GUIContent(string.Format("{0} ({1})", AssetBundleModel.Model.DataSource.Name, AssetBundleModel.Model.DataSource.ProviderName), "Select Asset Bundle Set");
                    if (GUILayout.Button(c , EditorStyles.toolbarPopup) )
                    {
                        GenericMenu menu = new GenericMenu();
                        bool firstItem = true;

                        foreach (var info in AssetBundleDataSource.ABDataSourceProviderUtility.CustomABDataSourceTypes)
                        {
                            List<AssetBundleDataSource.ABDataSource> dataSourceList = null;
                            dataSourceList = info.GetMethod("CreateDataSources").Invoke(null, null) as List<AssetBundleDataSource.ABDataSource>;
                        

                            if (dataSourceList == null)
                                continue;

                            if (!firstItem)
                            {
                                menu.AddSeparator("");
                            }

                            foreach (var ds in dataSourceList)
                            {
                                menu.AddItem(new GUIContent(string.Format("{0} ({1})", ds.Name, ds.ProviderName)), false,
                                    () =>
                                    {
                                        var thisDataSource = ds;
                                        AssetBundleModel.Model.DataSource = thisDataSource;
                                        m_ManageTab.ForceReloadData();
                                    }
                                );
                            }

                            firstItem = false;
                        }

                        menu.ShowAsContext();
                    }

                    GUILayout.FlexibleSpace();
                    if (AssetBundleModel.Model.DataSource.IsReadOnly())
                    {
                        GUIStyle tbLabel = new GUIStyle(EditorStyles.toolbar);
                        tbLabel.alignment = TextAnchor.MiddleRight;

                        GUILayout.Label("Read Only", tbLabel);
                    }
                }

                GUILayout.EndHorizontal();
                //GUILayout.EndArea();
            }
        }


    }
}