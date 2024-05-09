using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : SingleTon<GameManager>
{

    /*public bool IsGamePaused
    {
        get
        {
            return pauseCount > 0;
        }
    }
    public void AddPauseCount()
    {
        pauseCount++;
        if(pauseCount > 0)
        {
            Time.timeScale = 0;
        }
    }
    public void SubPauseCount()
    {
        pauseCount--;
        if(pauseCount <= 0)
        {
            Time.timeScale = 1;
        }
    }*/

    /*public UserData UserData 
    {
        get 
        { 
            if (userData == null)
            {
                userData = DataManager.Instance.LoadJsonData<UserData>("UserData");
            }
            return userData; 
        }
        set 
        { 
            userData = value; 
        }
    }*/


    //private Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();

    //private Dictionary<int, SkillHandler> skillHandlerDic = new Dictionary<int, SkillHandler>();


    /*private void Update()
    {
        if (IsGamePaused)
            return;
        UserData.UpdateCoolTime(Time.deltaTime);
        UserData.playerStatus.RestoreMp(Time.deltaTime);
    }

    public void AddMonster(Monster monster)
    {
        int instanceID = monster.GetColliderInstanceID();
        monsterDic.Add(instanceID, monster);
    }
    public void AddBoss(Boss monster)
    {
        int instanceID = monster.GetColliderInstanceID();
        monsterDic.Add(instanceID, monster);
    }
    public void RemoveMonster(Monster monster)
    {
        int instanceID = monster.GetColliderInstanceID();
        monsterDic.Remove(instanceID);
    }*/
/*
    public SkillHandler GetSkillHandler(GameObject skillObject)
    {
        int instanceID = skillObject.GetInstanceID();
        if (!monsterDic.ContainsKey(instanceID))
            AddSkillHandler(skillObject);
        return skillHandlerDic[instanceID];
    }*/
    /*public void AddSkillHandler(GameObject skillObject)
    {
        int instanceID = skillObject.GetInstanceID();
        skillHandlerDic.Add(instanceID, skillObject.GetComponent<SkillHandler>());
    }*/

    /*public Monster[] GetMonsterByInstanceID(int[] InstanceID)
    {
        return InstanceID.Where(k => monsterDic.ContainsKey(k))
                              .Select(k => monsterDic[k])
                              .ToArray();
    }*/

    public void GetPlayerData()
    {
        return;
    }
    public void GetMonsterData()
    {
        return;
    }
    public void GetStatusEffectData()
    {
        return;
    }


}
