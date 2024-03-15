using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHoverForDescription : MonoBehaviour
{
    public GameObject descriptor;
    public AudioSource scrape;
    public CursorMoment cursorChanger;
    public GameObject pointer;

    public void OnMouseOver()
    {
        pointer.SetActive(false);
        descriptor.SetActive(true);
        scrape.enabled = true;
        cursorChanger.ChangeCursor();
    }

    public void OnMouseExit()
    {
        descriptor.SetActive(false);
        scrape.enabled = false;
        cursorChanger.UnchangeCursor();
    }
}
