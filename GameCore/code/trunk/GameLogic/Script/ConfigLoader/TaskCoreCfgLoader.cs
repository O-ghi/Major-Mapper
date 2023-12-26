using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCoreCfg
{
    public int ID;
    public string Name;
    public int Type;
    public int PersonalityType;
    public string Scene;
}

public class TaskCoreCfgLoader : ConfigLoaderBase
{
    Dictionary<int, TaskCoreCfg> m_data = new Dictionary<int, TaskCoreCfg>();

    protected override void OnLoad()
    {
        Debug.Log("task_core");
        ReadConfig<TaskCoreCfg>("task_core_cfg", OnReadRow);
    }

    protected override void OnUnload()
    {
        throw new System.NotImplementedException();
    }
    private void OnReadRow(TaskCoreCfg row)
    {
        m_data[row.ID] = row;
    }

    public TaskCoreCfg GetCfg(int id)
    {
        TaskCoreCfg cfg;
        if (m_data.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        return null;
    }
}
