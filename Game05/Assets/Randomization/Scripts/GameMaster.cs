using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Service locator pattern - No setting, only getting
public class GameMaster : MonoBehaviour
{
    private static GameMaster _instance;
    public static GameMaster Instance { get { return _instance;}}

    public BookMarkSelectionManager BookMarkSelectionManager { get; private set; }
    public GameEvent sceneStartEvent;

    void Awake(){
        _instance = this;

        BookMarkSelectionManager = GetComponentInChildren<BookMarkSelectionManager>();
    }

    void Start(){
        sceneStartEvent.Raise();
    }
}