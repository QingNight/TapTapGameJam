using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : SingletonMono<MonsterController>
{

    // 定义私有变量rb，类型为Rigidbody2D，用于存储刚体组件的引用
    private Rigidbody2D rb;
    // 定义私有变量coll，类型为BoxCollider2D，用于存储碰撞体组件的引用
    private BoxCollider2D coll;


    // Start方法在脚本实例化后、第一帧更新前被调用
    private void Start()
    {
        // 获取并赋值当前GameObject上的Rigidbody2D组件到rb变量
        rb = GetComponent<Rigidbody2D>();
        // 获取并赋值当前GameObject上的BoxCollider2D组件到coll变量
        coll = GetComponent<BoxCollider2D>();
        // 获取并赋值当前GameObject上的Animator组件到anim变量
        //anim = GetComponent<Animator>();
    }

    public void PickUp( Transform transform)
    {
        this.transform.parent = transform;
        rb.simulated = false;

        this.transform.localPosition = Vector3.zero;
    }


    public void ThrowItOut(float HSpeed,float VSpeed)
    {
        Debug.LogError($"NowSpeed:{HSpeed}");
        rb.simulated = true;
        rb.velocity = new Vector2(HSpeed, VSpeed);

    }




}
