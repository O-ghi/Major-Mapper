using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Text;


public class ClientConfig
{
	public int small_client;		// 小包
	public long server_list_version;		// server_list.json 版本号
	public long patch_version;				// 分包 版本号
	public int pack_version;				// 当前分包 版本号 (本地配置有效)
	public string app_version;				// App版本号
	public string min_app_version;			// App强更版本号
}

public static class GameInfoManager
{
	private static PathFormatter m_formatter;// = new PathFormatter();
	private static Dictionary<string, string> m_dicAttibute = new Dictionary<string, string>();

	public static LitJson.JsonData m_dicLocalConfig = new LitJson.JsonData();

	public static bool IsSmallClientCache = false;
	public static void Init()
	{
		m_dicAttibute.Add("app_asset_path", UpdaterUtils.streamingAsset.TrimEnd('/', '\\'));
		m_dicAttibute.Add("persistent_path", UpdaterUtils.persistentDataPath.TrimEnd('/', '\\'));
		m_dicAttibute.Add("os", UpdaterUtils.OS);

		IsSmallClientCache = IsSmallClient;
	}

	public static bool IsSmallClient
	{
		get
		{
			return GetAttibuteBool("small_client");
		}
		set
		{
			string strValue = value ? "1" : "0";
			m_dicLocalConfig["small_client"] = strValue;
			m_dicAttibute["small_client"] = strValue;
		}
	}

	public static bool IsVerifyClient
	{
		get
		{
			return GetAttibuteBool("verify_client");
		}
	}

	#region 根据需求显影部分按钮状态

	private static string[] m_btnArray;
	private static void InitFuncitonBtnName()
	{
		string btnInfo = GetAttibute("function_icon_name");
		if (string.IsNullOrEmpty(btnInfo))
			return;

		m_btnArray = btnInfo.Split(',');
	}
	public static bool IsFunctionOpenCfg(string btnName)
	{
		if (null == m_btnArray)
			return false;

		int index = System.Array.IndexOf(m_btnArray, btnName);
		if (index == -1)
			return false;

		return true;
	}

	public static void PanelChildFalse(string panelName, GameObject obj)
	{
		if (!IsDeal())
			return;

		var panelPathArray = System.Array.FindAll(m_btnArray, t => t.Contains(panelName));
		if (null == panelPathArray)
			return;

		System.Array.ForEach(panelPathArray, t =>
		{
			int index = t.IndexOf("/");
			string path = t.Substring(index + 1);

			var tra = obj.transform.Find(path);
			tra?.gameObject.SetActive(false);
		});
	}

	public static bool IsDeal()
	{
		if (null == m_btnArray)
			return false;

		return true;
	}
	#endregion



	public static long ServerListVersion
	{
		get
		{
			return GetAttibuteLong("server_list_version");
		}
		set
		{
			string strValue = value.ToString();
			m_dicLocalConfig["server_list_version"] = strValue;
			m_dicAttibute["server_list_version"] = strValue;
		}
	}

	public static long PatchVersion
	{
		get
		{
			return GetAttibuteLong("patch_version");
		}
		set
		{
			string strValue = value.ToString();
			m_dicLocalConfig["patch_version"] = strValue;
			m_dicAttibute["patch_version"] = strValue;
		}
	}

	public static int PackVersion
	{
		get
		{
			return GetAttibuteInt("pack_version");
		}
		set
		{
			string strValue = value.ToString();
			m_dicLocalConfig["pack_version"] = strValue;
			m_dicAttibute["pack_version"] = strValue;
		}
	}

	public static string MinAppVersion
	{
		get
		{
			return GetAttibute("min_app_version");
		}
		set
		{
			m_dicLocalConfig["min_app_version"] = value;
			m_dicAttibute["min_app_version"] = value;
		}
	}


	public static string AppVersion
	{
		get
		{
			return GetAttibute("app_version");
		}
		set
		{
			m_dicLocalConfig["app_version"] = value;
			m_dicAttibute["app_version"] = value;
		}
	}


