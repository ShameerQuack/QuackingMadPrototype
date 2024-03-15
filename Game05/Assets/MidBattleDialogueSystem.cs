using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Narrative
{
    public class MidBattleDialogueSystem : MonoBehaviour
    {
        public GameObject[] thingsToReset;
        public ScriptableObject[] thingsToStart;
        public DialogueOnStart csvHolder;
        public DialogueBox fightChanger;
        public DialogueSystem delayChanger;
        public DialogueTrigger startText;
        public GameObject[] skipButtons;

        // Start is called before the first frame update
        void Start()
        {
            // Things to start with:  

            // DialogueSystem.PlaySequence(dialogueCSV);
            // dialogueCanvas.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MakeConversation(int convoNumber, float convoTime)
        {
            foreach (GameObject button in skipButtons)
            {
                button.GetComponent<Image>().enabled = false;
                button.GetComponent<Button>().enabled = false;
            }
            foreach (GameObject dialogueScript in thingsToReset)
            {
                dialogueScript.SetActive(false);
            }
            csvHolder.fightConvo = convoNumber;
            DialogueBox.midFight = true;
            fightChanger.fightWait = convoTime;
            delayChanger.waitForTime = 0;
            foreach (GameObject dialogueScript in thingsToReset)
            {
                dialogueScript.SetActive(true);
            }
            DialogueSystem.PlaySequence(csvHolder.fightDialogues[csvHolder.fightConvo - 1]);
        }
    }
}
