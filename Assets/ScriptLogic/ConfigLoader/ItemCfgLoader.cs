
using System.Collections.Generic;

public class ItemCfg
{
	public int Id;
	public string Name;
	public string Image;
}

public class ItemCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, ItemCfg> m_data = new Dictionary<int, ItemCfg>();
    protected override void OnLoad()
    {
        ReadConfig<ItemCfg>("item_cfg", OnReadRow); 
    }

    protected override void OnUnload()
    {
        throw new System.NotImplementedException();
    }

    private void OnReadRow(ItemCfg row)
    {
        m_data[row.Id] = row;
    }

    public ItemCfg GetCfg(int id)
    {
        ItemCfg cfg;
        if (m_data.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        return null;
    }
}