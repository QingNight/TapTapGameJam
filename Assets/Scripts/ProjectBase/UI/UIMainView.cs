using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIMainView : SingletonMono<UIMainView>
{
    public GameObject go_game;
    public GameObject go_main;
    public GameObject go_mask;


    public GameObject go_esc;
    public GameObject go_die;
    public GameObject go_setting;

    public Button btn_Esc;
    public Button btn_Close;
    public Button btn_Reset;
    public Button btn_BackGame;
    public Button btn_Play;
    public Button btn_Tips;


    public void Start()
    {
        MaskAnim(1, 0, 2.0f, 1.0f);
        AddListener();
    }


    public void AddListener()
    {
        btn_Esc.onClick.RemoveAllListeners();
        btn_Close.onClick.RemoveAllListeners();
        btn_Reset.onClick.RemoveAllListeners();
        btn_BackGame.onClick.RemoveAllListeners();
        btn_Play.onClick.RemoveAllListeners();
        btn_Tips.onClick.RemoveAllListeners();


        btn_Esc.onClick.AddListener(OnEscClick);
        btn_Close.onClick.AddListener(OnCloseClick);
        btn_Reset.onClick.AddListener(OnResetClick);
        btn_BackGame.onClick.AddListener(OnBackGameClick);
        btn_Play.onClick.AddListener(OnPlayClick);
        btn_Tips.onClick.AddListener(OnTipsClick);

    }


    void OnEscClick()
    {
        go_main.SetActive(false);
        go_game.SetActive(true);

        go_esc.SetActive(false);
        go_setting.SetActive(true);
    }
    void OnCloseClick()
    {
        go_main.SetActive(false);
        go_game.SetActive(true);

        go_esc.SetActive(true);
        go_setting.SetActive(false);
    }
    void OnResetClick()
    {
        go_main.SetActive(false);
        go_game.SetActive(true);

        go_esc.SetActive(true);
        go_setting.SetActive(false);

        PlayerController.Instance.Die();
    }
    void OnBackGameClick()
    {

    }
    void OnPlayClick()
    {

    }
    void OnTipsClick()
    {

    }



    public void DieShow(bool isShow)
    {
        go_die.gameObject.SetActive(isShow);
    }

    public void MaskAnim(float startA ,float  endA,float _time = 1.5f, float _Dtime = 0.0f)
    {
        go_mask.transform.GetComponent<Image>().color = new Color(0, 0, 0, startA);
        go_mask.transform.GetComponent<Image>().DOFade(endA, _time).SetDelay(_Dtime);
    }

}
