using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public AudioSource scrapeSFX;
    public AudioSource talkingSFX;
    public AudioSource dialogueClickedSFX;
    public AudioSource dialogueSkipSFX;
    public AudioSource sceneTransitionSFX;
    public AudioSource rummageStartSFX;
    public AudioSource eatAppleSFX;
    public AudioSource clothSFX;
    public AudioSource playerHealSFX;
    public AudioSource crystalBallShineSFX;
    public AudioSource cudgelGrabSFX;
    public AudioSource cudgelSwooshSFX;
    public AudioSource cudgelHitSFX;
    public AudioSource metalHandsWearStartSFX;
    public AudioSource reload1SFX;
    public AudioSource bulletShells1SFX;
    public AudioSource bulletShot1SFX;
    public AudioSource bulletHitSFX;

    void Start(){
        foreach (Transform childTransform in this.transform.Find("SoundEffects").transform)
        {
            AudioSource sound = childTransform.GetComponent<AudioSource>();
            sound.volume *= PlayerPrefs.GetFloat("SoundVolume", 1);
		}
    }

    public void OnCursorChangedEnter(){
        scrapeSFX.enabled = true;
    }

    public void OnCursorChangedExit(){
        scrapeSFX.enabled = false;
    }

    public void OnTalkingStart(){
        talkingSFX.Play();
    }

    public void OnTalkingStop(){
        talkingSFX.Stop();
    }

    public void OnDialogueClicked(){
        dialogueClickedSFX.Play();
    }

    public void OnDialogueSkip(){
        dialogueSkipSFX.Play();
    }

    public void OnSceneTransitionStart(){
        sceneTransitionSFX.Play();
    }

    public void OnRummageStart(){
        rummageStartSFX.Play();
    }

    public void OnEatAppleStart(){
        eatAppleSFX.Play();
    }

    public void OnClothStart(){
        clothSFX.Play();
    }

    public void OnPlayerHeal(){
        playerHealSFX.Play();
    }

    public void OnCrystalBallShine(){
        crystalBallShineSFX.Play();
    }

    public void OnCudgelGrabStart(){
        cudgelGrabSFX.Play();
    }

    public void OnCudgelThrowStart(){
        cudgelSwooshSFX.Play();
    }

    public void OnCudgelThrowHit(){
        cudgelSwooshSFX.Stop();
        cudgelHitSFX.Play();
    }

    public void OnMetalHandsWearStart(){
        metalHandsWearStartSFX.Play();
    }

    public void OnHatUnloadStart1(){
        reload1SFX.Play();
        bulletShells1SFX.Play();
        bulletShot1SFX.Play();
        bulletHitSFX.Play();
        Debug.Log("Bullet1Fired");
    }
}
