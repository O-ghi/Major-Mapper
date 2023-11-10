using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonoBehaviourLogic.CreateInstance();

        //Create Manager
        PanelManager.CreateInstance();
        DialogueManager.CreateInstance();
        InputManager.CreateInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            var obj = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "scenes/campingscene"));
            var scenes = obj.GetAllScenePaths();
            //obj.LoadAsset("campingscene", typeof(Scene));
            SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenes[0]));
        }
    }
}
