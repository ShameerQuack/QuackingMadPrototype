using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Service Locator pattern - NO SETTING, ONLY GETTING
public class CombatMaster : MonoBehaviour
{
    private static CombatMaster _instance;
    public static CombatMaster Instance { get { return _instance;}}

    public GameEvent sceneStartEvent;
    
    void Awake(){
        _instance = this;
    }

    void Start(){
        sceneStartEvent.Raise();
    }
}
