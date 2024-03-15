using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDescriptor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty");
        if (difficulty == 1)
		{
            transform.Find("Text").gameObject.SetActive(false);
            transform.Find("Text (1)").gameObject.SetActive(true);
        }
    }
}
