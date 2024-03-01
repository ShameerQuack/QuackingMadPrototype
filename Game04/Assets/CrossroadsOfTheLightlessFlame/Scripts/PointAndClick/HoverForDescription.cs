using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HoverForDescription : MonoBehaviour
{
    public Image spriteRenderer;
    public AudioSource scrape;

    public void OnMouseOver()
    {
        spriteRenderer.enabled = true;
        scrape.Play();
    }

    public void OnMouseExit()
    {
        spriteRenderer.enabled = false;
    }
}