using DG.Tweening;
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


    public float ToEndTime = 3.0f;
    public float ToStartTime = 3.0f;

    public float WaitEndTime = 1.0f;
    public float WaitStartTime = 1.0f;


    public int Dir = 1; // -1是向起点 是向终点

    public PalletState state = PalletState.Wait;
    public Ease ToEndEase;
    public Ease ToStartEase;


    public bool Loop = false;

    public bool PlayrTrigger = false;
    public bool MonsterTrigger = true;



    // Start is called before the first frame update
    void Start()
    {
        if (state == PalletState.Move)
        {
            ToEnd(true);
            isTrigger.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToEnd(bool trigger)
    {
        state = PalletState.Move;
        pallectObj.transform.DOKill();
        pallectObj.transform.DOLocalMove(EndPos.transform.localPosition, ToEndTime).SetEase(ToEndEase).OnComplete(() =>
        {
            if (Loop)
            {
                ToStart(false);
            }
        }).SetDelay(trigger ? 0 : WaitEndTime);
    }
    public void ToStart(bool trigger)
    {
        state = PalletState.Move;
        pallectObj.transform.DOKill();
        pallectObj.transform.DOLocalMove(StartPos.transform.localPosition, ToStartTime).SetEase(ToStartEase).OnComplete(() =>
        {
            if (Loop)
            {
                ToEnd(false);
            }
        }).SetDelay(trigger ? 0 : WaitStartTime);
    }


}
