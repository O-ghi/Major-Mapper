using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;


public class GameTools2
{

    public static Type stringType = typeof(System.String);
    public static Type intType = typeof(System.Int32);
    public static Type Int64Type = typeof(System.Int64);
    public static Type DoubleType = typeof(System.Double);
    public static Type SingleType = typeof(System.Single);
    public static Type UInt32Type = typeof(System.UInt32);
    public static Type UInt64Type = typeof(System.UInt64);
    public static Type BooleanType = typeof(System.Boolean);

    //private static long longVal;
    //private static int intVal;
    //private static float floatVal;
    //private static double doubleVal;
    //private static uint uintVal;
    //private static ulong ulongVal;
    //private static bool boolVal;

    public static void SetField(FieldInfo field, object obj, string strVal)
    {
        //Debuger.Log("field.FieldType is: " + field.FieldType);
        //Debuger.Log("field.FieldType 1 is: " + (field.FieldType == typeof(System.Int32)));
        //Debuger.Log("field.FieldType 2 is: " + (GameLauncherCore.ilrtApp.GetType(field.FieldType).ReflectionType == typeof(System.Int32)));
        //Debuger.Log("field.FieldType 3 is: " + (GameLauncherCore.ilrtApp.GetType(field.FieldType).GetType() == typeof(System.Int32)));
        //Debuger.Log("field.FieldType 4 is: " + (GameLauncherCore.ilrtApp.GetType(field.FieldType).TypeForCLR== typeof(System.Int32)));

#if !Main
        Type fieldType = GameLauncherCore.ilrtApp.GetType(field.FieldType).TypeForCLR;
#else
		 Type fieldType= field.FieldType;
#endif
        if (fieldType == stringType)
        {
            //GameTools2.SetFieldInfoValueString(field,obj, strVal);
            field.SetValue(obj, strVal);
        }
        else if (fieldType == intType)
        {
            int intVal;
            System.Int32.TryParse(strVal, out intVal);
            field.SetValue(obj, intVal);

            //GameTools2.SetFieldInfoValue(field,obj, intVal);
        }
        else if (fieldType == Int64Type)
        {
            long longVal;
            System.Int64.TryParse(strVal, out longVal);
            field.SetValue(obj, longVal);

            //GameTools2.SetFieldInfoValue(field,obj, longVal);
        }
        else if (fieldType == SingleType)
        {
            float floatVal;
            System.Single.TryParse(strVal, out floatVal);
            field.SetValue(obj, floatVal);

            //GameTools2.SetFieldInfoValue(field, obj, floatVal);
        }
        else if (fieldType == DoubleType)
        {
            double doubleVal;
            System.Double.TryParse(strVal, out doubleVal);
            field.SetValue(obj, doubleVal);

            //GameTools2.SetFieldInfoValue(field,obj, doubleVal);
        }
        else if (fieldType == UInt32Type)
        {
            uint uintVal;
            System.UInt32.TryParse(strVal, out uintVal);
            field.SetValue(obj, uintVal);

            //GameTools2.SetFieldInfoValue(field, obj, uintVal);
        }
        else if (fieldType == UInt64Type)
        {
            ulong ulongVal;
            System.UInt64.TryParse(strVal, out ulongVal);
            field.SetValue(obj, ulongVal);

            //GameTools2.SetFieldInfoValue(field,obj, ulongVal);
        }
        else if (fieldType == BooleanType)
        {
            bool boolVal;
            System.Boolean.TryParse(strVal, out boolVal);
            field.SetValue(obj, boolVal);

            //GameTools2.SetFieldInfoValue(field, obj, boolVal);
        }
        else
        {
            Debug.LogErrorFormat("Unsupported config type {0}", fieldType);
        }
    }

    public static void SetFieldInfoValueInt(FieldInfo mField, object mTarget, string strVal)
    {
        int intVal = 0;
        System.Int32.TryParse(strVal, out intVal);
        mField.SetValue(mTarget, intVal);

    }
    public static void SetFieldInfoValueString(FieldInfo mField, object mTarget, string strVal)
    {
        mField.SetValue(mTarget, strVal);

    }

    public static void SetFieldInfoValue(FieldInfo mField, object mTarget, object value)
    {
        mField.SetValue(mTarget, value);

    }
    public static void SetPropertyInfoValue(PropertyInfo mField, object mTarget, object value, object[] index)
    {
        mField.SetValue(mTarget, value, index);

    }


