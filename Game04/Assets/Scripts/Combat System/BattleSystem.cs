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
    public GameObject redBookMarkContents;
    public GameObject blueBookMarkContents;
    public Canvas choicesInterface;
    public Canvas inventoryInterface;
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

    // Boss Music Transitions
    public int MCstate = 0;
    public AudioSource MC100;
    public MusicLoop MC100Switch;
    public AudioSource MC50;
    public MusicLoop MC50Switch;
    public AudioSource MC25;
    public MusicLoop MC25Switch;

    Stats playerStats;
    Stats enemyStats;
    private float enemyAttackModifier = 1.0f;
    private float playerAttackModifier = 1.0f;
    private float enemyHpMod = 1.0f;
    private float playerHpMod = 1.0f;

    // Item First Use Trackers
    private bool CudgelFirstUse;
    private bool MirrorFirstUse;
    private bool ClothFirstUse;
    private bool CrystalBallFirstUse;
    private bool HatFirstUse;
    private bool VenomFirstUse;
    private bool SilvHandsFirstUse;
    private bool HairFirstUse;
    private bool IronShoesFirstUse;
    private bool AppleFirstUse;
    private bool WisdomFirstUse;

    private Animator playerAnim;
    private Animator enemyAnim;

    public GameObject musicIntro;
    

    void Start(){
        state = BattleState.START;
        CudgelFirstUse = true;
        MirrorFirstUse = true;
        ClothFirstUse = true;
        CrystalBallFirstUse = true;
        HatFirstUse = true;
        VenomFirstUse = true;
        SilvHandsFirstUse = true;
        HairFirstUse = true;
        IronShoesFirstUse = true;
        AppleFirstUse = true;
        WisdomFirstUse = true;
        playerAnim = player.GetComponent<Animator>();
        enemyAnim = enemy.GetComponent<Animator>();
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
    }

    public void Wait()
	{
        state = BattleState.PLAYERTURN;
        StartCoroutine(StartIntro());
    }


    IEnumerator StartIntro(){
        skip.SetActive(false);
        musicIntro.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "My Favourite Kind of Death";
        musicIntro.GetComponent<Animator>().StartPlayback();
        playerHUD.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(true);
        player.GetComponent<Animator>().SetTrigger("Fight");
        PlayerTurn();
        yield return null;
    }

    IEnumerator WaitForDoneProcess(float timeout)
    {
        while (!(Input.GetKey(KeyCode.Mouse1)))
        {
            yield return null;
            timeout -= Time.deltaTime;
            if (timeout <= 0f) break;
        }
    }

    YieldInstruction WaitForDone(float timeout) { return StartCoroutine(WaitForDoneProcess(timeout)); }

    // Player Attack Method
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyStats.TakeDamage(playerStats.damage);
        transitionMusic();

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

        yield return WaitForDone(3f * textSpeed);
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
        // Updates Health-Based Modifiers
        if(playerStats.currentHP <= 0.25*playerStats.maxHP){
            playerHpMod = 3.0f;
        } else if(playerStats.currentHP <= 0.5*playerStats.maxHP){
            playerHpMod = 2.0f;
        } else {
            playerHpMod = 1.0f;
        }
        if(enemyStats.currentHP <= 0.25*enemyStats.maxHP){
            enemyHpMod = 3.0f;
        }else if(enemyStats.currentHP <= 0.5*enemyStats.maxHP){
            enemyHpMod = 2.0f;
        } else {
            enemyHpMod = 1.0f;
        }

        playerAttackModifier = 1;
        // Debuff Logic =============
        switch (enemyStats.debuffState){
            case Debuff.BIND:
            enemyAttackModifier = 0.5f/playerHpMod;
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
            backgroundMusic.Pause();
            winCanvas.SetActive(true);
        } else if (state == BattleState.LOST){
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
        EventSystem.current.SetSelectedGameObject(null);
        OnInventoryButton();
    }

    // Method to write text in dialogue box (Unused right now)
    IEnumerator Write(string text){
        textTransition.Play();
        yield return new WaitForSeconds(2f);
    }

    // Opens Inventory Menu
    public void OnInventoryButton(){
        inventoryInterface.GetComponent<Canvas>().enabled = true;
    }

    // Method to close all UI
    void closeAllInterface(){
        choicesInterface.GetComponent<Canvas>().enabled = false;
        inventoryInterface.GetComponent<Canvas>().enabled = false;
    }

    // Method for Music Transitions UNFINISHED!
    void transitionMusic()
	{
        if (false)
        {
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC100Switch.VariableTransition(MC25, 0.3333f);
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC100Switch.VariableTransition(MC50, 0.3333f);
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC50Switch.VariableTransition(MC100, 0.3333f);
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC50Switch.VariableTransition(MC25, 0.3333f);
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC25Switch.VariableTransition(MC100, 0.3333f);
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC25Switch.VariableTransition(MC50, 0.3333f);
            }
        }
    }

    // Methods for Enemy Actions ===================================================================
    IEnumerator EnemyAttack0(){
        yield return WaitForDone(3f * textSpeed);
        bool isDead = playerStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier*enemyHpMod));
        transitionMusic();
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyStats.damage*enemyAttackModifier*enemyHpMod*playerHpMod*0.5))){
                enemyStats.currentHP = 1;
                transitionMusic();
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
        yield return WaitForDone(3f * textSpeed);
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
        int enemyDamage= Random.Range(0, 13);
        yield return WaitForDone(3f * textSpeed);
        bool isDead = playerStats.TakeDamage((int)(enemyDamage*enemyAttackModifier*enemyHpMod));
        transitionMusic();
        playerHUD.SetHP(playerStats);

        if (playerStats.buffState==Buff.REFLECT){
            if(enemyStats.TakeDamage((int)(enemyDamage*enemyAttackModifier*enemyHpMod*playerHpMod*0.5))){
                enemyStats.currentHP = 1;
                transitionMusic();
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
        yield return WaitForDone(3f * textSpeed);
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
        yield return WaitForDone(3f * textSpeed);
        int barrier = (int)(Random.Range(4, 7) * enemyHpMod);
        enemyStats.AddBarrier(barrier);
        enemyHUD.SetHP(enemyStats);
        // Can use Write Method here-------
        yield return WaitForDone(3f * textSpeed);
        
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
        transitionMusic();

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return WaitForDone(3f * textSpeed);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }
    // Method for Mirror Item - Needs Audio (my bad)
    IEnumerator UseMirror()
    {

        playerAnim.SetTrigger("Mirror");
        yield return new WaitForSeconds(3.267f);
        playerStats.applyBuff(Buff.REFLECT, 3);

        playerHUD.SetHP(playerStats);

        yield return WaitForDone(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();

        reflect.SetActive(true);
    }


    // Method for Cudgel Item - Needs Audio (my bad)
    IEnumerator UseCudgel()
    {

        playerAnim.SetTrigger("Cudgel");
        yield return new WaitForSeconds(3.267f);
        if (playerStats.buffState == Buff.VAMPIRISM){
            if ((int)(5*playerAttackModifier*playerHpMod) > enemyStats.barrier){
                playerStats.Heal((int)(4*playerHpMod));
            }   
        }
        bool isDead = enemyStats.TakeDamage((int)(5*playerAttackModifier*playerHpMod));
        transitionMusic();

        playerHUD.SetHP(playerStats);
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
        yield return WaitForDone(3f * textSpeed);
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }


    // Method for Magic Cloth Item - Needs Audio (my bad)
    IEnumerator UseMagicCloth()
    {

        playerAnim.SetTrigger("Cloth");
        yield return new WaitForSeconds(3.267f);
        if (playerStats.currentHP == playerStats.maxHP){
        } else {
            playerStats.Heal((int)(5*playerHpMod));
            transitionMusic();
            playerHUD.SetHP(playerStats);
        }
    
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Silver Hands Item - Needs Audio (my bad)
    IEnumerator UseSilverHands()
    {

        playerAnim.SetTrigger("Hands");
        playerStats.AddBarrier((int)(6*playerHpMod));

        playerHUD.SetHP(playerStats);
        hitSound.Play();
        yield return WaitForDone(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Rapunzels Hair Item - Needs Audio (my bad)
    IEnumerator UseRapunzelHair()
    {

        playerAnim.SetTrigger("Hair");
        yield return new WaitForSeconds(3.267f);
        enemyStats.applyDebuff(Debuff.BIND, 3);

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        yield return WaitForDone(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // New Items =================================================================
    // Method for Iron Shoes Item
    IEnumerator UseIronShoes()
    {

        playerAnim.SetTrigger("Shoes");
        yield return new WaitForSeconds(3.267f);
        if (playerStats.debuffState != Debuff.NONE){
            enemyStats.applyDebuff(playerStats.debuffState, (int)(playerStats.debuffDuration + playerHpMod - 1));
            // Need to get code for activating debuff intigators
            playerStats.applyDebuff(Debuff.NONE, 0);
        } else
        {

            
        }

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Hat that fires bullets Item
    IEnumerator UseHat()
    {

        playerAnim.SetTrigger("Hat");
        yield return new WaitForSeconds(3.267f);
        int randInt = Random.Range(1, 5);
        bool isDead = false;
        for (int i = 0; i < randInt; i++){
            if (playerStats.buffState == Buff.VAMPIRISM){
                if ((int)(2*playerAttackModifier*playerHpMod) > enemyStats.barrier){
                    playerStats.Heal(4);
                }
            }
            isDead = enemyStats.TakeDamage((int)(2*playerAttackModifier*playerHpMod));
            transitionMusic();
        }

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "The hat fires " + randInt.ToString() + " bullets.";
        yield return WaitForDone(2.5f * textSpeed);
        dialogueText.text = enemyStats.charName + " took " + ((int)(randInt*2*playerAttackModifier*playerHpMod)).ToString() + " damage.";
        yield return WaitForDone(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        playerHUD.SetHP(playerStats);
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
    IEnumerator UseCrystalBall()
    {

        playerAnim.SetTrigger("Ball");
        yield return new WaitForSeconds(3.267f);
        if (playerStats.debuffState != Debuff.NONE)
        {

            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You rub the surface of the crystal ball and feel a strange sensation wash over you.";
            yield return WaitForDone(2.5f * textSpeed);
            dialogueText.text = "Your ailments seem to have disappeared, while your boons feel more enduring.";
            yield return WaitForDone(2.5f * textSpeed);
            if (playerStats.buffState != Buff.NONE){
                playerStats.applyBuff(playerStats.buffState, (int)(playerStats.buffDuration + playerStats.debuffDuration + playerHpMod - 1));
                dialogueText.text = "Your buff duration is increased to " + playerStats.buffDuration.ToString();
                yield return WaitForDone(2.5f * textSpeed);
            }
            playerStats.applyDebuff(Debuff.NONE, 0);
            // Need to get code for handling debuff/buff indicators

        } else {
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "You rub the surface of the crystal ball and feel a strange sensation wash over you.";
            yield return WaitForDone(2.5f * textSpeed);
            dialogueText.text = "But it doesn't seem to be doing anything.";
            yield return WaitForDone(2.5f * textSpeed);
        }

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Apple from Tree of Life Item
    IEnumerator UseApple()
    {

        playerAnim.SetTrigger("Apple");
        yield return new WaitForSeconds(3.267f);
        playerStats.applyBuff(Buff.DMGBOOST, (int)(4 + playerHpMod - 1));
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You ate the apple, and instantly feel a tingling sensation.";
        yield return WaitForDone(2.5f * textSpeed);
        dialogueText.text = "You feel energized, and your weapon feels lighter in your hand.";
        yield return WaitForDone(2.5f * textSpeed);
        dialogueInterface.GetComponent<Canvas>().enabled = false;

        dialogueInterface.GetComponent<Canvas>().enabled = true;
        dialogueText.text = "You gain a <b>DAMAGE BOOST</b> for " + ((int)(4 + playerHpMod - 1)).ToString() + " turns!";
        yield return WaitForDone(3f * textSpeed);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
        // Buff Indicator handling needed here!
    }

    // Method for White Snake Venom Item
    IEnumerator UseVenom()
    {

        playerAnim.SetTrigger("Venom");
        yield return new WaitForSeconds(3.267f);
        bool isDead = playerStats.TakeDamage(3);
        transitionMusic();
        playerHUD.SetHP(playerStats);
        // Shaky Effect
        for ( int i = 0; i < 10; i++){
            player.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            player.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dialogueText.text = " You took 3 damage from the bitter venom.";
        yield return WaitForDone(2f * textSpeed);

        if(isDead){
            dialogueText.text = "Your weakened body couldn't handle the poison...";
            yield return WaitForDone(2f * textSpeed);
            dialogueText.text = "Your consciousness fades...";
            yield return WaitForDone(2f * textSpeed);
            state = BattleState.LOST;
            EnemyAttackIndicatorController.Instance.disableAllIndicators();
            EndBattle();
        } else {
            playerStats.applyBuff(Buff.VAMPIRISM, (int)(4*playerHpMod));
            dialogueInterface.GetComponent<Canvas>().enabled = true;
            dialogueText.text = "The bitterness turns to thirst...";
            yield return WaitForDone(2.5f * textSpeed);
            dialogueText.text = "You gained <b>VAMPIRISM</b> for " + ((int)(4*playerHpMod) - 1).ToString() + " turns!";
            yield return WaitForDone(2.5f * textSpeed);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            // Needs Indicator handling
        }
    }

    // Method for Sack of Knowledge Item
    IEnumerator UseSackOfKnowledge(){
        
        dialogueInterface.GetComponent<Canvas>().enabled = true;
        // Do sack of knowledge UI stuff here
        yield return WaitForDone(2.5f * textSpeed);

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
