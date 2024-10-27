using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletTrigger : MonoBehaviour
{
    public PalletControll controll;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((controll.PlayrTrigger && other.transform.tag == "Player") || (controll.MonsterTrigger && other.transform.tag == "Monster"))
        {
            controll.ToEnd(true);
            this.gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if ((controll.PlayrTrigger && other.transform.tag == "Player") || (controll.MonsterTrigger && other.transform.tag == "Monster"))
        {
            controll.ToEnd(true);
            this.gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((controll.PlayrTrigger && other.transform.tag == "Player") || (controll.MonsterTrigger && other.transform.tag == "Monster"))
        {
            controll.ToEnd(true);
            this.gameObject.SetActive(false);
        }
    }
}
