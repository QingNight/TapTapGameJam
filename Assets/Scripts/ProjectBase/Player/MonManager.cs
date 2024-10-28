using qtools.qhierarchy.phelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonManager : BaseManager<MonManager>
{
    public List<MonsterController> monsters = new List<MonsterController>();


    public Dictionary<MonsterController,MonsterSaveData> monsterSaveData = new Dictionary<MonsterController,MonsterSaveData>();

    public MonsterController GetWeadMonster(Transform tar)
    {
        foreach (var obj in monsters) 
        {
            if (obj.state == MonsterState.Weak)
            {
                float dis = Vector2.Distance(obj.gameObject.transform.position, tar.position);
                Debug.LogError($"{dis}");
                if (dis < 2.0f)
                {
                    return obj;
                }
            }
        }

        return null;
    }
    //保存所有怪物数据
    public void SaveAllMonster()
    {
        monsterSaveData.Clear();
        foreach (var obj in monsters)
        {
            monsterSaveData.Add(obj, new MonsterSaveData(obj));
        }
    }
    //重置所有怪物数据
    public void ResetAllMonster()
    {
        foreach (var obj in monsterSaveData)
        {
            obj.Key.state = obj.Value.state;
            obj.Key.transform.position = obj.Value.position;
        }
    }

    public void ClearData()
    {
        monsters.Clear();
        monsterSaveData.Clear();
    }


}
public class MonsterSaveData
{ 
    public MonsterState state;
    public Vector3 position;


    public MonsterSaveData(MonsterController Obj)
    {
        state = Obj.state;
        position = Obj.transform.position;
    }


}


