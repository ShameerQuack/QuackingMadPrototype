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

    public bool TakeDamage(int dmg){
        dmgTaken = dmg;
        if (barrier > 0){
            barrier -= dmgTaken;
            if (barrier < 0){
                dmgTaken = barrier;
                barrier = 0;
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
}
