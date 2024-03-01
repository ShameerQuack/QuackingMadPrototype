using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSound : MonoBehaviour
{
    public bool music = false;
    public float multiplyer = 1.0f;
    private string volumeType = "SoundVolume";

    // Start is called before the first frame update
    void Start()
    {
        if (music)
            volumeType = "MusicVolume";
        foreach (AudioSource sound in gameObject.GetComponents<AudioSource>())
        {
            sound.volume *= PlayerPrefs.GetFloat(volumeType, 1);
		}
    }

    public void JustForPlayButton()
	{
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(volumeType, 1) * multiplyer;
    }
}
