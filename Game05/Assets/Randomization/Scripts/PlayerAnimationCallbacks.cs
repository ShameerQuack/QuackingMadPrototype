using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationCallbacks : MonoBehaviour
{
    public GameEvent rummageEvent;
    public GameEvent eatAppleStartEvent;
    public GameEvent clothEvent;
    public GameEvent healEvent;
    public GameEvent crystalBallShineEvent;
    public GameEvent cudgelGrabEvent;
    public GameEvent cudgelThrowStartEvent;
    public GameEvent cudgelThrowHitEvent;
    public GameEvent metalHandsWearStartEvent;
    public GameEvent hatUnloadEvent1;

    public void OnRummage(){
        rummageEvent.Raise();
    }

    public void OnEatAppleStart(){
        eatAppleStartEvent.Raise();
    }

    public void OnClothStart(){
        clothEvent.Raise();
    }

    public void OnHealStart(){
        healEvent.Raise();
    }

    public void OnCrystalBallShine(){
        crystalBallShineEvent.Raise();
    }

    public void OnCudgelGrabStart(){
        cudgelGrabEvent.Raise();
    }

    public void OnCudgelThrowStart(){
        cudgelThrowStartEvent.Raise();
    }

    public void OnCudgelThrowHit(){
        cudgelThrowHitEvent.Raise();
    }

    public void OnMetalHandsWearStart(){
        metalHandsWearStartEvent.Raise();
    }

    public void OnHatUnloadStart1(){
        hatUnloadEvent1.Raise();
    }
}
