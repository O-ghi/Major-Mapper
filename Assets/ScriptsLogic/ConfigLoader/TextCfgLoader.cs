using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCfg
{
    public int ID;
    public string Name;
    public int Type;
    public int PersonalityType;
    public string Scene;
}

public class TextCfgLoader : ConfigLoaderBase
{
    Dictionary<int, TextCfg> m_data = new Dictionary<int, TextCfg>();

    protected override void OnLoad()
    {
        Debug.Log("TextCfg");
        ReadConfig<TextCfg>("text_cfg", OnReadRow);
    }

    protected override void OnUnload()
    {
        throw new System.NotImplementedException();
    }
    private void OnReadRow(TextCfg row)
    {
        m_data[row.ID] = row;
    }

    public TextCfg GetCfg(int id)
    {
        TextCfg cfg;
        if (m_data.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        return null;
    }
}
