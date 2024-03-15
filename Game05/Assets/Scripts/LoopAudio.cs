using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAudio : MonoBehaviour
{
    public AudioSource audioSource;

    void Start(){
        audioSource.loop = true;
        audioSource.Play();
    }
}
