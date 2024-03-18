using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public GameEvent continueButtonClickedEvent;
    public GameObject continueLockIcon;
    private SaveData saveData;

    public void OnLoadResult(SaveData saveData){
        if( saveData.isAssigned == false) return;
        this.saveData = saveData;
        this.transform.GetComponent<Button>().interactable = true;
        continueLockIcon.SetActive(false);
    }

    public void OnContinueClicked(){
        SceneChanger.Instance.FadeToScene(saveData.sceneIndex);
    }
}
