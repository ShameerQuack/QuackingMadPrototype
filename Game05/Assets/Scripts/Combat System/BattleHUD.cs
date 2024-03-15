using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleHUD : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public bool reverseText;
    public GameObject shieldShower;
    public GameObject actualShield;
    private string barrierText;

    public void SetHUD(Stats unit){
        hpSlider.maxValue = unit.maxHP;
        SetHP(unit);
    }

    public void SetHP(Stats unit)
    {
        hpSlider.value = unit.currentHP;
        if (unit.barrier == 0){
            shieldShower.SetActive(false);
            actualShield.SetActive(false);
            barrierText = "";
        } else
        {
            shieldShower.SetActive(true);
            actualShield.SetActive(true);
            barrierText = unit.barrier.ToString();
        }
        if (reverseText){
            hpText.text = "       </size>" + unit.currentHP + "\n       " + unit.maxHP + " <color=#477ECA> \n\n<size=0.06> " + barrierText + "</color>";
        } 
        else {
            hpText.text = "</size>" + unit.currentHP + "\n" + unit.maxHP + " <color=#477ECA> \n\n       <size=0.06> " + barrierText + "</color>";
        }
    }
}
