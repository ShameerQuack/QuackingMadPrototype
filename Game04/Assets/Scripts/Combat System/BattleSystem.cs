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
    public GameObject bound;
    public GameObject reflect;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public float textSpeed = 1f;
    // Note: Might want to move Audio System to a different script
    public AudioSource textTransition;
    public AudioSource hitSound;
    public AudioSource gunshots;
    public AudioSource backgroundMusic;
    public AudioSource guncock;
    public Sprite playerDown;
    public Sprite enemyDown;
    public GameObject winCanvas;
    public GameObject skip;

    Stats playerStats;
    Stats enemyStats;
    private float enemyAttackModifier = 1.0f;
    private float playerAttackModifier = 1.0f;

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
        EnemyAttackIndicatorController.Instance.enableIndicator(0);
        playerStats = player.GetComponent<Stats>();
        enemyStats = enemy.GetComponent<Stats>();
        playerHUD.SetHUD(playerStats);
        enemyHUD.SetHUD(enemyStats);

        dialogueInterface.GetComponent<Canvas>().enabled = false;
    }

    public void Wait()
	{
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        state = BattleState.PLAYERTURN;
        StartCoroutine(StartIntro());
    }


    IEnumerator StartIntro(){
        skip.SetActive(false);
        dialogueText.text = enemyStats.charName + " looked at you funny.";
        yield return new WaitForSeconds(3f * 1.7778f);
        dialogueText.text = "Don't back down.";
        yield return new WaitForSeconds(3f * 1.7778f);
        player.GetComponent<Animator>().SetTrigger("Fight");
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
        yield return new WaitForSeconds(3f * textSpeed);
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
        playerAttackModifier = 1;
        // Debuff Logic =============
        switch (enemyStats.debuffState){
            case Debuff.BIND:
            enemyAttackModifier = 0.5f;
            break;
        }
        // ==========================
        if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 0) {
            StartCoroutine(EnemyAttack0());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 1) {
            StartCoroutine(EnemyAttack1());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 2)
        {
            StartCoroutine(EnemyAttack2());
        }
    }


    // Method to End Battle
    void EndBattle(){
        if (state == BattleState.WON){
            dialogueText.text = "You defeated Snow White!";
            backgroundMusic.Pause();
            winCanvas.SetActive(true);
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

        // Buff Logic =============
        switch (playerStats.buffState){
            case Buff.DMGBOOST:
            playerAttackModifier = 1.5f;
            break;
        }
        //=========================

        if (!(enemyStats.debuffState == Debuff.BIND))
        {
            bound.SetActive(false);
        }
        if (!(playerStats.buffState == Buff.REFLECT))
        {
            reflect.SetActive(false);
        }
        dialogueInterface.GetComponent<Canvas>().enabled = false;
        //choicesInterface.GetComponent<Canvas>().enabled = true;
        EventSystem.current.SetSelectedGameObject(null);
        OnInventoryButton();
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
        dialogueText.text = enemyStats.charName + " attacked with focused anger!";
        yield return new WaitForSeconds(3f * textSpeed);
        bool isDead = playerStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier));
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier*0.5))){
                enemyStats.currentHP = 1;
            }
            enemyHUD.SetHP(enemyStats);
        }

        enemy.GetComponent<Animator>().SetTrigger("omg punch");
        yield return new WaitForSeconds(0.416667f);
        hitSound.Play();
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        // Can use Write Method here-------
        dialogueText.text = " You took " + ((int)(enemyStats.damage*enemyAttackModifier)).ToString() + " damage.";
        yield return new WaitForSeconds(3f * textSpeed);
        enemyAttackModifier = 1;
        if(isDead){
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            int enemyAttackType = Random.Range(0, 3);
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
            PlayerTurn();
        }
    }

    IEnumerator EnemyAttack1(){
        dialogueText.text = enemyStats.charName + " attacked recklessly!";
        int enemyDamage= Random.Range(0, 13);
        yield return new WaitForSeconds(3f * textSpeed);
        bool isDead = playerStats.TakeDamage((int)(enemyDamage*enemyAttackModifier));
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyDamage*enemyAttackModifier*0.5))){
                enemyStats.currentHP = 1;
            }
            enemyHUD.SetHP(enemyStats);
        }

        enemy.GetComponent<Animator>().SetTrigger("omg punch");
        yield return new WaitForSeconds(0.416667f);
        hitSound.Play();
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        // Can use Write Method here-------
        dialogueText.text = " You took " + ((int)(enemyDamage*enemyAttackModifier)).ToString() + " damage.";
        yield return new WaitForSeconds(3f * textSpeed);
        enemyAttackModifier = 1;
        if(isDead){
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            int enemyAttackType = Random.Range(0, 3);
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
            PlayerTurn();
        }
    }

    // This one isn't used in the Tech Demo so ignore it :D
    IEnumerator EnemyAttack2(){
        dialogueText.text = enemyStats.charName + " created a barrier.";
        yield return new WaitForSeconds(3f * textSpeed);
        int barrier = Random.Range(4, 7);
        enemyStats.AddBarrier(barrier);
        enemyHUD.SetHP(enemyStats);
        // Can use Write Method here-------
        dialogueText.text = enemyStats.charName + " gained " + barrier + " barrier.";
        yield return new WaitForSeconds(3f * textSpeed);
        
        state = BattleState.PLAYERTURN;
        int enemyAttackType = Random.Range(0, 3);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.enableIndicator(enemyAttackType);
        PlayerTurn();
    }


    // Method for Items -----------------------------------------------------------------------------

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
        yield return new WaitForSeconds(3f * textSpeed);
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
        playerStats.applyBuff(Buff.REFLECT, 3);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You used the Mirror's reflective magic...";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = enemyStats.charName + " looked into the Mirror.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "It seems to have reminded her of something.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "She seems a bit... sad...";
        yield return new WaitForSeconds(3f * textSpeed);
        // I wanna reward the player for using this item by inflicting an additional debuff to Snow White called DEFENSELESS but we'll see to it later
        dialogueInterface.GetComponent<Canvas>().enabled = false;
        playerHUD.SetHP(playerStats);

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You will partially <b>REFLECT</b> damage for two turns!";
        yield return new WaitForSeconds(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();

        reflect.SetActive(true);
    }


    // Method for Cudgel Item - Needs Audio (my bad)
    IEnumerator UseCudgel(){
        bool isDead = enemyStats.TakeDamage((int)(5*playerAttackModifier));

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You lifted the cudgel and hit your opponent.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "Your strength isn't amazing, but it did some damage.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        enemy.GetComponent<Animator>().enabled = false;
        // Shaky Effect!
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        enemy.GetComponent<Animator>().enabled = true;
        if (enemyStats.currentHP <= 0)
        { enemy.GetComponent<Animator>().enabled = false;
            enemy.GetComponent<SpriteRenderer>().sprite = enemyDown; }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + " took 5 damage.";
        yield return new WaitForSeconds(3f * textSpeed);
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
            dialogueText.text = "You used the Magic Cloth.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "With no wounds to heal, it had no effect.";
            yield return new WaitForSeconds(2.5f * textSpeed);
        } else {
            playerStats.Heal(8);
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You used the Magic Cloth.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "Its warmth eased your pain.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            playerHUD.SetHP(playerStats);
            dialogueText.text = "You healed 8 Hp.";
            yield return new WaitForSeconds(3f * textSpeed);
        }
    
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Silver Hands Item - Needs Audio (my bad)
    IEnumerator UseSilverHands(){
        playerStats.AddBarrier(6);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The Metal Hands of the pure maiden.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "Unfortunately, they're too heavy for you to bear,";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "but they offer protection against pain to come.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        playerHUD.SetHP(playerStats);
        hitSound.Play();
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You gained 6 Barrier.";
        yield return new WaitForSeconds(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Rapunzels Hair Item - Needs Audio (my bad)
    IEnumerator UseRapunzelHair(){
        enemyStats.applyDebuff(Debuff.BIND, 3);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The Magic Hair binds your opponent.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = "With movement restricted, her attacks are weakened.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = enemyStats.charName + " is <b>Bound</b> for 3 turns.";
        yield return new WaitForSeconds(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // New Items =================================================================
    // Method for Iron Shoes Item
    IEnumerator UseIronShoes(){
        if (playerStats.debuffState != Debuff.NONE){
            enemyStats.applyDebuff(playerStats.debuffState, playerStats.debuffDuration);
            // Need to get code for activating debuff intigators
            playerStats.applyDebuff(Debuff.NONE, 0);
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "Envy pours from the shoes...";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "Your ailment is cast upon your opponent.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = enemyStats.charName + "'s debuff duration is increased to " + enemyStats.debuffDuration.ToString();
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueInterface.GetComponent<Canvas>().enabled = false;
        } else {
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "The shoes are snug...";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "But they don't seem to be doing anything.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueInterface.GetComponent<Canvas>().enabled = false;
        }

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Hat that fires bullets Item
    IEnumerator UseHat(){
        int randInt = Random.Range(1, 5);
        bool isDead = false;
        for (int i = 0; i < randInt; i++){
            isDead = enemyStats.TakeDamage((int)(2*playerAttackModifier));
        }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The hat fires " + randInt.ToString() + " bullets.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = enemyStats.charName + " took " + ((int)(randInt*2*playerAttackModifier)).ToString() + " damage.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        enemy.GetComponent<Animator>().enabled = false;
        // Shaky Effect!
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        enemy.GetComponent<Animator>().enabled = true;
        if (enemyStats.currentHP <= 0)
        { enemy.GetComponent<Animator>().enabled = false;
            enemy.GetComponent<SpriteRenderer>().sprite = enemyDown; }
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    // Method for Crystal Ball Item
    IEnumerator UseCrystalBall(){
        if (playerStats.debuffState != Debuff.NONE){
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "The Crystal Ball lit up...";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "Your ailment is alleviated.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            if (playerStats.buffState != Buff.NONE){
                dialogueText.text = "The malady turns into your strength.";
                yield return new WaitForSeconds(2.5f * textSpeed);
                playerStats.applyBuff(playerStats.buffState, playerStats.buffDuration + playerStats.debuffDuration);
                dialogueText.text = "Your buff duration is increased to " + playerStats.buffDuration.ToString();
                yield return new WaitForSeconds(2.5f * textSpeed);
            }
            dialogueInterface.GetComponent<Canvas>().enabled = false;
            playerStats.applyDebuff(Debuff.NONE, 0);
            // Need to get code for handling debuff/buff indicators

        } else {
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You try using the Crystal Ball...";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "But it doesn't seem to be doing anything.";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueInterface.GetComponent<Canvas>().enabled = false;
        }

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Apple from Tree of Life Item
    IEnumerator UseApple(){
        playerStats.applyBuff(Buff.DMGBOOST, 4);
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You ate the Apple from the Tree of Life.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueText.text = enemyStats.charName + "A newfound strength wells up inside you.";
        yield return new WaitForSeconds(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You gain a <b>DAMAGE BOOST</b> for three turns!";
        yield return new WaitForSeconds(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
        // Buff Indicator handling needed here!
    }

    // Method for White Snake Venom Item
    IEnumerator UseVenom(){
        bool isDead = playerStats.TakeDamage(3);
        playerHUD.SetHP(playerStats);
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueText.text = " You took 3 damage from the bitter venom.";
        yield return new WaitForSeconds(2f * textSpeed);

        if(isDead){
            dialogueText.text = "Your weakened body couldn't handle the poison...";
            yield return new WaitForSeconds(2f * textSpeed);
            dialogueText.text = "Your consciousness fades...";
            yield return new WaitForSeconds(2f * textSpeed);
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            playerStats.applyBuff(Buff.VAMPIRISM, 4);
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "The bitterness turns to thirst...";
            yield return new WaitForSeconds(2.5f * textSpeed);
            dialogueText.text = "You gained <b>VAMPIRISM</b> for three turns!";
            yield return new WaitForSeconds(2.5f * textSpeed);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            // Needs Indicator handling
        }
    }

    // Method for Sack of Knowledge Item
    IEnumerator UseSackOfKnowledge(){
        
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        // Do sack of knowledge UI stuff here
        yield return new WaitForSeconds(2.5f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }
    // New Items =================================================================

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

    // New Items=================================================
    public void OnIronShoesUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseIronShoes());

    }

    public void OnHatUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseHat());

    }

    public void OnCrystalBallUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseCrystalBall());

    }

    public void OnAppleUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseApple());

    }

    public void OnVenomUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseVenom());

    }

    public void OnSackOfKnowledgeUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseSackOfKnowledge());

    }
    // New Items=================================================

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
