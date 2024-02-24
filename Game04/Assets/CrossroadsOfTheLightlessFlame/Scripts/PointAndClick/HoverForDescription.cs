using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HoverForDescription : MonoBehaviour
{
    public Image spriteRenderer;

    public void OnMouseOver()
    {
        spriteRenderer.enabled = true;
    }

    public void OnMouseExit()
    {
        spriteRenderer.enabled = false;
    }
}