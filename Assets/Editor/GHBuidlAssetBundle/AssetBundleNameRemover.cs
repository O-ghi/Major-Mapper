using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

// Tool gỡ Bundle name. Trình add code 10/10/2023.

public class AssetBundleNameRemover : EditorWindow
{
    /// <summary>
    /// Các Bundle name được bỏ qua. Asset được gắn các Bundle name này sẽ không bị gỡ
    /// dù chúng có nằm ngoài thư mục chỉ định đi nữa.
    /// </summary>
    public static string ignoredBundleNames = "gameshaderall,gamelogic,configgame";

    /// <summary>
    /// Đường dẫn đến thư mục chỉ định. Các Asset nằm trong thư mục này sẽ không bị gỡ
    /// Bundle name.
    /// </summary>
    public static string specifiedFolder = "ArtResources/OutPut";

    public static string userRequestFolder = "ArtResources/OutPut/Scene/new_scene";

    [MenuItem("Tools/AssetBundles/Remove AssetBundle Name")]
    static void Init()
    {
        // Tìm một Window có sẵn. Nếu không có thì mở một Window mới.
        AssetBundleNameRemover window = (AssetBundleNameRemover)EditorWindow.GetWindow(typeof(AssetBundleNameRemover));
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Gỡ hết tất cả Bundle name, trừ một vài ngoại lệ", EditorStyles.boldLabel);
        GUILayout.Label("Tool này giúp gỡ Asset Bundle name khỏi tất cả asset nằm ngoài thư mục chỉ định.");
        GUILayout.Label("Do Tool quét hết tất cả Asset, nên vui lòng chỉ dùng khi thật sự cần.");
        specifiedFolder = EditorGUILayout.TextField("Thư mục chỉ định", specifiedFolder);
        GUILayout.Label("Các asset nằm trong thư mục này sẽ không bị xóa AssetBundle name.", EditorStyles.miniBoldLabel);
        ignoredBundleNames = EditorGUILayout.TextField("Các Bundle name bỏ qua", ignoredBundleNames);
        GUILayout.Label("Các asset có Bundle name trong đây sẽ không bị gỡ ngay cả khi chúng nằm ngoài thư mục chỉ định.", EditorStyles.miniBoldLabel);
        GUILayout.Label("Các Folder bị bỏ qua bởi quá trình gán Bundle name: ");
        GUILayout.Label(EditorBundleWindow.folderIgnoreAssignBundle);

        if (GUILayout.Button("Gỡ Bundle name"))
        {
            if (EditorUtility.DisplayDialog("Gỡ Bundle name", "Bạn thật sự muốn gỡ Bundle name chứ? Hành động này có thể mất đến 30 phút.", "Chắc chắn", "Không"))
            {
               RemoveAssetBundleName();
            }
        }
        GUILayout.EndVertical();

        GUILayout.Label(string.Empty);

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Gỡ hết tất cả Bundle name trong thư mục, không ngoại lệ", EditorStyles.boldLabel);
        GUILayout.Label("Nếu mọi người muốn gỡ Bundle name của tất cả asset tại 1 thư mục nào đó.");
        GUILayout.Label("Tất cả asset trong đó sẽ bị gỡ Bundle name mà không có ngoại lệ.");
        userRequestFolder = EditorGUILayout.TextField("Thư mục cần gỡ", userRequestFolder);

        if (GUILayout.Button("Gỡ Bundle name"))
        {
            RemoveUserRequestFolderBundle();
        }
        GUILayout.EndVertical();
    }

    public static void RemoveUserRequestFolderBundle()
    {
        string path = "Assets/" + userRequestFolder;
        string[] assets = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        int count = 0;
        int processedCount = 0;
        foreach (var asset in assets) 
        {
            processedCount++;
            if (asset.EndsWith(".meta")
                || asset.EndsWith(".cs"))
                continue;

            if (AssetImporter.GetAtPath(asset).assetBundleName.Equals(string.Empty) == false)
            {
                EditorUtility.DisplayProgressBar("Gỡ Asset Bundle name...",
                string.Format("Tiến độ {0}/{1} assets...", processedCount, assets.Length), ((float)processedCount / (float)assets.Length));
                Debug.Log("<color=red>[AssetBundleNameRemover] Bundle name: "
                    + AssetImporter.GetAtPath(asset).assetBundleName + " thuộc về: " + asset + "</color>");
                AssetImporter.GetAtPath(asset).SetAssetBundleNameAndVariant(null, null);
                count++;
                //EditorUtility.ClearProgressBar();
                //return;
            }
        }
        Debug.Log("<color=lime>Đã gỡ Bundle name khỏi " + count + " asset!</color>");
        EditorUtility.ClearProgressBar();
        return;
    }

