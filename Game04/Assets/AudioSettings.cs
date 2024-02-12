using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public GameObject musicSlider;
    public GameObject soundSlider;
    // Start is called before the first frame update
    void Start()
    {
        // Update sound volumes
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume", 1);
        soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume", 1);


    }

    public void MusicSlider()
    {
        // Change music volume
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);
    }

    public void SoundSlider()
    {
        // Change sound effects volume
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.GetComponent<Slider>().value);
    }
}
