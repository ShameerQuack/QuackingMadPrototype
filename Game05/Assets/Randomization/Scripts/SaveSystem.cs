using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public struct SaveData {
    public List<string> items;
    public int sceneIndex;
    public string message;
    public bool isAssigned;

    public SaveData(int sceneIndex, List<string> items, string message){
        this.sceneIndex = sceneIndex;
        this.items = items;
        this.message = message;
        isAssigned = true;
    }
}

public class SaveSystem : MonoBehaviour
{
    public CrossRoadData crossRoadDataSaveObject;
    public LoadEvent loadResultEvent;
    private string buildTag = "savedSceneNo";
    private string countTag = "ItemCount";
    private string buffPrefTag = "buff";

    private void saveItems(){
        List<Item> items = crossRoadDataSaveObject.items;
        for(int i=0; i< items.Count; i++){
            PlayerPrefs.SetString("Items" + i, items[i].name);
        }
        PlayerPrefs.SetInt(countTag, items.Count);
    }

    private List<string> loadItems(){
        List<string> items = new List<string>();
        int savedListItemCount = PlayerPrefs.GetInt(countTag);
        for(int i = 0; i < savedListItemCount; i++){
            string itemName = PlayerPrefs.GetString("Items"+i);
            items.Add(itemName);
        }
        return items;
    }

    private void saveScene(int buildIndex){
        PlayerPrefs.SetInt(buildTag, buildIndex);
    }

    private void saveBuff(){
        PlayerPrefs.SetString(buffPrefTag, crossRoadDataSaveObject.message);
    }

    private string loadBuff(){
        return PlayerPrefs.GetString(buffPrefTag);
    }

    private int loadScene(){
        return PlayerPrefs.GetInt(buildTag);
    }

    public void save(){
        saveItems();
        saveScene(SceneManager.GetActiveScene().buildIndex);
        saveBuff();
    }

    public void deleteSave(){
        PlayerPrefs.DeleteKey(buildTag);
        PlayerPrefs.DeleteKey(countTag);
        PlayerPrefs.DeleteKey(buffPrefTag);
    }

    public void load(){
        SaveData saveData;
        if(!PlayerPrefs.HasKey(countTag) || !PlayerPrefs.HasKey(buildTag) || !PlayerPrefs.HasKey(buffPrefTag)) {
            saveData = new SaveData();
        } else {
            List<string> items = loadItems();
            int sceneIndex = loadScene();
            string buff = loadBuff();
            saveData = new SaveData(sceneIndex, items, buff);
        }
        loadResultEvent.Raise(saveData);
    }

}