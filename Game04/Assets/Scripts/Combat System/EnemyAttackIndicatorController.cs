using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackIndicatorController : MonoBehaviour
{
    private static EnemyAttackIndicatorController _instance;
    public static EnemyAttackIndicatorController Instance { get { return _instance; } }

    public List<SpriteRenderer> attackIndicators;

    private int indicatorEnabled;

    void Awake(){
        _instance = this;
        disableAllIndicators();
    }

    public void disableAllIndicators(){
        for(int i=0;i<attackIndicators.Count;i++){
            attackIndicators[i].enabled = false;
        }
    }

    public void enableIndicator(int index){
        attackIndicators[index].enabled = true;
        indicatorEnabled = index;
    }

    public void disableIndicator(int index){
        attackIndicators[index].enabled = false;
    }

    public int getEnabledIndicator(){
        return indicatorEnabled;
    }
}
