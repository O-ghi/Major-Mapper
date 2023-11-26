using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RuleListCfg
{
	public List<RuleReferenceInfo> SceneRuleList;
	public RuleReferenceInfo SkillMainList;
	public RuleReferenceInfo SkillActionList;
	public RuleReferenceInfo SkillSphereList;
	public RuleReferenceInfo OtherRuleList;

	//public RuleListCfg()
	//{
	//	SceneRuleList = new List<RuleReferenceInfo>();
	//	SkillMainList = new RuleReferenceInfo();
	//	SkillActionList = new RuleReferenceInfo();
	//	SkillSphereList = new RuleReferenceInfo();
	//	OtherRuleList = new RuleReferenceInfo();

	//}

}
[System.Serializable]
public class RuleReferenceInfo
{
	public string Name;
	public List<string> RuleList;
}

public class ServerListCfg
{
	// 配置表数据
	public string sid;
	public string main_sid;//合服后的id
	public string sname;
	public string s_ip;
	public string s_port;
	public string idx;
	public string open_status;
	public string is_hot;
	public string is_new;
	public string group;

}
public struct ServerListB92
{
	public string msg;
	public ServerListB92Data data;
}
public struct ServerListB92Data
{
	public string ip;
	public List<B92ServerListZone> zones;
	public List<B92ServerListCfg> servers;
	public List<B92NoticeCfg> notices;
}
public struct B92ServerListZone
{
}
public struct B92ServerListCfg
{
	public int id;
	public string name;
	public int zone_id;
	public string wan_ip;
	public int port;
	public int status;
	public int icon;
	public int hide;
	public int ip_white;
	public string open_time;
	public string maintain_notice;
}
public struct B92NoticeCfg
{
	public string title;
	public string content;
	public int order;

}