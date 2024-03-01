using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoop : MonoBehaviour
{
    public float endTime;
    public float loopTime;

    private AudioSource music;
    public AudioSource music2;
    public AudioSource[] startTheseToo;
    public float fade;
    public bool crossFade = false;
    public bool alreadyPlaying = false;
    public bool dontStop = false;
    // Start is called before the first frame update
    void Start()
    {
        music = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (music.time >= endTime)
		{
            music.time = loopTime;
		}
    }

    public void Transition()
	{
        StartCoroutine(CrossFade(music2, fade));
	}

    public void VariableTransition(AudioSource secondSong, float fadeTime)
    {
        StartCoroutine(CrossFade(secondSong, fadeTime));
    }

    public IEnumerator CrossFade(AudioSource audioSource, float fadeTime)
    {
        if (crossFade)
		{
            audioSource.volume = 0;
		}
        if (!alreadyPlaying)
        {
            audioSource.Play();
            foreach (AudioSource these in startTheseToo)
			{
                these.Play();
			}
        }
        float startingVolume = music.volume;
        
        //yield return new WaitForSeconds(fadeTime);
        while (music.volume > 0)
        {
            music.volume -= startingVolume * Time.deltaTime / fadeTime;
            if (crossFade)
			{
                audioSource.volume += 1 * Time.deltaTime / fadeTime;
            }

            yield return null;
        }

        if (!dontStop)
            music.Stop();
    }
}
