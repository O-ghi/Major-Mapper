using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskStepCfg
{
    public int ID;
    public int Step;
    public string Name;
    public string Dialogue;
    public string ItemNeeds;
    public string SubmitNPC;
    public float Points;
}
public class TaskStepCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, Dictionary<int, TaskStepCfg>> m_data = new Dictionary<int, Dictionary<int, TaskStepCfg>>();
    protected override void OnLoad()
    {
        ReadConfig<TaskStepCfg>("task_step_cfg", OnReadRow);

    }

    protected override void OnUnload()
    {
        throw new System.NotImplementedException();
    }

    private void OnReadRow(TaskStepCfg row)
    {
        Dictionary<int, TaskStepCfg> _value;
        if (!m_data.TryGetValue(row.ID, out _value))
        {
            _value = new Dictionary<int, TaskStepCfg>();
            m_data.Add(row.ID, _value);
        }
        _value.Add(row.Step, row);
    }

    public TaskStepCfg GetCfg(int taskCoreId, int id)
    {
        Dictionary<int, TaskStepCfg> _value;
        if (m_data.TryGetValue(taskCoreId, out _value))
        {
            TaskStepCfg cfg;
            if (_value.TryGetValue(id, out cfg))
            {
                return cfg;
            }
        }
        return null;
    }
}
