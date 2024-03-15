using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class BookMarkSelectionManager : MonoBehaviour
{
    public Canvas bookMarkSelectionCanvas;
    public List<BookMarkSet> bookMarkSets;
    public GameObject crossRoadsItemPrefab;
    public GameObject notePrefab;
    public List<Transform> redItemPositions;
    public List<Transform> blueItemPositions;
    public GameEvent redClickedEvent;
    public GameEvent blueClickedEvent;
    public CrossRoadData crossRoadDataSaveObject;
    public Transform notePosition;
    public AudioSource scrapeSound;
    public TextMeshProUGUI redBuff;
    public TextMeshProUGUI blueBuff;
    public TextMeshProUGUI redTitle;
    public TextMeshProUGUI blueTitle;
    public TextMeshProUGUI redDifficulty;
    public TextMeshProUGUI blueDifficulty;
    public CursorMoment theCursor;

    private List<Item> redItems;
    private List<Item> blueItems;

    private string attackBuff = "Buff: Immune to Debuffs";
    private string defenseBuff = "Buff: Damage + 50%";
    private string controlBuff = "Buff: No Item Cooldown";
    private string balanceBuff = "Buff: Max HP + 50%";
    private string knowledgeBuff = "Buff: ???";

    private string attackTitle = "Offense";
    private string defenseTitle = "Defense";
    private string controlTitle = "Control";
    private string balanceTitle = "Balance";

    private string attackDifficulty = "Difficulty:\n4/4";
    private string defenseDifficulty = "Difficulty:\n2/4";
    private string controlDifficulty = "Difficulty:\n1/4";
    private string balanceDifficulty = "Difficulty:\n3/4";

    private string redMessage;
    private string blueMessage;

    public MusicLoop thaMusic;
    public GameObject recommendedText;
    public Button redButton;
    public Button blueButton;
    public GameObject redStar;
    public GameObject blueStar;

    void Start(){
        bookMarkSelectionCanvas.enabled = false;
        redItems = new List<Item>();
        blueItems = new List<Item>();
    }

    public void startSelection(){
        bookMarkSelectionCanvas.enabled = true;
        List<int> recommendedDifficulty = new List<int> { PlayerPrefs.GetInt("ControlComplete", 0), PlayerPrefs.GetInt("DefenseComplete", 0),
                                                          PlayerPrefs.GetInt("BalanceComplete", 0), PlayerPrefs.GetInt("OffenseComplete", 0) };
        int difficultyLevel = recommendedDifficulty.IndexOf(0);
        BookMarkSet redSet = null;
        if (difficultyLevel > -1)
        {
            redSet = bookMarkSets[difficultyLevel];
            bookMarkSets.RemoveAt(difficultyLevel);
        }
        List<BookMarkSet> shuffledBookMarkSets = bookMarkSets.OrderBy(x => Random.value).ToList();
        if (difficultyLevel == -1)
        {
            redSet = shuffledBookMarkSets[0];
            recommendedText.SetActive(false);
        }
        BookMarkSet blueSet = shuffledBookMarkSets[1];
        redMessage = redSet.setName;
        blueMessage = blueSet.setName;
        redItems = redSet.items;
        blueItems = blueSet.items;

        for (int i = 0; i < redSet.items.Count();i++){
            GameObject obj = Instantiate(crossRoadsItemPrefab, redItemPositions[i]);
            GameObject noteObj = Instantiate(notePrefab, notePosition);
            var note= noteObj.transform.GetComponent<Image>();
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var iconNoteReference = obj.transform.Find("Icon").GetComponent<HoverForDescription>();
            obj.transform.Find("Icon").GetComponent<RectTransform>().sizeDelta = redItemPositions[i].GetComponent<RectTransform>().sizeDelta;
            iconNoteReference.spriteRenderer = noteObj.transform.GetComponent<Image>();
            note.sprite = redSet.items[i].note;
            icon.sprite = redSet.items[i].crossRoadsIcon;
            obj.transform.Find("Icon").GetComponent<HoverForDescription>().scrape = scrapeSound;
            obj.transform.Find("Icon").GetComponent<HoverForDescription>().cursorChanger = theCursor;
        }

        for (int i = 0; i < blueSet.items.Count();i++){
            GameObject obj = Instantiate(crossRoadsItemPrefab, blueItemPositions[i]);
            GameObject noteObj = Instantiate(notePrefab, notePosition);
            var note= noteObj.transform.GetComponent<Image>();
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var iconNoteReference = obj.transform.Find("Icon").GetComponent<HoverForDescription>();
            obj.transform.Find("Icon").GetComponent<RectTransform>().sizeDelta = blueItemPositions[i].GetComponent<RectTransform>().sizeDelta;
            iconNoteReference.spriteRenderer = noteObj.transform.GetComponent<Image>();
            note.sprite = blueSet.items[i].note;
            icon.sprite = blueSet.items[i].crossRoadsIcon;
            obj.transform.Find("Icon").GetComponent<HoverForDescription>().scrape = scrapeSound;
            obj.transform.Find("Icon").GetComponent<HoverForDescription>().cursorChanger = theCursor;
        }

        if (redSet.setName == "AttackSet")
        {
            if (PlayerPrefs.GetInt("OffenseComplete", 0) == 1)
                redStar.SetActive(true);
            redBuff.text = attackBuff;
            redTitle.text = attackTitle + " |";
            redDifficulty.text = attackDifficulty;
        }
        else if (redSet.setName == "DefenseSet")
        {
            if (PlayerPrefs.GetInt("DefenseComplete", 0) == 1)
                redStar.SetActive(true);
            redBuff.text = defenseBuff;
            redTitle.text = defenseTitle + " |";
            redDifficulty.text = defenseDifficulty;
        }
        else if (redSet.setName == "ControlSet")
        {
            if (PlayerPrefs.GetInt("ControlComplete", 0) == 1)
                redStar.SetActive(true);
            redBuff.text = controlBuff;
            redTitle.text = controlTitle + " |";
            redDifficulty.text = controlDifficulty;
        }
        else if (redSet.setName == "BalanceSet")
        {
            if (PlayerPrefs.GetInt("BalanceComplete", 0) == 1)
                redStar.SetActive(true);
            redBuff.text = balanceBuff;
            redTitle.text = balanceTitle + " |";
            redDifficulty.text = balanceDifficulty;
        }
        else if (redSet.setName == "KnowledgeSet")
        {
            redBuff.text = knowledgeBuff;
        }

        if (blueSet.setName == "AttackSet")
        {
            if (PlayerPrefs.GetInt("OffenseComplete", 0) == 1)
                blueStar.SetActive(true);
            blueBuff.text = attackBuff;
            blueTitle.text = "| " + attackTitle;
            blueDifficulty.text = attackDifficulty;
        }
        else if (blueSet.setName == "DefenseSet")
        {
            if (PlayerPrefs.GetInt("DefenseComplete", 0) == 1)
                blueStar.SetActive(true);
            blueBuff.text = defenseBuff;
            blueTitle.text = "| " + defenseTitle;
            blueDifficulty.text = defenseDifficulty;
        }
        else if (blueSet.setName == "ControlSet")
        {
            if (PlayerPrefs.GetInt("ControlComplete", 0) == 1)
                blueStar.SetActive(true);
            blueBuff.text = controlBuff;
            blueTitle.text = "| " + controlTitle;
            blueDifficulty.text = controlDifficulty;
        }
        else if (blueSet.setName == "BalanceSet")
        {
            if (PlayerPrefs.GetInt("BalanceComplete", 0) == 1)
                blueStar.SetActive(true);
            blueBuff.text = balanceBuff;
            blueTitle.text = "| " + balanceTitle;
            blueDifficulty.text = balanceDifficulty;
        }
        else if (blueSet.setName == "KnowledgeSet")
        {
            blueBuff.text = knowledgeBuff;
        }

    }

    private void closeSelection(){
        redButton.interactable = false;
        blueButton.interactable = false;
        bookMarkSelectionCanvas.enabled = false;
    }

    public void onRedClicked(){
        crossRoadDataSaveObject.items = redItems;
        crossRoadDataSaveObject.message = redMessage;
        if (PlayerPrefs.GetInt("ChangingBookmark") == 1)
		{
            thaMusic.Transition();
            SceneChanger.Instance.FadeToNextScene();
        }
        else
            redClickedEvent.Raise();
        closeSelection();
    }

    public void onBlueClicked(){
        crossRoadDataSaveObject.items = blueItems;
        crossRoadDataSaveObject.message = blueMessage;
        if (PlayerPrefs.GetInt("ChangingBookmark") == 1)
        {
            thaMusic.Transition();
            SceneChanger.Instance.FadeToNextScene();
        }
        else
            blueClickedEvent.Raise();
        closeSelection();
    }
}
