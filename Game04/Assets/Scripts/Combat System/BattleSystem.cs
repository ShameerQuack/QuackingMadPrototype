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
    public Canvas dialogueInterface;
    public GameObject redBookMarkContents;
    public GameObject blueBookMarkContents;
    public Canvas choicesInterface;
    public Canvas inventoryInterface;
    public TextMeshProUGUI dialogueText;
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
    private float enemyAttackModifier = 1.0f;

    void Start(){
        state = BattleState.START;
        preIntro();
    }

    void preIntro(){
        if (Mark.bookmarkSelected == BookmarkSelected.RED){
            redBookMarkContents.SetActive(true);
        } else {
            blueBookMarkContents.SetActive(true);
        }
        inventoryInterface.GetComponent<Canvas>().enabled = false;
        choicesInterface.GetComponent<Canvas>().enabled = false;
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
        EnemyAttackIndicatorController.Instance.enableIndicator(0);
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
            enemy.transform.position += new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }

        dialogueText.text = enemyStats.charName + " took " + playerStats.damage + " damage.";
        yield return new WaitForSeconds(3f);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    // Enemy Turn Method
    void EnemyTurn(){
        // Debuff Logic =============
        switch (enemyStats.debuffState){
            case Debuff.BIND:
            enemyAttackModifier = 0.5f;
            break;
        }
        // ==========================
        if(EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 0) {
            StartCoroutine(EnemyAttack0());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 1) {
            StartCoroutine(EnemyAttack1());
        }
    }


    // Method to End Battle
    void EndBattle(){
        if (state == BattleState.WON){
            dialogueText.text = "You defeated the evil Snow White!";
        } else if (state == BattleState.LOST){
            dialogueText.text = "You were defeated.";
            backgroundMusic.Pause();
            GameOverOverlay.Instance.FadeToGameOver();
        }
    }

    // Method to start PlayerTurn
    void PlayerTurn(){
        // Reduces Duration of Buffs and Debuffs
        if (playerStats.buffDuration > 0) {
            playerStats.buffDuration -= 1;
            if (playerStats.buffDuration == 0){playerStats.buffState = Buff.NONE;}
            }
        if (enemyStats.buffDuration > 0) {
            enemyStats.buffDuration -= 1;
            if (enemyStats.buffDuration == 0){enemyStats.buffState = Buff.NONE;}
            }
        if (playerStats.debuffDuration > 0) {
            playerStats.debuffDuration -= 1;
            if (playerStats.debuffDuration == 0){playerStats.debuffState = Debuff.NONE;}
            }
        if (enemyStats.debuffDuration > 0) {
            enemyStats.debuffDuration -= 1;
            if (enemyStats.debuffDuration == 0){enemyStats.debuffState = Debuff.NONE;}
            }
        // =======================================

        dialogueInterface.GetComponent<Canvas>().enabled = false;
        choicesInterface.GetComponent<Canvas>().enabled = true;
        EventSystem.current.SetSelectedGameObject(null);
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
    public void OnInventoryButton(){
        inventoryInterface.GetComponent<Canvas>().enabled = true;
    }

    // Method to close all UI
    void closeAllInterface(){
        choicesInterface.GetComponent<Canvas>().enabled = false;
        dialogueInterface.GetComponent<Canvas>().enabled = false;
        inventoryInterface.GetComponent<Canvas>().enabled = false;
    }

    // Methods for Enemy Actions ===================================================================
    IEnumerator EnemyAttack0(){
        dialogueText.text = enemyStats.charName + " angrily attacks!";
        yield return new WaitForSeconds(3f);
        bool isDead = playerStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier));
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier*0.5))){
                enemyStats.currentHP = 1;
            }
            enemyHUD.SetHP(enemyStats);
        }

        hitSound.Play();
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        // Can use Write Method here-------
        dialogueText.text = " You took " + ((int)(enemyStats.damage*enemyAttackModifier)).ToString() + " damage.";
        yield return new WaitForSeconds(3f);
        enemyAttackModifier = 1;
        if(isDead){
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            int enemyAttackType = Random.Range(0, 2);
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
            PlayerTurn();
        }
    }

    IEnumerator EnemyAttack1(){
        dialogueText.text = enemyStats.charName + " recklessly attacks!";
        int enemyDamage= Random.Range(0, 16);
        yield return new WaitForSeconds(3f);
        bool isDead = playerStats.TakeDamage((int)(enemyDamage*enemyAttackModifier));
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyDamage*enemyAttackModifier*0.5))){
                enemyStats.currentHP = 1;
            }
            enemyHUD.SetHP(enemyStats);
        }

        hitSound.Play();
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        // Can use Write Method here-------
        dialogueText.text = " You took " + ((int)(enemyDamage*enemyAttackModifier)).ToString() + " damage.";
        yield return new WaitForSeconds(3f);
        enemyAttackModifier = 1;
        if(isDead){
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            int enemyAttackType = Random.Range(0, 2);
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
            PlayerTurn();
        }
    }

    // This one isn't used in the Tech Demo so ignore it :D
    IEnumerator EnemyAttack2(){
        dialogueText.text = enemyStats.charName + "Created a barrier";
        yield return new WaitForSeconds(3f);
        enemyStats.AddBarrier(10);
        enemyHUD.SetHP(enemyStats);
        // Can use Write Method here-------
        dialogueText.text = enemyStats.charName + " gained 10 barrier";
        yield return new WaitForSeconds(3f);
        
        state = BattleState.PLAYERTURN;
        int enemyAttackType = Random.Range(0, 2);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
        PlayerTurn();
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
            enemy.transform.position += new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(25f, 0, 0);
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
            EnemyTurn();
        }

    }
    // Method for Mirror Item - Needs Audio (my bad)
    IEnumerator UseMirror(){
        playerStats.applyBuff(Buff.REFLECT, 2);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You used the Mirror's reflective magic...";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = enemyStats.charName + " Looked at the Mirror";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "It seems to have reminded her of something";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "She seems a bit...sad...";
        yield return new WaitForSeconds(3f);
        // I wanna reward the player for using this item by inflicting an additional debuff to Snow White called DEFENSELESS but we'll see to it later
        dialogueInterface.GetComponent<Canvas>().enabled = false;
        playerHUD.SetHP(playerStats);

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You will partially <b>REFLECT</b> damage for two turns";
        yield return new WaitForSeconds(3f);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Cudgel Item - Needs Audio (my bad)
    IEnumerator UseCudgel(){
        bool isDead = enemyStats.TakeDamage(5);

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You lifted the cudgel and hit your opponent";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "Your strength isn't amazing but it did some damage";
        yield return new WaitForSeconds(2.5f);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        // Shaky Effect!
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(25f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + "Took 5 damage";
        yield return new WaitForSeconds(3f);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }


    // Method for Magic Cloth Item - Needs Audio (my bad)
    IEnumerator UseMagicCloth(){
        if (playerStats.currentHP == playerStats.maxHP){
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You used the Magic Cloth";
            yield return new WaitForSeconds(2.5f);
            dialogueText.text = "With no wounds to heal, it had no effect";
            yield return new WaitForSeconds(2.5f);
        } else {
            playerStats.Heal(6);
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You used the Magic Cloth";
            yield return new WaitForSeconds(2.5f);
            dialogueText.text = "Its warmth eased your pain";
            yield return new WaitForSeconds(2.5f);
            playerHUD.SetHP(playerStats);
            dialogueText.text = "You healed 6 Hp";
            yield return new WaitForSeconds(3f);
        }
    
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Silver Hands Item - Needs Audio (my bad)
    IEnumerator UseSilverHands(){
        playerStats.AddBarrier(10);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The Silver Hands of the pure maiden";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "Unfortunately, they're too heavy for you to bear";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "But they offer protection against pain to come";
        yield return new WaitForSeconds(2.5f);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        playerHUD.SetHP(playerStats);
        hitSound.Play();
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You gained 10 Barrier";
        yield return new WaitForSeconds(3f);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Rapunzels Hair Item - Needs Audio (my bad)
    IEnumerator UseRapunzelHair(){
        enemyStats.applyDebuff(Debuff.BIND, 3);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The Magic Hair binds your opponent";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "With movement restricted, her attacks are weakened";
        yield return new WaitForSeconds(2.5f);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + " <b>Bound</b> for 3 turns";
        yield return new WaitForSeconds(3f);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
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

    public void OnCudgelUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseCudgel());

    }

    public void OnMagicClothUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseMagicCloth());

    }

    public void OnSilverHandsUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseSilverHands());

    }

    public void OnRapunzelHairUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseRapunzelHair());

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
