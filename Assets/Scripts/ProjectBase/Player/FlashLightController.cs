using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightController : SingletonMono<FlashLightController>
{

    // ����ҽ�ɫ���봥������Χʱ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Debug.Log("������봥������Χʱ����");
        }
    }

    // ����ҽ�ɫ�ڴ�������Χ��ʱ����
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

    // ����ҽ�ɫ�뿪��������Χʱ����
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            Debug.Log("�����뿪��������Χʱ����");
        }
    }
}