    public static void OrderbyItem(ref List<MemberInfo> arr1)
    {
        for (int i = 0; i < arr1.Count; i++)
        {
            for (int j = i + 1; j < arr1.Count; j++)
            {
                if (arr1[j].Name.CompareTo(arr1[i].Name) < 0)
                {
                    //cach trao doi gia tri
                    var tmp = arr1[i];
                    arr1[i] = arr1[j];
                    arr1[j] = tmp;
                }
            }
        }
    }
    /// <summary>
    /// ÉèÖÃÕûÐÍµÄÄ³Ò»Î»
    /// </summary>
    /// <param name="value">±»ÉèÖÃµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒªÉèÖÃµÄÎ»Ë÷Òý</param>
    /// <returns>±»ÉèÖÃºóµÄÖµ</returns>
    public static int BitSet(int value, int index)
    {
        return value | (1 << index);
    }

    /// <summary>
    /// ÇåÀíÕûÐÍµÄÄ³Ò»Î»
    /// </summary>
    /// <param name="value">±»ÇåÀíµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒªÇåÀíµÄÎ»Ë÷Òý</param>
    /// <returns>±»ÇåÀíºóµÄÖµ</returns>
    public static int BitClear(int value, int index)
    {
        return value & ~(1 << index);
    }

    /// <summary>
    /// ²âÊÔÕûÐÍµÄÄ³Ò»Î»ÊÇ·ñ±»ÉèÖÃ
    /// </summary>
    /// <param name="value">±»²âÊÔµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒª²âÊÔµÄÎ»Ë÷Òý</param>
    /// <returns>ÊÇ·ñ±»ÉèÖÃ</returns>
    public static bool BitTest(int value, int index)
    {
        return 0 != (value & (1 << index));
    }

    /// <summary>
    /// ÉèÖÃÕûÐÍµÄÄ³Ò»Î»
    /// </summary>
    /// <param name="value">±»ÉèÖÃµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒªÉèÖÃµÄÎ»Ë÷Òý</param>
    /// <returns>±»ÉèÖÃºóµÄÖµ</returns>
    public static long BitSet(long value, int index)
    {
        return value | (1L << index);
    }

    /// <summary>
    /// ÇåÀíÕûÐÍµÄÄ³Ò»Î»
    /// </summary>
    /// <param name="value">±»ÇåÀíµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒªÇåÀíµÄÎ»Ë÷Òý</param>
    /// <returns>±»ÇåÀíºóµÄÖµ</returns>
    public static long BitClear(long value, int index)
    {
        return value & ~(1L << index);
    }


    /// <summary>
    /// ²âÊÔÕûÐÍµÄÄ³Ò»Î»ÊÇ·ñ±»ÉèÖÃ
    /// </summary>
    /// <param name="value">±»²âÊÔµÄÕûÐÍÖµ</param>
    /// <param name="index">ÐèÒª²âÊÔµÄÎ»Ë÷Òý</param>
    /// <returns>ÊÇ·ñ±»ÉèÖÃ</returns>
    public static bool BitTest(long value, int index)
    {
        return 0 != (value & (1L << index));
    }

}



