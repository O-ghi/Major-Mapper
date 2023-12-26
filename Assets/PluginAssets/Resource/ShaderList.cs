using UnityEngine;
using System.Collections.Generic;

public class ShaderList : MonoBehaviour
{
    public static ShaderList instance;
    private static Dictionary<string, Shader> map;
    public List<Shader> shaders = new List<Shader>();

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    void mapShader()
    {
        instance = this;
        map = new Dictionary<string, Shader>();
        for (int i = 0, len = shaders.Count; i < len; ++i)
        {
            if (shaders[i] != null)
                map[shaders[i].name] = shaders[i];
        }
    }

    public Shader Find(string name)
    {
#if UNITY_EDITOR && !Main
        return Shader.Find(name);
#else
        if (map == null)
            mapShader();

        if (map.ContainsKey(name))
            return map[name];
        return null;
        //return Shader.Find(name);
#endif

    }
}