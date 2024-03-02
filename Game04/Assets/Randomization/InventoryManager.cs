using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<Item> items;
    public CrossRoadData crossRoadDataSaveObject;
    public Transform contentTransform;

    void Start(){
        items = crossRoadDataSaveObject.items;
        this.gameObject.transform.GetComponent<Canvas>().enabled = false;
        foreach(Transform item in contentTransform){
            item.gameObject.SetActive(false);
        }
        foreach(Item item in items){
            contentTransform.Find(item.name).gameObject.SetActive(true);
        }
    }
}