    public static void RemoveAssetBundleName()
    {
        #region Lấy tất cả đường dẫn đến mọi asset.
        string folderPaths = "Assets/";
        string[] assets = Directory.GetFiles(folderPaths, "*", SearchOption.AllDirectories);
        #endregion 

        // Số asset đã bị gỡ Bundle name.
        int count = 0;

        #region Xử lý đường dẫn
        string[] ignoreBundleNamesArray = ignoredBundleNames.Split(',');
        string[] ignoreFolders = EditorBundleWindow.folderIgnoreAssignBundle.Split(',');

        string specifiedFolderPath = specifiedFolder.Replace("/", "\\");
        #endregion 

        // Số asset đã xử lý.
        int processedCount = 0;
        foreach (var asset in assets)
        {
            processedCount++;
            EditorUtility.DisplayProgressBar("Gỡ Asset Bundle name...",
                string.Format("Tiến độ {0}/{1} assets...", processedCount, assets.Length), ((float)processedCount / (float)assets.Length));

            // Bỏ qua file meta và file script.
            if (asset.EndsWith(".meta")
                || asset.EndsWith(".cs"))
                continue;

            #region Các trường hợp bị bỏ qua.
            bool shouldBeIgnored = false;
            foreach (var ignored in ignoreFolders)
            {
                if (asset.Contains(ignored))
                {
                    shouldBeIgnored = true;
                    break;
                }
            }

            if (shouldBeIgnored == false) 
            {
                foreach (var ignored in EditorBundleWindow.folderIgnoreAssignBundle.Split(','))
                {
                    if (asset.Contains(ignored))
                    {
                        shouldBeIgnored = true;
                        break;
                    }
                }
            }

            if (shouldBeIgnored)
                continue;
            #endregion

            //string res = specifiedFolderPath.Equals("ArtResources\\OutPut") ? "yes" : "no";
            //Debug.Log(specifiedFolderPath + " " + "ArtResources\\OutPut");
            //Debug.Log(res);
            //EditorUtility.ClearProgressBar();
            //return;

            //if (asset.Contains("ArtResources\\OutPut"))
            if (asset.Contains(specifiedFolderPath))
            {
                // Nếu asset có trong thư mục chỉ định, nhưng Bundle name là dạng đường dẫn, thì cũng gỡ.
                if (AssetImporter.GetAtPath(asset).assetBundleName.Contains("/"))
                {
                    Debug.Log("<color=red>[AssetBundleNameRemover] Bundle name không đúng: " 
                        + AssetImporter.GetAtPath(asset).assetBundleName + " thuộc về: " + asset + "</color>");
                    AssetImporter.GetAtPath(asset).SetAssetBundleNameAndVariant(null, null);
                    count++;
                    //EditorUtility.ClearProgressBar();
                    //return;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                // Bỏ qua nếu file không phải asset.
                if (AssetImporter.GetAtPath(asset) == null)
                {
                    continue;
                }

                bool exception = false;
                // Bỏ qua các Bundle name đặc biệt.
                foreach (string ignoredBundleName in ignoreBundleNamesArray)
                {
                    if (AssetImporter.GetAtPath(asset).assetBundleName == ignoredBundleName)
                    {
                        exception = true;
                        break;
                    }
                }
                if (exception)
                    continue;
                if (AssetImporter.GetAtPath(asset).assetBundleName.Equals(string.Empty) == false)
                {
                    Debug.Log("<color=red>[AssetBundleNameRemover] Bundle name không đúng: "
                        + AssetImporter.GetAtPath(asset).assetBundleName + " thuộc về: " + asset + "</color>");
                    AssetImporter.GetAtPath(asset).SetAssetBundleNameAndVariant(null, null);
                    count++;
                    //EditorUtility.ClearProgressBar();
                    //return;
                }

            }
        }
        Debug.Log("<color=lime>Đã gỡ Bundle name khỏi " + count + " asset!</color>");
        EditorUtility.ClearProgressBar();
        return;
    }
}
