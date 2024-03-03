using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMoment : MonoBehaviour
{
    public Texture2D newCursor;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(newCursor, new Vector2(11, 0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
