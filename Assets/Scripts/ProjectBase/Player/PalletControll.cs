using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum PalletState
{
    Wait,
    Move,
}

public class PalletControll : MonoBehaviour
{
    public GameObject isTrigger;//触发器
    public Transform StartPos;//开始位置
    public Transform EndPos;//结束位置
    public GameObject pallectObj;//平台


    public float MoveSpeed = 5.0f;
    public int Dir = 1; // -1是向起点 是向终点

    public PalletState state = PalletState.Wait;
    public bool PlayrTrigger = false;
    public bool MonsterTrigger = true;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case PalletState.Wait:
                { 

                }
                break;

            case PalletState.Move:
                {
                    Transform target = Dir == 1? EndPos: StartPos;
                    var movDir = Vector3.Normalize(target.localPosition - pallectObj.transform.localPosition);
                    pallectObj.transform.localPosition += movDir * MoveSpeed * Time.deltaTime;

                    if (Mathf.Abs(Vector3.Distance(target.localPosition, pallectObj.transform.localPosition)) <= 0.01f)
                    {
                        Dir *= -1;
                    }

                }
                break;
        }
    }
}
