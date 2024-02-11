using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoverForDescription : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void OnMouseOver()
    {
        spriteRenderer.enabled = true;
    }

    public void OnMouseExit()
    {
        spriteRenderer.enabled = false;
    }
}