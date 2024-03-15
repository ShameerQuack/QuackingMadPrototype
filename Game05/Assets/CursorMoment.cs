using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMoment : MonoBehaviour
{
    public Texture2D newCursor;
    public Texture2D changeCursor;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(newCursor, new Vector2(11, 0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    public void ChangeCursor()
	{
        Cursor.SetCursor(changeCursor, new Vector2(50, 34), CursorMode.ForceSoftware);
    }

    public void UnchangeCursor()
    {
        Cursor.SetCursor(newCursor, new Vector2(11, 0), CursorMode.ForceSoftware);
    }
}
