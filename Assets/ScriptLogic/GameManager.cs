using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviourSingleton<GameManager>
{

    public static void Initialize(GameObject obj, bool debugMode)
    {
        obj.AddComponent<GameManager>();
        Debug.Log("进入GameManager!!!!!!!!!!");
    }  
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        MonoBehaviourLogic.CreateInstance();
        InitManager();
        //Create Manager
        PanelManager.CreateInstance();
        DialogueManager.CreateInstance();
        InputManager.CreateInstance();
        ConfigManager.CreateInstance();
        SceneDataManager.CreateInstance();
        EntityManager.CreateInstance();
        TaskManager.CreateInstance();

        var mainmenu = PanelManager.SetPanel("MainMenuPanel");

    }
    private void InitManager()
    {
        gameObject.AddComponent<CoroutineManager>();
        gameObject.AddComponent<AssetLoadManager>();

    }
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    //var obj = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "scenes/campingscene"));
        //    //var scenes = obj.GetAllScenePaths();
        //    //obj.LoadAsset("campingscene", typeof(Scene));
        //    SceneManager.ChangeScene(1);
            
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    //var obj = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "scenes/campingscene"));
        //    //var scenes = obj.GetAllScenePaths();
        //    //obj.LoadAsset("campingscene", typeof(Scene));
        //}
    }
}
