using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightController : SingletonMono<FlashLightController>
{

    // 当玩家角色进入触发器范围时调用
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Debug.Log("怪物进入触发器范围时调用");
        }
    }

    // 当玩家角色在触发器范围内时调用
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            var monster = collision.transform.GetComponent<MonsterController>();
            if (monster.state == MonsterState.Die || monster.state == MonsterState.ThrowItOut) return;
            monster.ByWead();
            Debug.LogError("Monster Die");
        }
    }

    // 当玩家角色离开触发器范围时调用
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            Debug.Log("怪物离开触发器范围时调用");
        }
    }
}
