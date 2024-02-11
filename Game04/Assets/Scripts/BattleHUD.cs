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
    private string barrierText;

    public void SetHUD(Stats unit){
        hpSlider.maxValue = unit.maxHP;
        SetHP(unit);
    }

    public void SetHP(Stats unit)
    {
        hpSlider.value = unit.currentHP;
        if (unit.barrier == 0){
            barrierText = "";
        } else {
            barrierText = unit.barrier.ToString();
        }
        if (reverseText){
            hpText.text =  "<color=#6BF2F6>" + barrierText + "</color> <size=0.06>" + unit.currentHP + "</size>/" + unit.maxHP;
        } 
        else {
            hpText.text =  "<size=0.06>" + unit.currentHP + "</size>/" + unit.maxHP + "<color=#6BF2F6> " + barrierText + "</color>";
        }
    }
}
