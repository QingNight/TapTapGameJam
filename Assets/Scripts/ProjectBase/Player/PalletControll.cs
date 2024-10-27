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
    public GameObject isTrigger;//������
    public Transform StartPos;//��ʼλ��
    public Transform EndPos;//����λ��
    public GameObject pallectObj;//ƽ̨


    public float MoveSpeed = 5.0f;
    public int Dir = 1; // -1������� �����յ�

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
