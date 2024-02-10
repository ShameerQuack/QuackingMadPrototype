using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject userInterface;
    public Canvas dialogueInterface;
    public Canvas choicesInterface;
    public Canvas inventoryInterface;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public GameObject player;
    public GameObject enemy;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    // Note: Might want to move Audio System to a different script
    public AudioSource textTransition;
    public AudioSource hitSound;
    public AudioSource gunshots;
    public AudioSource backgroundMusic;
    public AudioSource guncock;

    Stats playerStats;
    Stats enemyStats;

    void Start(){
        state = BattleState.START;
        preIntro();
    }

    void preIntro(){
        choicesInterface.GetComponent<Canvas>().enabled = false;
        inventoryInterface.GetComponent<Canvas>().enabled = false;
        playerStats = player.GetComponent<Stats>();
        enemyStats = enemy.GetComponent<Stats>();
        playerHUD.SetHUD(playerStats);
        enemyHUD.SetHUD(enemyStats);

        state = BattleState.PLAYERTURN;
        StartCoroutine(StartIntro());
    }


    IEnumerator StartIntro(){
        dialogueText.text = enemyStats.charName + " looked at you funny.";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "Pop her ass.";
        yield return new WaitForSeconds(3f);
        PlayerTurn();
    }

    // Player Attack Method
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyStats.TakeDamage(playerStats.damage);

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        // Shake Effect upon taking damage (Very nice)
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }

        dialogueText.text = enemyStats.charName + " took " + playerStats.damage + " damage.";
        yield return new WaitForSeconds(3f);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    // Enemy Turn Method
    IEnumerator EnemyTurn(){
        dialogueText.text = enemyStats.charName + " attacks!";
        
        yield return new WaitForSeconds(3f);

        bool isDead = playerStats.TakeDamage(enemyStats.damage);

        playerHUD.SetHP(playerStats);
        hitSound.Play();
        // Shaky Effect
        for ( int i = 0; i < 10; i++)
        {
            player.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        
        // Can use Write Method here-------
        dialogueText.text = " You took " + enemyStats.damage + " damage.";

        yield return new WaitForSeconds(3f);
        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    // Method to End Battle
    void EndBattle(){
        if (state == BattleState.WON){
            dialogueText.text = "You defeated the evil Snow White!";
        } else if (state == BattleState.LOST){
            dialogueText.text = "You were defeated.";
        }
    }

    // Method to start PlayerTurn
    void PlayerTurn(){
        dialogueInterface.GetComponent<Canvas>().enabled = false;
    //    choicesInterface.GetComponent<Canvas>().enabled = true;
        EventSystem.current.SetSelectedGameObject(null);
        inventoryInterface.GetComponent<Canvas>().enabled = true;
    }

    // Method to write text in dialogue box (Unused right now)
    IEnumerator Write(string text){
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = text;
        textTransition.Play();
        yield return new WaitForSeconds(2f);
        dialogueInterface.GetComponent<Canvas>().enabled = false;
    }

    // Opens Inventory Menu
    //public void OnInventoryButton(){
    //    inventoryInterface.GetComponent<Canvas>().enabled = true;
    //}

    // Method to close all UI
    void closeAllInterface(){
        choicesInterface.GetComponent<Canvas>().enabled = false;
        dialogueInterface.GetComponent<Canvas>().enabled = false;
        inventoryInterface.GetComponent<Canvas>().enabled = false;
    }

    // Method for Items -------------------------------------------------------

    // Method for Glock Item
    IEnumerator UseGlock(){
        backgroundMusic.Pause();
        guncock.Play();
        yield return new WaitForSeconds(2f);
        gunshots.Play();
        yield return new WaitForSeconds(22f);
        backgroundMusic.Play();

        bool isDead = enemyStats.TakeDamage(9999999);

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + " took " + 999999 + " damage.";
        yield return new WaitForSeconds(3f);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }
    // Method for Mirror Item - Needs Audio (my bad)
    IEnumerator UseMirror(){
        bool isDead = enemyStats.TakeDamage(299);

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + " Looked at the Mirror";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "It seems to have reminded her of something";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "She seems a bit...sad...";
        yield return new WaitForSeconds(3f);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        // Shaky Effect!
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + "Took 299 Emotional Damage lol";
        yield return new WaitForSeconds(3f);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    //UI Element Methods -----------------------------------------------------------------------------
    public void OnGlockUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseGlock());

    }

    public void OnMirrorUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseMirror());

    }
    
    public void OnAttackButton()
    {
        closeAllInterface();
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(PlayerAttack());
    }
}