	public static bool LoadClientConfigLocal(string text)
	{
		if (string.IsNullOrEmpty(text))
			return false;

		try
		{
			LitJson.JsonData datas = LitJson.JsonMapper.ToObject(text);
			foreach (var key in datas.Keys)
			{
				LitJson.JsonData data = datas[key];
				string dataValue = data.ToString();
				m_dicLocalConfig[key] = dataValue;
				if (m_formatter == null)
				{
					m_formatter = new PathFormatter();
				}
				string value = string.Format(m_formatter, dataValue, m_dicAttibute);
				m_dicAttibute[key] = value;
			}
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		return false;
	}

	public static ClientConfig LoadClientConfigRemote(string text)
	{
		if (string.IsNullOrEmpty(text))
			return null;

		try
		{
			ClientConfig clientConfig = new ClientConfig();

			LitJson.JsonData datas = LitJson.JsonMapper.ToObject(text);

			var type = typeof(ClientConfig);
			foreach (var key in datas.Keys)
			{
				FieldInfo field = type.GetField(key);
				LitJson.JsonData data = datas[key];
				string dataValue = data.ToString();

				if (FieldInfoFix.IsFieldTypeNull(field) == false)
				{
					GameToolsCore.SetField(field, clientConfig, dataValue);
				}
				else
				{
					m_dicLocalConfig[key] = dataValue;
					if (m_formatter == null)
					{
						m_formatter = new PathFormatter();
					}
					string value = string.Format(m_formatter, dataValue, m_dicAttibute);
					m_dicAttibute[key] = value;
				}
			}

			InitFuncitonBtnName();
			SaveClientConfig();
			return clientConfig;
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}

		return null;
	}

	public static void SaveClientConfig()
	{
		string pathClientConfig = GameInfoManager.GetAttibute("client_config_local");
		_SaveSaveClientConfig(pathClientConfig);
	}

    public static void SaveLocalClientConfig()
    {
        string pathClientConfig = "Assets/StreamingAssets" + "/clientConfig.json";
		_SaveSaveClientConfig(pathClientConfig);
	}

	private static void _SaveSaveClientConfig(string pathClientConfig)
	{
		TextWriter twriter = new StreamWriter(pathClientConfig);
		LitJson.JsonWriter writer = new LitJson.JsonWriter(twriter);
		writer.PrettyPrint = true;

		m_dicLocalConfig.ToJson(writer);
		twriter.Close();

		Debug.LogFormat("save client config:{0}", pathClientConfig);
	}

	public static void SetAttibute(string key, string value)
	{
		m_dicAttibute[key] = value;
	}

    public static void SetAttibuteForSave(string key, string value)
    {
		m_dicAttibute[key] = value;
        m_dicLocalConfig[key] = value;
	}

	public static void SetAttibutes(Dictionary<string, string> dic)
	{
		if(m_formatter == null)
		{
			m_formatter = new PathFormatter();
		}
		foreach (var item in dic)
		{
			string value = string.Format(m_formatter, item.Value, m_dicAttibute);
			m_dicAttibute[item.Key] = value;
		}
	}

	public static string GetAttibute(string key)
	{
		string result = "";

		m_dicAttibute.TryGetValue(key, out result);
		return result;
	}

	public static long GetAttibuteLong(string key)
	{
		string result = GetAttibute(key);
		return UpdaterUtils.LongConvert(result);
	}

	public static int GetAttibuteInt(string key)
	{
		string result = GetAttibute(key);
		return UpdaterUtils.IntConvert(result);
	}

	public static bool GetAttibuteBool(string key, bool defaultValue = false)
	{
		string result = GetAttibute(key);
		return GameConvert.BoolConvert(result, defaultValue);
	}

	public static string GetFormatString(string value)
	{
		if (m_formatter == null)
		{
			m_formatter = new PathFormatter();
		}
		return string.Format(m_formatter, value);
	}

}
