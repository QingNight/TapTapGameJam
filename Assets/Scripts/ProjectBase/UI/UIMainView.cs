using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainView : SingletonMono<UIMainView>
{
    public Text txt_die;






    public void DieShow(bool isShow)
    {
        txt_die.gameObject.SetActive(isShow);
    }



}
