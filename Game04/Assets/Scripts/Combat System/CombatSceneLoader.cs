using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSceneLoader : MonoBehaviour
{
    void OnEnable(){
        SceneManager.LoadScene("Combat Scene");
    }
}
