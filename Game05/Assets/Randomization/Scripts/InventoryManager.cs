using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<Item> items;
    public CrossRoadData crossRoadDataSaveObject;
    public Transform contentTransform;
    public GameEvent loadRequestEvent;
    public GameEvent saveRequestEvent;
    public string passiveBuff;

    void Start(){
        this.gameObject.transform.GetComponent<Canvas>().enabled = false;
        loadRequestEvent.Raise();
    }

    public void OnLoadResult(SaveData saveData){
        foreach(Transform item in contentTransform){
            item.gameObject.SetActive(false);
        }
        foreach(string itemName in saveData.items){
            contentTransform.Find(itemName).gameObject.SetActive(true);
        }
        passiveBuff = saveData.message;
        saveRequestEvent.Raise();
    }
}
