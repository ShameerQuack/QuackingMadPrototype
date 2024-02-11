﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverOverlay : MonoBehaviour
{
    private static GameOverOverlay _instance;
    public static GameOverOverlay Instance { get { return _instance; } }

    public List<string> gameOverBarks;
    public TextMeshProUGUI bark;
    public Animator gameOverAnimator;

    void Awake(){
        _instance = this;
        string randomBark = gameOverBarks[Random. Range(0, gameOverBarks.Count)];
        bark.text = randomBark;
    }

    public void FadeToGameOver(){
        gameOverAnimator.SetTrigger("GameOver");
    }

    public void OnRetry(){
        SceneChanger.Instance.FadeToSameScene();
    }

    public void OnReturnToTitleScreen(){
        SceneChanger.Instance.FadeToScene(1);
    }
}
