using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Narrative;
using UnityEngine.Video;

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
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
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
    private AudioSource currentSong;
    public int MCstate = 0;
    public AudioSource MC100;
    public MusicLoop MC100Switch;
    public AudioSource MC50;
    public MusicLoop MC50Switch;
    public AudioSource MC25;
    public MusicLoop MC25Switch;
    public AudioSource MC100SW50;
    public MusicLoop MC100SW50Switch;
    public AudioSource MC50SW50;
    public MusicLoop MC50SW50Switch;
    public AudioSource MC25SW50;
    public MusicLoop MC25SW50Switch;
    public AudioSource MC100SW25;
    public MusicLoop MC100SW25Switch;
    public AudioSource MC50SW25;
    public MusicLoop MC50SW25Switch;
    public AudioSource MC25SW25;
    public MusicLoop MC25SW25Switch;
    public float bpm = 180;
    public float beatTime;

    // For Hat
    public ParticleSystem bullets;
    public AudioSource[] impacts;
    public AudioSource[] shells;
    public AudioSource[] yelps;

    Stats playerStats;
    Stats enemyStats;
    private float enemyAttackModifier = 1.0f;
    public float playerAttackModifier = 1.0f;
    public float attackBuff = 1.0f;
    private float enemyHpMod = 1.0f;
    public float playerHpMod = 1.0f;
    public int maxEnemyShield = 16;

    public InventoryManager buffLocation;
    public int mirrorCooldown = 3;
    public int clothCooldown = 2;
    public int ballCooldown = 2;
    public int appleCooldown = 4;
    public int venomCooldown = 5;
    public int handsCooldown = 2;
    public int hairCooldown = 4;
    public int mirrorRestore = 0;
    public int clothRestore = 0;
    public int ballRestore = 0;
    public int appleRestore = 0;
    public int venomRestore = 0;
    public int handsRestore = 0;
    public int hairRestore = 0;
    public GameObject mirrorObject;
    public GameObject clothObject;
    public GameObject shoesObject;
    public GameObject ballObject;
    public GameObject appleObject;
    public GameObject venomObject;
    public GameObject handsObject;
    public GameObject hairObject;
    public bool noCooldowns = false;
    public bool immune = false;

    public EnemyAttackIndicatorController secondAttack;
    public EnemyAttackIndicatorController thirdAttack;
    private List<int> doneAttacks = new List<int>(new int[] { });
    private List<int> doubleDoneAttacks = new List<int>(new int[] { });
    private int attackAmount = 0;

    private int reflectMax = 1;
    private int dmgBoostMax = 1;
    private int vampirismMax = 1;
    private int bindMax = 1;
    private int stunMax = 1;

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
    public AnimationClip angry1;
    public AnimationClip angry2;
    public Animator rootsAnim;

    public GameObject miniNarrator;
    public GameObject pointer;

    private int phase = 1;
    public GameObject x2Text;
    public GameObject x3Text;
    public TextMeshProUGUI playerDamageText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI enemyDamageText;
    public TextMeshProUGUI enemyDamageText2;
    private int shieldCounter = 0;
    private int birdCounter = 0;
    private int catCounter = 0;
    private int rootsCounter = 0;
    private int punchCounter = 0;
    private int attackVariants = 4;
    private List<int> introAttacks = new List<int>(new int[] {0, 1, 2, 3});

    public GameObject startParticles;
    public GameObject phase2Particles;
    public GameObject phase3Particles;
    public Animator windEffect;
    public GameObject windParticles;
    public Animator phaseTransition;
    public Animator thacamra;

    public bool badMusicSync = false;
    public float startVolume = 1;
    public MidBattleDialogueSystem phaseChange;
    private bool talking = false;
    private int maxAttack;
    private bool maxShield = false;

    public AudioSource theDie;
    public AudioSource gameOver;
    public VideoPlayer increaseMusicSpeed;

    void Start(){
        state = BattleState.START;
        beatTime = (1 / bpm) * 60;
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
        currentSong = MC100;
        startVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        Debug.Log(startVolume);
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
        int difficulty = PlayerPrefs.GetInt("Difficulty");
        if (difficulty == 1)
        {
            enemyStats.maxHP = 30;
            enemyStats.currentHP = 30;
            maxEnemyShield = 12;
            enemyStats.damage = 4;
        }
        else
        {
            enemyStats.maxHP = 40;
            enemyStats.currentHP = 40;
            maxEnemyShield = 16;
            enemyStats.damage = 6;
        }
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
        miniNarrator.SetActive(true);
        enemy.GetComponent<Animator>().SetTrigger("Fine Again");
        windParticles.SetActive(true);
        var windMain = windParticles.GetComponent<ParticleSystem>().main;
        windMain.simulationSpeed = 1;
        yield return new WaitForSeconds(10.6667f);
        if (buffLocation.passiveBuff == "AttackSet")
        {
            immune = true;
        }
        if (buffLocation.passiveBuff == "DefenseSet")
        {
            attackBuff = 1.5f;
        }
        if (buffLocation.passiveBuff == "BalanceSet")
        {
            playerStats.maxHP = (int)(playerStats.maxHP * 1.5f);
            playerStats.currentHP = playerStats.maxHP;
        }
        if (buffLocation.passiveBuff == "ControlSet")
        {
            noCooldowns = true;
        }
        playerHUD.SetHUD(playerStats);
        miniNarrator.SetActive(false);
        playerHUD.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(true);
        player.GetComponent<Animator>().SetTrigger("Fight");
        PlayerTurn();
        EnemyAttackIndicatorController.Instance.TurnOnSprites();
        secondAttack.TurnOnSprites();
        thirdAttack.TurnOnSprites();
        int firstAttack = Random.Range(0, 4);
        EnemyAttackIndicatorController.Instance.enableIndicator(firstAttack);
        DoTheMario(firstAttack);
        pointer.SetActive(true);
        startParticles.SetActive(true);
        windEffect.enabled = true;
        windMain = windParticles.GetComponent<ParticleSystem>().main;
        windMain.simulationSpeed = 3;
        if (playerStats.currentHP <= playerStats.maxHP * 0.25f)
        {
            MCstate = 2;
            currentSong = MC25;
        }
        else if (playerStats.currentHP <= playerStats.maxHP * 0.5f)
        {
            MCstate = 1;
            currentSong = MC50;
        }
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
        if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
            StartCoroutine(Phase1MusicQuickStop());

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
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    IEnumerator BePatient(float talkTime)
	{
        talking = true;
        yield return new WaitForSeconds(talkTime);
        talking = false;
    }

    IEnumerator DelayedPhase3()
	{
        Debug.Log("Fuck the fuck off");
        yield return new WaitForSeconds(beatTime * 35);
        phaseTransition.SetTrigger("Phase 3");
        ChangeIdleAnimation(enemyAnim, angry2);
        phaseChange.MakeConversation(2, 10.15873015873016f / 2);
        var windMain2 = windParticles.GetComponent<ParticleSystem>().main;
        windMain2.simulationSpeed = 6;
        windMain2.startSize = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        StartCoroutine(Phase3MusicTransition());
        attackVariants = 6;
        introAttacks.Add(5);
        thacamra.speed = 1.1f;
        phase = 3;

        thirdAttack.enableIndicator(0);
        DoTheMario(0);
        maxShield = true;
    }

    // Enemy Turn Method
    void EnemyTurn(){
        // Updates Health-Based Modifiers
        
        if ((enemyStats.currentHP <= 0.5 * enemyStats.maxHP) && phase == 1)
        {
            phaseTransition.SetTrigger("Phase 2");
            ChangeIdleAnimation(enemyAnim, angry1);
            thacamra.enabled = true;
            phaseChange.MakeConversation(1, beatTime * 16);
            var windMain = windParticles.GetComponent<ParticleSystem>().main;
            windMain.simulationSpeed = 5;
            StartCoroutine(Phase2MusicTransition());
            attackVariants = 5;
            introAttacks.Add(4);
            phase = 2;
            if ((enemyStats.currentHP <= 0.25 * enemyStats.maxHP))
            {
                StartCoroutine(BePatient((beatTime * 35) + 11.15873015873016f));
                StartCoroutine(DelayedPhase3());
            }
            else
                StartCoroutine(BePatient(beatTime * 32));

            secondAttack.enableIndicator(0);
            DoTheMario(0);
            maxShield = true;

        }
        else if ((enemyStats.currentHP <= 0.25 * enemyStats.maxHP) && phase == 2)
        {
            phaseTransition.SetTrigger("Phase 3");
            ChangeIdleAnimation(enemyAnim, angry2);
            phaseChange.MakeConversation(2, 10.15873015873016f / 2);
            var windMain2 = windParticles.GetComponent<ParticleSystem>().main;
            windMain2.simulationSpeed = 6;
            windMain2.startSize = new ParticleSystem.MinMaxCurve(0.5f, 2f);
            StartCoroutine(Phase3MusicTransition());
            StartCoroutine(BePatient(11.15873015873016f));
            attackVariants = 6;
            introAttacks.Add(5);
            thacamra.speed = 1.1f;
            phase = 3;

            thirdAttack.enableIndicator(0);
            DoTheMario(0);
            maxShield = true;
        }

        enemyHpMod = 1.0f;

        playerAttackModifier = 1;
        // Debuff Logic =============
        if (false)
        {
            switch (enemyStats.debuffState)
            {
                case Debuff.BIND:
                    enemyAttackModifier = 1 - (0.25f * playerHpMod);
                    break;
            }
        }

        if (enemyStats.bind)
        {
            enemyAttackModifier = 1 - (0.25f * playerHpMod*bindMax);
        }
        else
            enemyAttackModifier = 1;

        doneAttacks.Clear();
        doubleDoneAttacks.Clear();
        attackAmount = 0;

        // ==========================
        if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 2) {
            StartCoroutine(BirdAttack());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 3) {
            StartCoroutine(CatAttack());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 0)
        {
            StartCoroutine(ShieldDefense());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 1)
        {
            StartCoroutine(ButterflyAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 4)
        {
            StartCoroutine(RootsAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 5)
        {
            StartCoroutine(PunchAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 6)
        {
            StartCoroutine(ShieldAttack());
        }
    }


    // Method to End Battle
    void EndBattle(){
        if (state == BattleState.WON){
            StartCoroutine(WinSequence());
        } else if (state == BattleState.LOST){
            StartCoroutine(LoseSequence());
        }
    }

    IEnumerator WinSequence()
    {
        if (buffLocation.passiveBuff == "AttackSet")
        {
            PlayerPrefs.SetInt("OffenseComplete", 1);
        }
        if (buffLocation.passiveBuff == "DefenseSet")
        {
            PlayerPrefs.SetInt("DefenseComplete", 1);
        }
        if (buffLocation.passiveBuff == "BalanceSet")
        {
            PlayerPrefs.SetInt("BalanceComplete", 1);
        }
        if (buffLocation.passiveBuff == "ControlSet")
        {
            PlayerPrefs.SetInt("ControlComplete", 1);
        }
        phaseTransition.SetTrigger("End");
        enemyAnim.SetTrigger("Die");
        currentSong.Stop();
        windEffect.enabled = false;
        thacamra.enabled = false;
        playerHUD.gameObject.SetActive(false);
        enemyHUD.gameObject.SetActive(false);
        EnemyAttackIndicatorController.Instance.ResetParticles();
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        secondAttack.disableAllIndicators();
        thirdAttack.disableAllIndicators();
        yield return new WaitForSeconds(3);
        startParticles.SetActive(false);
        phase2Particles.SetActive(false);
        phase3Particles.SetActive(false);
        windParticles.SetActive(false);
        playerHUD.actualShield.SetActive(false);
        enemyHUD.actualShield.SetActive(false);
        yield return new WaitForSeconds(3.6667f);
        phaseChange.MakeConversation(3, 3.5555f);
        yield return new WaitForSeconds(31.3333f);
        winCanvas.SetActive(true);
    }

    IEnumerator LoseSequence()
	{
        playerAnim.SetTrigger("Die");
        while (currentSong.pitch > 0)
        {
            currentSong.pitch -= 1f * Time.deltaTime;
            yield return null;
        }
        currentSong.Stop();
        gameOver.Play();
        yield return new WaitForSeconds(1);
        GameOverOverlay.Instance.FadeToGameOver();
    }

    void UpdateEffectDisplay(GameObject display, TextMeshProUGUI text, bool buffs, Stats character)
	{
        if (buffs)
		{
            if (!character.reflect && !character.dmgBoost && !character.vampirism)
			{
                display.SetActive(false);
                text.text = "";
			}
            else
			{
                display.SetActive(true);
                text.text = "";
                if (character.reflect)
				{
                    text.text = "<color=#2A72FF>REFLECT";
                }
                if (character.dmgBoost)
                {
                    if (text.text == "")
                        text.text = "<color=#FF2A2A>DAMAGE BOOST";
                    else
                        text.text += ", <color=#FF2A2A>DAMAGE BOOST";
                }
                if (character.vampirism)
                {
                    if (text.text == "")
                        text.text = "<color=#696969>VAMPIRISM";
                    else
                        text.text += ", <color=#696969>VAMPIRISM";
                }
            }
		}
        else
        {
            if (!character.bind && !character.stun)
            {
                display.SetActive(false);
                text.text = "";
            }
            else
            {
                display.SetActive(true);
                text.text = "";
                if (character.bind)
                {
                    text.text = "<color=#7D7C00>BOUND";
                }
                if (character.stun)
                {
                    if (text.text == "")
                        text.text = "<color=#C75700>STUNNED";
                    else
                        text.text += ", <color=#C75700>STUNNED";
                }
            }
        }
    }

    void ReduceEffects()
	{
        // Reduces Duration of Buffs and Debuffs
        if (playerStats.currentReflectDuration > 0)
        {
            playerStats.currentReflectDuration -= 1;
            if (playerStats.currentReflectDuration == 0)
            {
                playerStats.reflect = false;
                if (reflectMax >= 2)
                    EnableItem(mirrorObject);
                reflectMax = 1;
                mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            }
        }
        if (playerStats.currentDmgBoostDuration > 0)
        {
            playerStats.currentDmgBoostDuration -= 1;
            if (playerStats.currentDmgBoostDuration == 0)
            {
                playerStats.dmgBoost = false;
                if (dmgBoostMax >= 2)
                    EnableItem(appleObject);
                dmgBoostMax = 1;
                mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            }
        }
        if (playerStats.currentVampirismDuration > 0)
        {
            playerStats.currentVampirismDuration -= 1;
            if (playerStats.currentVampirismDuration == 0)
            {
                playerStats.vampirism = false;
                if (vampirismMax >= 2)
                    EnableItem(venomObject);
                vampirismMax = 1;
                mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            }
        }
        if (playerStats.currentStunDuration > 0)
        {
            playerStats.currentStunDuration -= 1;
            if (playerStats.currentStunDuration == 0)
            {
                playerStats.stun = false;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            }
        }
        if (enemyStats.currentBindDuration > 0)
        {
            enemyStats.currentBindDuration -= 1;
            if (enemyStats.currentBindDuration == 0)
            {
                enemyStats.bind = false;
                if (bindMax >= 2)
                    EnableItem(hairObject);
                bindMax = 1;
                mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            }
        }
        if (enemyStats.currentStunDuration > 0)
        {
            enemyStats.currentStunDuration -= 1;
            if (enemyStats.currentStunDuration == 0)
            {
                enemyStats.stun = false;
                if (stunMax >= 2)
                    EnableItem(shoesObject);
                stunMax = 1;
                mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            }
        }
    }

    void ReduceCooldowns()
    {
        // Reduces Duration of Cooldowns
        if (mirrorRestore > 0)
        {
            mirrorRestore -= 1;
            mirrorObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + mirrorRestore;
            if (mirrorRestore == 0)
            {
                EnableItem(mirrorObject);
            }
        }
        if (clothRestore > 0)
        {
            clothRestore -= 1;
            clothObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + clothRestore;
            if (clothRestore == 0)
            {
                EnableItem(clothObject);
            }
        }
        if (ballRestore > 0)
        {
            ballRestore -= 1;
            ballObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + ballRestore;
            if (ballRestore == 0)
            {
                EnableItem(ballObject);
            }
        }
        if (appleRestore > 0)
        {
            appleRestore -= 1;
            appleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + appleRestore;
            if (appleRestore == 0)
            {
                EnableItem(appleObject);
            }
        }
        if (venomRestore > 0)
        {
            venomRestore -= 1;
            venomObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + venomRestore;
            if (venomRestore == 0)
            {
                EnableItem(venomObject);
            }
        }
        if (handsRestore > 0)
        {
            handsRestore -= 1;
            handsObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + handsRestore;
            if (handsRestore == 0)
            {
                EnableItem(handsObject);
            }
        }
        if (hairRestore > 0)
        {
            hairRestore -= 1;
            hairObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + hairRestore;
            if (hairRestore == 0)
            {
                EnableItem(hairObject);
            }
        }
    }

    // Would be nice to not have to update this function every time an item is added but this whole script is spaghetti now basically so siudfhaisudhfauis
    void MaxUpdate()
	{
        // Mirror
        mirrorObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (mirrorRestore <= 0 && playerStats.reflect && reflectMax < 2)
        {
            mirrorObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (reflectMax >= 2)
        {
            mirrorObject.GetComponent<Button>().interactable = false;
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            mirrorObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Apple
        appleObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (appleRestore <= 0 && playerStats.dmgBoost && dmgBoostMax < 2)
        {
            appleObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (dmgBoostMax >= 2)
        {
            appleObject.GetComponent<Button>().interactable = false;
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            appleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Shoes
        shoesObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (playerStats.stun && enemyStats.stun && stunMax < 2)
        {
            shoesObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (stunMax >= 2)
        {
            shoesObject.GetComponent<Button>().interactable = false;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            shoesObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Venom
        venomObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (venomRestore <= 0 && playerStats.vampirism && vampirismMax < 2)
        {
            venomObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (vampirismMax >= 2)
        {
            venomObject.GetComponent<Button>().interactable = false;
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            venomObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Hair
        hairObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (hairRestore <= 0 && enemyStats.bind && bindMax < 2)
        {
            hairObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (bindMax >= 2)
        {
            hairObject.GetComponent<Button>().interactable = false;
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            hairObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }
    }

    // Method to start PlayerTurn
    void PlayerTurn()
    {
        if (playerStats.currentHP <= 0.25 * playerStats.maxHP)
        {
            playerHpMod = 3.0f;
        }
        else if (playerStats.currentHP <= 0.5 * playerStats.maxHP)
        {
            playerHpMod = 2.0f;
        }
        else
        {
            playerHpMod = 1.0f;
        }

        transitionMusic();
        ReduceEffects();
        ReduceCooldowns();


        if (!playerStats.stun)
		{
            shoesObject.GetComponent<Button>().interactable = false;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        else
		{
            shoesObject.GetComponent<Button>().interactable = true;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
            shoesObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }

        MaxUpdate();

        // Buff Logic =============
        if (false)
        {
            switch (playerStats.buffState)
            {
                case Buff.DMGBOOST:
                    playerAttackModifier = 1.5f;
                    break;
            }
        }

        if (playerStats.dmgBoost)
		{
            playerAttackModifier = 1 + (0.25f * playerHpMod*dmgBoostMax);
		}
        //=========================

        
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
        if (phase == 1)
        {
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25Switch.StopAllCoroutines();
                MC100Switch.StopAllCoroutines();
                MC100Switch.VariableTransition(MC25, 0.1666f);
                currentSong = MC25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50Switch.StopAllCoroutines();
                MC100Switch.StopAllCoroutines();
                MC100Switch.VariableTransition(MC50, 0.1666f);
                currentSong = MC50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100Switch.StopAllCoroutines();
                MC50Switch.StopAllCoroutines();
                MC50Switch.VariableTransition(MC100, 0.1666f);
                currentSong = MC100;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25Switch.StopAllCoroutines();
                MC50Switch.StopAllCoroutines();
                MC50Switch.VariableTransition(MC25, 0.1666f);
                currentSong = MC25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100Switch.StopAllCoroutines();
                MC25Switch.StopAllCoroutines();
                MC25Switch.VariableTransition(MC100, 0.1666f);
                currentSong = MC100;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50Switch.StopAllCoroutines();
                MC25Switch.StopAllCoroutines();
                MC25Switch.VariableTransition(MC50, 0.1666f);
                currentSong = MC50;
            }
        }
        else if (phase == 2)
		{
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25SW50Switch.StopAllCoroutines();
                MC100SW50Switch.StopAllCoroutines();
                MC100SW50Switch.VariableTransition(MC25SW50, 0.1666f);
                currentSong = MC25SW50;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC100SW50Switch.VariableTransition(MC50SW50, 0.1666f);
                currentSong = MC50SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.VariableTransition(MC100SW50, 0.1666f);
                currentSong = MC100SW50;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.VariableTransition(MC25SW50, 0.1666f);
                currentSong = MC25SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100SW50Switch.StopAllCoroutines();
                MC25SW50Switch.StopAllCoroutines();
                MC25SW50Switch.VariableTransition(MC100SW50, 0.1666f);
                currentSong = MC100SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50SW50Switch.StopAllCoroutines();
                MC25SW50Switch.StopAllCoroutines();
                MC25SW50Switch.VariableTransition(MC50SW50, 0.1666f);
                currentSong = MC50SW50;
            }
        }

        else if (phase == 3)
        {
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25SW25Switch.StopAllCoroutines();
                MC100SW25Switch.StopAllCoroutines();
                MC100SW25Switch.VariableTransition(MC25SW25, 0.1666f);
                currentSong = MC25SW25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC100SW25Switch.VariableTransition(MC50SW25, 0.1666f);
                currentSong = MC50SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.VariableTransition(MC100SW25, 0.1666f);
                currentSong = MC100SW25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.VariableTransition(MC25SW25, 0.1666f);
                currentSong = MC25SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100SW25Switch.StopAllCoroutines();
                MC25SW25Switch.StopAllCoroutines();
                MC25SW25Switch.VariableTransition(MC100SW25, 0.1666f);
                currentSong = MC100SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50SW25Switch.StopAllCoroutines();
                MC25SW25Switch.StopAllCoroutines();
                MC25SW25Switch.VariableTransition(MC50SW25, 0.1666f);
                currentSong = MC50SW25;
            }
        }

        if (MCstate == 0)
		{
            x2Text.SetActive(false);
            x3Text.SetActive(false);
        }
        else if (MCstate == 1)
        {
            x2Text.SetActive(true);
            x3Text.SetActive(false);
        }
        else if (MCstate == 2)
        {
            x2Text.SetActive(false);
            x3Text.SetActive(true);
        }
    }

    void ChangeIdleAnimation(Animator animator, AnimationClip anim)
	{
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            if (a.name == "regular idle")
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, anim));
        aoc.ApplyOverrides(anims);
        animator.runtimeAnimatorController = aoc;
    }


    IEnumerator Phase1MusicQuickStop()
	{
        MC100Switch.StopAllCoroutines();
        MC50Switch.StopAllCoroutines();
        MC25Switch.StopAllCoroutines();
        while (currentSong.volume > 0)
        {
            currentSong.volume -= startVolume * Time.deltaTime;
            yield return null;
        }
        MC100.Stop();
        MC50.Stop();
        MC25.Stop();
    }

    IEnumerator Phase2MusicQuickStop(AudioSource song)
    {
        MC100SW50Switch.StopAllCoroutines();
        MC50SW50Switch.StopAllCoroutines();
        MC25SW50Switch.StopAllCoroutines();
        while (song.pitch > 0)
        {
            song.pitch -= 1.1f * Time.deltaTime;
            yield return null;
        }
        MC100SW50.Stop();
        MC50SW50.Stop();
        MC25SW50.Stop();
    }

    IEnumerator Phase2MusicTransition()
	{

        MC100.Stop();
        MC50.Stop();
        MC25.Stop();
        MC100Switch.StopAllCoroutines();
        MC50Switch.StopAllCoroutines();
        MC25Switch.StopAllCoroutines();
        AudioSource transitionTo;
        if (playerStats.currentHP <= (playerStats.maxHP * 0.25f))
		{
            transitionTo = MC25SW50;
		}
        else if (playerStats.currentHP <= (playerStats.maxHP * 0.5f))
        {
            transitionTo = MC50SW50;
        }
        else
		{
            transitionTo = MC100SW50;
        }

        transitionTo.volume = 0;
        MC25SW50.time = 130.6667f;
        MC50SW50.time = 130.6667f;
        MC100SW50.time = 130.6667f;
        MC25SW50.Play();
        MC50SW50.Play();
        MC100SW50.Play();

        currentSong = transitionTo;
        while (currentSong.volume < startVolume * 0.05f)
        {
            currentSong.volume += (startVolume * 0.05f) * Time.deltaTime / (beatTime * 8);
            yield return null;
        }
        while (currentSong.volume < startVolume * 0.25f)
		{
            currentSong.volume += (startVolume * 0.20f) * Time.deltaTime / (beatTime * 8);
            yield return null;
        }
        while (currentSong.volume < startVolume)
        {
            currentSong.volume += (startVolume * 0.75f) * Time.deltaTime / (beatTime * 16);
            yield return null;
        }
        currentSong.volume = startVolume;
        phase2Particles.SetActive(true);
    }
    IEnumerator Phase3MusicTransition()
    {
        MC100SW50Switch.StopAllCoroutines();
        MC50SW50Switch.StopAllCoroutines();
        MC25SW50Switch.StopAllCoroutines();
        AudioSource transitionTo;
        if (playerStats.currentHP <= (playerStats.maxHP * 0.25f))
        {
            transitionTo = MC25SW25;
        }
        else if (playerStats.currentHP <= (playerStats.maxHP * 0.5f))
        {
            transitionTo = MC50SW25;
        }
        else
        {
            transitionTo = MC100SW25;
        }
        
        while (currentSong.pitch < 1.1)
        {
            currentSong.pitch += 0.1f * Time.deltaTime / 10.15873015873016f;
            yield return null;
        }
        StartCoroutine(Phase2MusicQuickStop(currentSong));

        transitionTo.volume = startVolume;
        MC25SW25.time = currentSong.time / 1.1f;
        MC50SW25.time = currentSong.time / 1.1f;
        MC100SW25.time = currentSong.time / 1.1f;
        MC25SW25.Play();
        MC50SW25.Play();
        MC100SW25.Play();
        currentSong = transitionTo;
        phase3Particles.SetActive(true);
        windEffect.SetTrigger("Windy");
    }

    float GetNextBeat(float multiplier)
	{
        float untilNext = (beatTime * multiplier) - (MC100.time % (beatTime * multiplier));
        Debug.Log(beatTime);
        Debug.Log(multiplier);
        Debug.Log(MC100.time);
        return (beatTime * multiplier) - (MC100.time % (beatTime * multiplier));
	}

    bool CheckMaxAttack()
	{
        if (playerStats.dmgBoost)
        {
            playerAttackModifier = 1 + (0.25f * playerHpMod * dmgBoostMax);
        }
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);
        if (phase == 1)
		{
            return maxAttack >= enemyStats.barrier + (enemyStats.currentHP - enemyStats.maxHP * 0.5f);
		}
        else if (phase == 2)
		{
            return maxAttack >= enemyStats.barrier + (enemyStats.currentHP - enemyStats.maxHP * 0.25f);
        }
        else
		{
            return maxAttack >= enemyStats.barrier + enemyStats.currentHP;
        }
    }

    // Methods for Enemy Actions ===================================================================
    IEnumerator BirdAttack(){

        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(2))
            birdCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1*playerHpMod*stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Bird");
            yield return new WaitForSeconds(0.3833f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyStats.damage));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyStats.damage * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyStats.damage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyStats.damage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.6167f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;

                // Enemy AI (THIS SHOULD DEFINITELY BE MADE INTO A FUNCTION!!!!!!!!!!!!)
                if (introAttacks.Contains(2))
				{
                    introAttacks.Remove(2);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(birdCounter, 2, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(birdCounter, 2, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(birdCounter, 2, thirdAttack);
                    }

                    if (birdCounter >= 2)
                        birdCounter = 0;
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }


                
            }
        }

        else
		{
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;

            // Enemy AI
            if (introAttacks.Contains(2))
            {
                introAttacks.Remove(2);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
			{
                PlayerTurn();
                ChooseNextAttack(birdCounter, 2, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(birdCounter, 2, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(birdCounter, 2, thirdAttack);
                }

                if (birdCounter >= 2)
                    birdCounter = 0;
                
            }
            else
			{
                if (attackAmount == 1)
				{
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    void ChooseNextAttack(int counter, int attackType, EnemyAttackIndicatorController attackDisplay)
	{
        maxShield = false;
        int enemyAttackType = Random.Range(0, attackVariants);
        if (CheckMaxAttack() && phase == 2 && attackAmount == 2 && !doneAttacks.Contains(0))
        {
            enemyAttackType = 0;
            maxShield = true;
        }
        else if (CheckMaxAttack() && phase == 3 && introAttacks.Count <= 0 && doneAttacks.Count <= 1)
		{
            enemyAttackType = 5;
		}
        else
        {
            if (CheckMaxAttack())
            {
                enemyAttackType = Random.Range(1, attackVariants);
            }
            if ((counter >= 2) && enemyAttackType == attackType)
            {
                enemyAttackType = Random.Range(0, attackVariants);
                if (CheckMaxAttack())
                {
                    enemyAttackType = Random.Range(1, attackVariants);
                }
                if (enemyAttackType == attackType)
                {
                    enemyAttackType = attackType - 1;
                    if ((CheckMaxAttack() && attackType - 1 == 0) || enemyAttackType < 0)
                        enemyAttackType = attackVariants - 1;
                }
            }
            if (playerStats.stun && enemyAttackType == 1)
            {
                enemyAttackType = attackType - 1;
                if ((CheckMaxAttack() && attackType - 1 == 0) || enemyAttackType < 0)
                    enemyAttackType = attackVariants - 1;
            }

            if ((!introAttacks.Contains(enemyAttackType)) && introAttacks.Count > 0)
            {
                if (enemyAttackType > introAttacks[0])
                {
                    while (!introAttacks.Contains(enemyAttackType))
                        enemyAttackType -= 1;
                }
                else
                {
                    enemyAttackType = introAttacks[0];
                }
            }

            if (enemyAttackType == 1 && doneAttacks.Contains(1))
            {
                enemyAttackType = Random.Range(0, attackVariants - 1);
                if (enemyAttackType >= 1)
                    enemyAttackType += 1;
                if (enemyAttackType == 5 && doneAttacks.Contains(5))
                {
                    enemyAttackType = Random.Range(0, 4);
                    if (enemyAttackType >= 1)
                        enemyAttackType += 1;
                }
            }
            else if (enemyAttackType == 5 && doneAttacks.Contains(5))
            {
                enemyAttackType = Random.Range(0, 5);
                if (enemyAttackType == 1 && doneAttacks.Contains(1))
                {
                    enemyAttackType = Random.Range(0, 4);
                    if (enemyAttackType >= 1)
                        enemyAttackType += 1;
                }
            }
        }

        Debug.Log("Max Shield is " + maxShield);
        if (introAttacks.Contains(enemyAttackType))
        {
            introAttacks.Remove(enemyAttackType);
        }
        int randomShieldChance = Random.Range(1, 3);
        if (enemyStats.barrier >= maxEnemyShield/randomShieldChance && !maxShield && enemyAttackType == 0)
            attackDisplay.enableIndicator(6);
        else
            attackDisplay.enableIndicator(enemyAttackType);
        DoTheMario(enemyAttackType);
    }

    IEnumerator ButterflyAttack()
    {
        
        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Butterfly");
            yield return new WaitForSeconds(2.06f);
            if (playerStats.TakeDamage((int)(enemyAttackModifier * 1)))
			{
                playerStats.currentHP = 1;
            }
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * 1 * (0.25f * playerHpMod)*reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * 1 * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }


            if (!immune)
            {
                playerStats.currentStunDuration = playerStats.stunDuration;
                playerStats.stun = true;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            }
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier*1);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(1))
            {
                introAttacks.Remove(1);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 1, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 1, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 1, thirdAttack);
                }

                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(1))
            {
                introAttacks.Remove(1);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 1, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 1, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 1, thirdAttack);
                }

                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    void DoTheMario(int enemyAttackType)
	{
        string amountString = "One";
        if (doneAttacks.Contains(enemyAttackType))
		{
            amountString = "Two";
            doubleDoneAttacks.Add(enemyAttackType);
        }
        else
		{
            doneAttacks.Add(enemyAttackType);
        }
        if (doubleDoneAttacks.Contains(enemyAttackType))
		{
            amountString = "Three";
        }
        if (enemyAttackType == 0)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Shield", amountString);
        }
        else if (enemyAttackType == 1)
        {
            EnemyAttackIndicatorController.Instance.ActivateParticles("Butterfly", amountString);
        }
        else if (enemyAttackType == 2)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Bird", amountString);
        }
        else if (enemyAttackType == 3)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Cat", amountString);
        }
        else if (enemyAttackType == 4)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Roots", amountString);
        }
        else if (enemyAttackType == 5)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Punch", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Punch", "Three");
            if (!(amountString == "Two"))
                EnemyAttackIndicatorController.Instance.ActivateParticles("Punch", amountString);
        }
    }

    IEnumerator CatAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(3))
            catCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int enemyDamage= Random.Range(0, (enemyStats.damage*2)+1);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {
            enemy.GetComponent<Animator>().SetTrigger("Kitty");
            yield return new WaitForSeconds(0.7f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod)*reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(1.15f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(3))
                {
                    introAttacks.Remove(3);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(catCounter, 3, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(catCounter, 3, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(catCounter, 3, thirdAttack);
                    }

                    if (catCounter >= 2)
                        catCounter = 0;
                    
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(3))
            {
                introAttacks.Remove(3);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(catCounter, 3, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(catCounter, 3, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(catCounter, 3, thirdAttack);
                }

                if (catCounter >= 2)
                    catCounter = 0;
                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    // This one isn't used in the Tech Demo so ignore it :D
    IEnumerator ShieldDefense()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(0))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if (maxShield)
		{
            stunChance = 5;
		}
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {
            enemyAnim.SetTrigger("Shield Time");
            yield return new WaitForSeconds(1.5f);
            int barrier = (int)(Random.Range((int)(enemyStats.damage*0.6667f), enemyStats.damage + 1));
            if (maxShield)
                barrier = enemyStats.damage;
            enemyStats.AddBarrier(barrier);
            enemyHUD.SetHP(enemyStats);
            // Can use Write Method here-------
        }
        else
		{
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        state = BattleState.PLAYERTURN;
        if (introAttacks.Contains(0))
        {
            introAttacks.Remove(0);
        }
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

        if (attackAmount >= phase)
        {
            PlayerTurn();
            maxShield = false;
            ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
            if (phase >= 2)
            {
                ChooseNextAttack(shieldCounter, 0, secondAttack);
            }
            if (phase == 3)
            {
                ChooseNextAttack(shieldCounter, 0, thirdAttack);
            }

            if (shieldCounter >= 2)
                shieldCounter = 0;
            
        }
        else
        {
            if (attackAmount == 1)
            {
                if (secondAttack.getEnabledIndicator() == 2)
                {
                    StartCoroutine(BirdAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 3)
                {
                    StartCoroutine(CatAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 0)
                {
                    StartCoroutine(ShieldDefense());
                }
                else if (secondAttack.getEnabledIndicator() == 1)
                {
                    StartCoroutine(ButterflyAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 4)
                {
                    StartCoroutine(RootsAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 5)
                {
                    StartCoroutine(PunchAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 6)
                {
                    StartCoroutine(ShieldAttack());
                }
            }
            else if (attackAmount == 2)
            {
                if (thirdAttack.getEnabledIndicator() == 2)
                {
                    StartCoroutine(BirdAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 3)
                {
                    StartCoroutine(CatAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 0)
                {
                    StartCoroutine(ShieldDefense());
                }
                else if (thirdAttack.getEnabledIndicator() == 1)
                {
                    StartCoroutine(ButterflyAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 4)
                {
                    StartCoroutine(RootsAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 5)
                {
                    StartCoroutine(PunchAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 6)
                {
                    StartCoroutine(ShieldAttack());
                }
            }
        }
    }

    IEnumerator ShieldAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(3))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int enemyDamage = Random.Range((int)(enemyStats.damage * 0.6667f), enemyStats.damage + 1);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            enemy.GetComponent<Animator>().SetTrigger("Throw Shield");
            yield return new WaitForSeconds(0.1666f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = false;
            if (playerStats.barrier > 0)
			{
                if ((int)(enemyAttackModifier * enemyDamage / 2) > playerStats.barrier)
				{
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + playerStats.barrier;
                    playerDamageText.gameObject.SetActive(true);
                    playerStats.barrier = 0;
				}
                else
				{
                    isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage / 2));
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage / 2);
                    playerDamageText.gameObject.SetActive(true);
                }
			}
            else
			{
                isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage));
                playerDamageText.gameObject.SetActive(false);
                playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
                playerDamageText.gameObject.SetActive(true);
            }

            if ((int)(enemyAttackModifier * enemyDamage / 2) > enemyStats.barrier)
            {
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + enemyStats.barrier;
                enemyDamageText2.gameObject.SetActive(true);
                enemyStats.barrier = 0;
            }
            else
            {
                enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage / 2));
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage / 2);
                enemyDamageText2.gameObject.SetActive(true);
            }

            transitionMusic();
            playerHUD.SetHP(playerStats);
            enemyHUD.SetHP(enemyStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Can use Write Method here-------
            yield return new WaitForSeconds(1.5f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(3))
                {
                    introAttacks.Remove(3);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(shieldCounter, 0, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(shieldCounter, 0, thirdAttack);
                    }

                    if (shieldCounter >= 2)
                        shieldCounter = 0;

                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(3))
            {
                introAttacks.Remove(3);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(shieldCounter, 0, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(shieldCounter, 0, thirdAttack);
                }

                if (shieldCounter >= 2)
                    shieldCounter = 0;

            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    IEnumerator RootsAttack()
    {

        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(4))
            rootsCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("RootsMagic");
            yield return new WaitForSeconds(0.25f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = false;
            playerStats.currentHP -= (int)((int)(enemyStats.damage * 0.6667f)* enemyAttackModifier);
            if (playerStats.currentHP <= 0)
			{
                playerStats.currentHP = 0;
                isDead = true;
			}
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * (int)(enemyStats.damage * 0.6667f) * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * (int)(enemyStats.damage * 0.6667f) * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)((int)(enemyStats.damage * 0.6667f) * enemyAttackModifier);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.6167f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;

                // Enemy AI
                if (introAttacks.Contains(4))
                {
                    introAttacks.Remove(4);
                }

                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(rootsCounter, 4, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(rootsCounter, 4, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(rootsCounter, 4, thirdAttack);
                    }

                    if (rootsCounter >= 2)
                        rootsCounter = 0;
                    
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }

        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;

            // Enemy AI
            if (introAttacks.Contains(4))
            {
                introAttacks.Remove(4);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(rootsCounter, 4, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(rootsCounter, 4, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(rootsCounter, 4, thirdAttack);
                }

                if (rootsCounter >= 2)
                    rootsCounter = 0;
                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    IEnumerator PunchAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(5))
            punchCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int missChance = Random.Range(0, 3);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.25f);
            enemy.GetComponent<Animator>().SetTrigger("omg punch");
            yield return new WaitForSeconds(0.4167f);
            bool isDead = false;
            if (missChance >= 1)
                isDead = playerStats.TakeDamage((int)(enemyAttackModifier * (enemyStats.damage * 2)));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect && missChance == 0)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * (enemyStats.damage * 2) * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * (enemyStats.damage * 2) * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            if (missChance >= 1)
            {
                playerDamageText.gameObject.SetActive(false);
                playerDamageText.text = "-" + (int)(enemyAttackModifier * (enemyStats.damage * 2));
                playerDamageText.gameObject.SetActive(true);
                hitSound.Play();
                // Shaky Effect
                for (int i = 0; i < 10; i++)
                {
                    player.transform.position += new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                    player.transform.position -= new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
                // Can use Write Method here-------
            }
            yield return new WaitForSeconds(1.15f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(5))
                {
                    introAttacks.Remove(5);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(punchCounter, 5, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(punchCounter, 5, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(punchCounter, 5, thirdAttack);
                    }

                    if (punchCounter >= 2)
                        punchCounter = 0;
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(5))
            {
                introAttacks.Remove(5);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(punchCounter, 5, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(punchCounter, 5, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(punchCounter, 5, thirdAttack);
                }

                if (punchCounter >= 2)
                    punchCounter = 0;
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
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
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }

    IEnumerator VolumeItem(float timeStop, AudioSource music, bool skip = false)
	{
        if (false)
        {
            if (!skip)
                yield return new WaitForSeconds(beatTime * 3.5f);
            while (music.volume > startVolume * 0.8f)
            {
                music.volume -= startVolume * Time.deltaTime * 4 / timeStop / 1.25f;
                yield return null;
            }
            yield return new WaitForSeconds(timeStop * .5f);
            while (music.volume < startVolume)
            {
                music.volume += startVolume * Time.deltaTime * 4 / timeStop / 1.25f;
                yield return null;
            }
            music.volume = startVolume;
        }
    }

    int DisableItem(GameObject itemObject, int maxWait)
	{
        itemObject.GetComponent<Button>().interactable = false;
        Debug.Log(itemObject.GetComponent<Button>().interactable);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
        itemObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: "+maxWait;
        return maxWait;
    }

    void EnableItem(GameObject itemObject)
    {
        itemObject.GetComponent<Button>().interactable = true;
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
        itemObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    // Method for Mirror Item - Needs Audio (my bad)
    IEnumerator UseMirror()
    {
        
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Mirror");
            yield return new WaitForSeconds(beatTime * 9.5f);
            playerStats.currentReflectDuration = playerStats.reflectDuration;
            if (playerStats.reflect)
			{
                reflectMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.reflect = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);

            playerHUD.SetHP(playerStats);

            if (!noCooldowns)
                mirrorRestore = DisableItem(mirrorObject, mirrorCooldown);
        }
        else
		{
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();

    }


    // Method for Cudgel Item - Needs Audio (my bad)
    IEnumerator UseCudgel()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Cudgel");
            bool isDead = false;
            if (enemyStats.TakeDamage((int)((3 * playerAttackModifier * playerHpMod) * attackBuff)))
            {
                if (phase == 3)
                    isDead = true;
                else
                {
                    enemyStats.currentHP = 1;
                }
            }
            if (isDead)
            {
                yield return new WaitForSeconds(beatTime * 3.3f);
                theDie.Play();
                yield return new WaitForSeconds(beatTime * 4);
                enemyHUD.SetHP(enemyStats);
                enemyAnim.SetTrigger("Die");
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                yield return new WaitForSeconds(beatTime * 7.3f);
                enemyAnim.SetTrigger("Owie");
                yield return new WaitForSeconds(beatTime * 0.2f);

                if (playerStats.vampirism)
                {
                    if ((int)((5 * playerAttackModifier * playerHpMod) * attackBuff) > enemyStats.barrier)
                    {
                        playerStats.Heal((int)(2 * playerHpMod * vampirismMax));
                        playerDamageText.gameObject.SetActive(false);
                        playerDamageText.text = "<color=#FF3A3A>+" + ((int)(2 * playerHpMod * vampirismMax));
                        playerDamageText.gameObject.SetActive(true);
                    }
                }

                enemyDamageText.text = "-" + (int)((3 * playerAttackModifier * playerHpMod) * attackBuff);
                transitionMusic();
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());

                playerHUD.SetHP(playerStats);
                enemyHUD.SetHP(enemyStats);
                hitSound.Play();
                // Shaky Effect!
                // for ( int i = 0; i < 10; i++)
                //{
                //enemy.transform.position += new Vector3(5f, 0, 0);
                //yield return new WaitForSeconds(0.01f);
                //enemy.transform.position -= new Vector3(5f, 0, 0);
                //yield return new WaitForSeconds(0.01f);
                //}
                enemy.GetComponent<Animator>().enabled = true;
                yield return new WaitForSeconds(2.2f);
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }


    // Method for Magic Cloth Item - Needs Audio (my bad)
    IEnumerator UseMagicCloth()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime))
            {
                yield return new WaitForSeconds(waitTime - (beatTime));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 3));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Cloth");
            yield return new WaitForSeconds(beatTime * 5);
            if (playerStats.currentHP == playerStats.maxHP)
            {
            }
            else
            {
                playerDamageText.gameObject.SetActive(false);
                if ((playerStats.currentHP + (int)(4 * playerHpMod)) <= playerStats.maxHP)
                    playerDamageText.text = "<color=#FF3A3A>+" + (int)(4 * playerHpMod);
                else
                    playerDamageText.text = "<color=#FF3A3A>+" + (playerStats.maxHP - playerStats.currentHP);
                playerDamageText.gameObject.SetActive(true);
                playerStats.Heal((int)(4 * playerHpMod));
                transitionMusic();
                playerHUD.SetHP(playerStats);
            }
            yield return new WaitForSeconds(beatTime * 4.5f);

            if (!noCooldowns)
                clothRestore = DisableItem(clothObject, clothCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }


        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Silver Hands Item - Needs Audio (my bad)
    IEnumerator UseSilverHands()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Hands");
            yield return new WaitForSeconds(beatTime * 6f);
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#3E7EFF>+" + (int)(3 * playerHpMod);
            playerDamageText.gameObject.SetActive(true);
            playerStats.AddBarrier((int)(3 * playerHpMod));

            playerHUD.SetHP(playerStats);
            yield return new WaitForSeconds(beatTime * 3.5f);

            if (!noCooldowns)
                handsRestore = DisableItem(handsObject, handsCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        yield return null;
    }

    // Method for Rapunzels Hair Item - Needs Audio (my bad)
    IEnumerator UseRapunzelHair()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 2.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 2.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 1.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            
            playerAnim.SetTrigger("Hair");
            yield return new WaitForSeconds(beatTime * 6);
            enemyStats.currentBindDuration = enemyStats.bindDuration;
            if (enemyStats.bind)
			{
                bindMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.bind = true;
            UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);

            enemyHUD.SetHP(enemyStats);

            yield return new WaitForSeconds(beatTime * 3.5f);

            if (!noCooldowns)
                hairRestore = DisableItem(hairObject, hairCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // New Items =================================================================
    // Method for Iron Shoes Item
    IEnumerator UseIronShoes()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        playerAnim.SetTrigger("Shoes");
        yield return new WaitForSeconds(beatTime * 9.5f);
        if (playerStats.stun)
        {
            enemyStats.currentStunDuration = enemyStats.stunDuration;
            if (enemyStats.stun)
            {
                stunMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.stun = true;
            UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            // Need to get code for activating debuff intigators
            playerStats.currentStunDuration = 0;
            playerStats.stun = false;
            UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
        }
        else
        {


        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Hat that fires bullets Item
    IEnumerator UseHat()
    {

        int randInt = Random.Range(1, 7);
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Hat " + randInt);
            yield return new WaitForSeconds(beatTime * 5.75f);
            enemyDamageText.text = "-" + (int)((1 * playerAttackModifier * playerHpMod) * attackBuff);
            bool isDead = false;
            enemyAnim.SetTrigger("Shot");
            for (int i = 0; i < randInt; i++)
            {
                if (playerStats.vampirism)
                {
                    if ((int)((1 * playerAttackModifier * playerHpMod) * attackBuff) > enemyStats.barrier)
                    {
                        playerStats.Heal(2 * (int)playerHpMod);
                        playerDamageText.gameObject.SetActive(false);
                        playerDamageText.text = "<color=#FF3A3A>+" + ((int)(2 * playerHpMod));
                        playerDamageText.gameObject.SetActive(true);
                    }
                }
                if (enemyStats.TakeDamage((int)((1 * playerAttackModifier * playerHpMod) * attackBuff)))
                {
                    if (phase == 3)
                        isDead = true;
                    else
                    {
                        enemyStats.currentHP = 1;
                    }
                }
                if (isDead)
                {
                    theDie.Play();
                    yield return new WaitForSeconds(beatTime * 4);
                    enemyHUD.SetHP(enemyStats);
                    enemyAnim.SetTrigger("Die");
                    state = BattleState.WON;
                    EndBattle();
                    break;
                }
                transitionMusic();
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                playerHUD.SetHP(playerStats);
                enemyHUD.SetHP(enemyStats);
                if (i == randInt - 1)
                {
                    hitSound.Play();
                }
                yield return new WaitForSeconds(beatTime / 2);
            }
            if (!isDead)
            {
                enemyAnim.SetTrigger("Unshot");
                yield return new WaitForSeconds(beatTime + ((beatTime / 2) * (6 - randInt)));
                yield return new WaitForSeconds(1);
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    // Method for Crystal Ball Item
    IEnumerator UseCrystalBall()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Ball");
            yield return new WaitForSeconds(beatTime * 6);
            if (playerStats.currentReflectDuration > 0)
            {
                playerStats.currentReflectDuration += 1 * (int)playerHpMod;
            }
            if (playerStats.currentDmgBoostDuration > 0)
            {
                playerStats.currentDmgBoostDuration += 1 * (int)playerHpMod;
            }
            if (playerStats.currentVampirismDuration > 0)
            {
                playerStats.currentVampirismDuration += 1 * (int)playerHpMod;
            }

            if (playerStats.stun)
            {
                playerStats.currentStunDuration = 0;
                playerStats.stun = false;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            }

            if (!noCooldowns)
                ballRestore = DisableItem(ballObject, ballCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Apple from Tree of Life Item
    IEnumerator UseApple()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 3));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Apple");
            yield return new WaitForSeconds(beatTime * 5);
            playerStats.currentDmgBoostDuration = playerStats.dmgBoostDuration;
            if (playerStats.dmgBoost)
			{
                dmgBoostMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.dmgBoost = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            yield return new WaitForSeconds(beatTime * 2);

            if (!noCooldowns)
                appleRestore = DisableItem(appleObject, appleCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        // Buff Indicator handling needed here!
    }

    // Method for White Snake Venom Item
    IEnumerator UseVenom()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 2))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 2));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Venom");
            yield return new WaitForSeconds(beatTime * 6);
            if (playerStats.TakeDamage(3))
                playerStats.currentHP = 1;
            transitionMusic();
            playerHUD.SetHP(playerStats);
            // Shaky Effect
            if (false)
            {
                for (int i = 0; i < 10; i++)
                {
                    player.transform.position += new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                    player.transform.position -= new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            yield return new WaitForSeconds(beatTime * 3.5f);
            playerStats.currentVampirismDuration = playerStats.vampirismDuration;
            if (playerStats.vampirism)
			{
                vampirismMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.vampirism = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);

            if (!noCooldowns)
                venomRestore = DisableItem(venomObject, venomCooldown);

            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            // Needs Indicator handling
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }

    // Method for Sack of Knowledge Item
    IEnumerator UseSackOfKnowledge(){
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        yield return null;
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
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(PlayerAttack());
    }

}
