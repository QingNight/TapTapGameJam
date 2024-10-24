using qtools.qhierarchy.phelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonManager : SingletonMono<MonManager>
{
    public List<MonsterController> monsters = new List<MonsterController>();

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



}
