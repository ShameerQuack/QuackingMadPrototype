using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public GameObject musicSlider;
    public GameObject soundSlider;
    private float currentSound = 1;
    private float currentMusic = 1;

    public AudioSource volumeDown;
    public AudioSource volumeUp;
    // Start is called before the first frame update
    void Start()
    {
        // Update sound volumes
        musicSlider.GetComponent<Slider>().value = Mathf.Round(PlayerPrefs.GetFloat("MusicVolume", 1) * 20.0f) * 0.05f;
        soundSlider.GetComponent<Slider>().value = Mathf.Round(PlayerPrefs.GetFloat("SoundVolume", 1) * 20.0f) *0.05f;
        currentSound = soundSlider.GetComponent<Slider>().value;
        currentMusic = soundSlider.GetComponent<Slider>().value;
    }

    public void MusicSlider()
    {
        // Change music volume
        musicSlider.GetComponent<Slider>().value = Mathf.Round(musicSlider.GetComponent<Slider>().value * 20.0f) * 0.05f;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);

        // Play Volume Up and Volume Down sound effects
        if (musicSlider.GetComponent<Slider>().value < currentMusic)
		{
            volumeDown.Play();
            currentMusic = musicSlider.GetComponent<Slider>().value;
        }
        else if (musicSlider.GetComponent<Slider>().value > currentMusic)
        {
            volumeUp.Play();
            currentMusic = musicSlider.GetComponent<Slider>().value;
        }
    }

    public void SoundSlider()
    {
        // Change sound effects volume
        soundSlider.GetComponent<Slider>().value = Mathf.Round(soundSlider.GetComponent<Slider>().value * 20.0f) * 0.05f;
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.GetComponent<Slider>().value);

        // Play Volume Up and Volume Down sound effects
        if (soundSlider.GetComponent<Slider>().value < currentSound)
        {
            volumeDown.Play();
            currentSound = soundSlider.GetComponent<Slider>().value;
        }
        else if (soundSlider.GetComponent<Slider>().value > currentSound)
        {
            volumeUp.Play();
            currentSound = soundSlider.GetComponent<Slider>().value;
        }
    }
}
