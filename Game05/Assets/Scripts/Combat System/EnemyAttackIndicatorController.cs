using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackIndicatorController : MonoBehaviour
{
    private static EnemyAttackIndicatorController _instance;
    public static EnemyAttackIndicatorController Instance { get { return _instance; } }

    public List<Animator> attackIndicators;

    public Transform particles;
    public GameObject pointer;
    public bool particleManager = false;

    private int indicatorEnabled;

    void Awake(){
        _instance = this;
        disableAllIndicators();
        if (particleManager)
            ResetParticles();
    }

    public void disableAllIndicators(){
        pointer.SetActive(false);
        for(int i=0;i<attackIndicators.Count;i++){
            attackIndicators[i].SetTrigger("Disappear");
        }
    }

    public void enableIndicator(int index){
        attackIndicators[index].ResetTrigger("Disappear");
        attackIndicators[index].SetTrigger("Appear");
        indicatorEnabled = index;
    }

    public void disableIndicator(int index)
    {
        pointer.SetActive(false);
        attackIndicators[index].ResetTrigger("Appear");
        attackIndicators[index].SetTrigger("Disappear");
    }

    public int getEnabledIndicator(){
        return indicatorEnabled;
    }

    public void ActivateParticles(string attack, string amount)
	{
        if (particleManager)
            particles.Find(attack).Find(amount).gameObject.SetActive(true);
	}

    public void DeactivateParticles(string attack, string amount)
    {
        if (particleManager)
            particles.Find(attack).Find(amount).gameObject.SetActive(false);
    }

    public void ResetParticles()
	{
        if (particleManager)
        {
            foreach (Transform attack in particles)
            {
                foreach (Transform amount in attack)
                {
                    amount.gameObject.SetActive(false);
                }
            }
        }
	}

    public void TurnOnSprites()
	{
        for (int i = 0; i < attackIndicators.Count; i++)
        {
            attackIndicators[i].gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
