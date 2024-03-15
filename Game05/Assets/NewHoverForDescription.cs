using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHoverForDescription : MonoBehaviour
{
    public GameObject descriptor;
    // public AudioSource scrape;
    public CursorMoment cursorChanger;
    public GameEvent cursorChangeEnterEvent;
    public GameEvent cursorChangeExitEvent;
    public GameObject pointer;

    public void OnMouseOver()
    {
        pointer.SetActive(false);
        descriptor.SetActive(true);
        // scrape.enabled = true;
        cursorChangeEnterEvent.Raise();
        cursorChanger.ChangeCursor();
    }

    public void OnMouseExit()
    {
        descriptor.SetActive(false);
        // scrape.enabled = false;
        cursorChangeExitEvent.Raise();
        cursorChanger.UnchangeCursor();
    }
}
