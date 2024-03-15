using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public enum Buff{ NONE, REFLECT, DMGBOOST, VAMPIRISM }
public enum Debuff{ NONE, BIND, STUN }
public class Stats: MonoBehaviour
{

    
    public string charName;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int barrier;
    private int dmgTaken;
    public Buff buffState;
    public int buffDuration;
    public Debuff debuffState;
    public int debuffDuration;

    public Sprite ohNoFellDown;

    // Buffs and Debuffs
    public bool reflect = false;
    public bool dmgBoost = false;
    public bool vampirism = false;
    public bool bind = false;
    public bool stun = false;
    public GameObject buffDisplay;
    public GameObject debuffDisplay;
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI debuffText;

    public int reflectDuration = 2;
    public int dmgBoostDuration = 3;
    public int vampirismDuration = 4;
    public int bindDuration = 3;
    public int stunDuration = 3;

    public int currentReflectDuration = 0;
    public int currentDmgBoostDuration = 0;
    public int currentVampirismDuration = 0;
    public int currentBindDuration = 0;
    public int currentStunDuration = 0;

    // Method for taking damage
    public bool TakeDamage(int dmg){
        dmgTaken = dmg;
        if (barrier > 0){
            barrier -= dmgTaken;
            if (barrier < 0){
                dmgTaken = -1*barrier;
                barrier = 0;
            } else {
                dmgTaken = 0;
            }
        }
        currentHP -= dmgTaken;
        if (currentHP <= 0){
            currentHP = 0;
            return true;
        }
        else {
            return false;
        }
    }

    // Method for healing
    public void Heal(int hpHealed){
        currentHP += hpHealed;
        if (currentHP > maxHP){
            currentHP = maxHP;
        }
    }

    // Method for adding Barrier
    public void AddBarrier(int barrierAdded){
        barrier += barrierAdded;
    }

    // Methods for applying buffs and debuffs
    public void applyBuff(Buff buff, int duration)
    {
        
        buffState = buff;
        buffDuration = duration;
    }
    public void applyDebuff(Debuff debuff, int duration){
        debuffState = debuff;
        debuffDuration = duration;
    }
}
