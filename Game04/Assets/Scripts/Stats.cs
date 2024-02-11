using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats: MonoBehaviour
{
    public string charName;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int barrier;
    private int dmgTaken;

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
}
