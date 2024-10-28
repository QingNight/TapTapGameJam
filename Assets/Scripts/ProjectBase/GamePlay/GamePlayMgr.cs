using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayMgr : BaseManager<GamePlayMgr>
{
    List<GameObject> reanimateList = new List<GameObject>();

    public GameObject lastReamnimate = null;

    Vector3 GamePlayStartPos;

    public void Init()
    {
        if (PlayerController.instance != null)
            GamePlayStartPos = PlayerController.instance.transform.position;
        MusicMgr.Instance.ChangeBKValue(0.5f);
        MusicMgr.Instance.ChangeSoundValue(1.0f);
        MusicMgr.Instance.PlayBkMusic("背景音乐");
    }


    //玩家复活重置物品和怪物
    public void PlayerReamnimate()
    {
        MonManager.Instance.ResetAllMonster();


        if (lastReamnimate != null)
        {
            PlayerController.Instance.transform.position = lastReamnimate.transform.position;
        }
        else
        {
            PlayerController.Instance.transform.position = GamePlayStartPos;
        }

        foreach (GameObject go in reanimateList)
        {
            go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        }

    }
    public void RefreshReamnimate(GameObject nextReamnimate)
    {
        MonManager.Instance.SaveAllMonster();
        reanimateList.Add(nextReamnimate);
        lastReamnimate = nextReamnimate;
        nextReamnimate.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }



}
