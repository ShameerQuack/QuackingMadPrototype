using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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

    private List<Item> redItems;
    private List<Item> blueItems;

    void Start(){
        bookMarkSelectionCanvas.enabled = false;
        redItems = new List<Item>();
        blueItems = new List<Item>();
    }

    public void startSelection(){
        bookMarkSelectionCanvas.enabled = true;
        List<BookMarkSet> shuffledBookMarkSets = bookMarkSets.OrderBy( x => Random.value ).ToList();
        BookMarkSet redSet = shuffledBookMarkSets[0];
        BookMarkSet blueSet = shuffledBookMarkSets[1];
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
        }

    }

    private void closeSelection(){
        bookMarkSelectionCanvas.enabled = false;
    }

    public void onRedClicked(){
        crossRoadDataSaveObject.items = redItems;
        redClickedEvent.Raise();
        closeSelection();
    }

    public void onBlueClicked(){
        crossRoadDataSaveObject.items = blueItems;
        blueClickedEvent.Raise();
        closeSelection();
    }
}
