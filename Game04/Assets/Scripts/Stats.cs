using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats: MonoBehaviour
{
    public string charName;
    public int damage;
    public int maxHP;
    public int currentHP;

    public bool TakeDamage(int dmg){
        currentHP -= dmg;
        if (currentHP <= 0){
            return true;
        }
        else {
            return false;
        }
    }
}
