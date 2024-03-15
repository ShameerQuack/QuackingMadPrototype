using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class HoverForDescription : MonoBehaviour
{
    public Image spriteRenderer;
    public AudioSource scrape;
    public CursorMoment cursorChanger;
    public bool inBattle = false;
    public bool knowledgeBag = false;
    public GameObject battleDescription;
    public TextMeshProUGUI description;
    public string itemName;
    public string itemDescription;
    public string effect1;
    public int effectChange;
    public string effect2;

    public BattleSystem HPReference;
    public bool damageItem = false;
    public bool disabled = false;
    public bool maxTime = false;
    public GameObject maxFlame;

    public void OnMouseOver()
    {
        if (!disabled)
        {
            if (inBattle)
            {
                int trueChange = (effectChange * (int)HPReference.playerHpMod);
                if (damageItem)
                {
                    trueChange = (int)(HPReference.attackBuff * trueChange * HPReference.playerAttackModifier);
                }
                if (maxTime)
				{
                    trueChange *= 2;
                    maxFlame.SetActive(true);
				}
                Debug.Log("maxTime is " + maxTime);
                itemName = itemName.Replace("@", System.Environment.NewLine);
                itemDescription = itemDescription.Replace("@", System.Environment.NewLine);
                effect1 = effect1.Replace("@", System.Environment.NewLine);
                effect2 = effect2.Replace("@", System.Environment.NewLine);
                description.text = "<b>        " + itemName + "\n                 </b>" + itemDescription + "\n            " + effect1 + "<b>" + trueChange + "</b>" + effect2;
                if (knowledgeBag)
                {
                    description.text = "<b>        " + itemName + "\n                 </b>" + itemDescription + "\n            " + effect1;
                }
                battleDescription.SetActive(true);
            }
            else
                spriteRenderer.enabled = true;
            scrape.Play();
            cursorChanger.ChangeCursor();
        }
    }

    public void OnMouseExit()
    {
        if (inBattle)
		{
            battleDescription.SetActive(false);
            maxFlame.SetActive(false);
        }
        else
            spriteRenderer.enabled = false;
        cursorChanger.UnchangeCursor();
    }
}