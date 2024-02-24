using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoop : MonoBehaviour
{
    public float endTime;
    public float loopTime;

    private AudioSource music;
    public AudioSource music2;
    public float fade;
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

    public IEnumerator CrossFade(AudioSource audioSource, float fadeTime)
    {
        audioSource.Play();
        float startingVolume = music.volume;

        yield return new WaitForSeconds(fadeTime);
        while (music.volume > 0)
        {
            music.volume -= startingVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        music.Stop();
    }
}
