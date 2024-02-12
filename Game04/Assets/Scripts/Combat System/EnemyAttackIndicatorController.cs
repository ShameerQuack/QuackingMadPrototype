using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackIndicatorController : MonoBehaviour
{
    private static EnemyAttackIndicatorController _instance;
    public static EnemyAttackIndicatorController Instance { get { return _instance; } }

    public List<Image> attackIndicators;

    private int indicatorEnabled;

    void Awake(){
        _instance = this;
        disableAllIndicators();
    }

    private void setIndicatorComponent(Image indicatorComponent, bool value){
        indicatorComponent.enabled = value;
    }

    public void disableAllIndicators(){
        for(int i=0;i<attackIndicators.Count;i++){
            setIndicatorComponent(attackIndicators[i], false);
        }
    }

    public void enableIndicator(int index){
        setIndicatorComponent(attackIndicators[index], true);
        indicatorEnabled = index;
    }

    public void disableIndicator(int index){
        setIndicatorComponent(attackIndicators[index], false);
    }

    public int getEnabledIndicator(){
        return indicatorEnabled;
    }
}