public class ConfigMessageIgnoreCore
{
    public static Dictionary<string, List<string>> ignoreMessage = new Dictionary<string, List<string>>()
    {
      { "ActiveOverviewCfg", new List<string>(){ "Activity" }},
{ "ArenaRankAwardCfg", new List<string>(){ "ItemAward" }},
{ "BeastMainCfg", new List<string>(){ "CallLimit","BaseProp","SkillList" }},
{ "ChangeHeadCfg", new List<string>(){ "ColourCost" }},
{ "ChangeStrong", new List<string>(){ "Panel" }},
{ "ChatChannelCfg", new List<string>(){ "Channels" }},
{ "ConsumeInfo", new List<string>(){ "Capital","Item" }},
{ "CommonGrowthPropCfg", new List<string>(){ "Prop" }},
{ "CommonGrowthSkillCfg", new List<string>(){ "ActiveID" }},
{ "CommonModelCfg", new List<string>(){ "SkillBag","ActiveSkillBag" }},
{ "CompsiteSoulCfg", new List<string>(){ "NeedSoul" }},
{ "CompositeTicketCfg", new List<string>(){ "NeedItem" }},
{ "CompsiteItemCfg", new List<string>(){ "NeedItem" }},
{ "DragonMainCfg", new List<string>(){ "OpenNeedItem","OpenCondition","BaseProp","DeblockingSkill" }},
{ "DragonPropCfg", new List<string>(){ "UpgradeProp" }},
{ "DropShowCfg", new List<string>(){ "DropID","DefeatBoxID" }},
{ "EffectCfg", new List<string>(){ "OnBeginPlay","OnEndPlay","LocalPosition","LocalRotation","LocalScale","ShowPosition","ShowRotation" }},
{ "FantasySkillCfg", new List<string>(){ "ConsumeItem","OpenItem" }},
{ "FightRecCfg", new List<string>(){ "ProposalEquip1","ProposalEquip2","ProposalEquip3" }},
{ "FishGameCfg", new List<string>(){ "Condition1","Condition2","Condition3","Condition4","Condition5" }},
{ "FunctionShield", new List<string>(){ "PanelName" }},
{ "FunctionShowCfg", new List<string>(){ "ShowModel","HelpType","OpenType" }},
{ "GetresourcesCfg", new List<string>(){ "hyperlinkevent","hyperlinkicons","hyperlinkname" }},
{ "GmShortCut", new List<string>(){ "Key" }},
{ "GodThingsMainCfg", new List<string>(){ "Unlock" }},
{ "GodWeaponAttributeFormulaCfg", new List<string>(){ "UpgradeValue0","UpgradeValue1","UpgradeValue2" }},
{ "GradePropCfg", new List<string>(){ "Prop" }},
{ "GuildBossCfg", new List<string>(){ "ItemRewardLevel","ItemReward" }},
{ "HorcruxCfg", new List<string>(){ "EnchantmentItem","RandomAddRate" }},
{ "InspiritPropCfg", new List<string>(){ "SuitProp" }},
{ "LeaguePriceCfg", new List<string>(){ "Prize","KillerPrize" }},
{ "MapPositionICfg", new List<string>(){ "Position" }},
{ "MapUICfg", new List<string>(){ "porisition" }},
{ "MarryBabyPropCfg", new List<string>(){ "Prop" }},
{ "MarryHeartLockCfg", new List<string>(){ "Prop","ShareProp" }},
{ "MobaPrizeCfg", new List<string>(){ "Prize" }},
{ "PartyChargeCfg", new List<string>(){ "Level","Item" }},
{ "PartyQuestCfg", new List<string>(){ "Level","Item" }},
{ "PartyRankCfg", new List<string>(){ "Rank","Level","Item" }},
{ "PulseOpenCfg", new List<string>(){ "PropAdd" }},
{ "QmshtPropCfg", new List<string>(){ "Prop" }},
{ "RewardCfg", new List<string>(){ "itemID" }},
{ "VipPrivilegeCfg", new List<string>(){ "Privileges","PrivilegeShowFlag" }},
{ "RuneTowerCfg", new List<string>(){ "ItemShow" }},
{ "SceneGoodsCfg", new List<string>(){ "position","rotation","scale" }},
{ "SceneListCfg", new List<string>(){ "ID","BGM","WorldPostion","MapPostion","ShowLanguage" }},
{ "SeasonRingPropCfg", new List<string>(){ "Prop" }},
{ "SectConsumeCfg", new List<string>(){ "ItemID" }},
{ "SectMainCfg", new List<string>(){ "SectIcon","SectCurrency" }},
{ "SectRule", new List<string>(){ "JoinCondition","CompletionCondition" }},
{ "SectTaskCfg", new List<string>(){ "NextTask" }},
{ "ShieldRankCfg", new List<string>(){ "Prop" }},
{ "ShieldSkillCfg", new List<string>(){ "ActiveID" }},
{ "ShopisticaCfg", new List<string>(){ "Use1","Use2","Use3","Use4" }},
{ "SkillCloneCfg", new List<string>(){ "ItemID" }},
{ "SoulAwakeCfg", new List<string>(){ "PropAdd" }},
{ "TaskCollectCfg", new List<string>(){ "ItemID" }},
{ "TaskHuntCfg", new List<string>(){ "MonsterID" }},
{ "TobestrongCfg", new List<string>(){ "Type" }},
{ "TopIconCfg", new List<string>(){ "Addres" }},
{ "TotalStarCfg", new List<string>(){ "Prop" }},
{ "TotalUpgradeCfg", new List<string>(){ "Prop" }},
{ "TrajectoryCfg", new List<string>(){ "trajectoryType","trajectoryParameters" }},
{ "TransferProfessionCfg", new List<string>(){ "NeedItem","GetAttackSkill","RemoveAttackSkill","TaskID","UnlockEquipment","PropAdd","StagePropAdd" }},
{ "TricksMainCfg", new List<string>(){ "Attack","StarLevel","Step","RecommendSect" }},
{ "VipGiftCfg", new List<string>(){ "OnceGift","DayGift" }},
{ "WingRandomPropCfg", new List<string>(){ "AddProp1","AddProp2" }},


    };
}
