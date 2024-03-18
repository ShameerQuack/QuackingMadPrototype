using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string name;

    public ItemData(Item item){
        this.name = item.name;
    }
}
