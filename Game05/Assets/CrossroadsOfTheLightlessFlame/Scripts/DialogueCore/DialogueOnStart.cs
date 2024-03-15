using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Narrative
{
    public class DialogueOnStart : MonoBehaviour
    {
        public bool fight = false;
        /// <summary> The csv file containing the dialogue to be played. </summary>
        [SerializeField] private TextAsset dialogueCSV;

        public TextAsset[] fightDialogues;
        public int fightConvo = 0;

        [Header("Set Dialogue Flag After Finishing Dialogue")]
        [Tooltip("Which flag to assign after finishing this dialogue.")]
        [SerializeField]
        private string writeToFlagId = "";
        [SerializeField] private bool writeToFlagValue = false;
        private float waitForTime = -1.0f;

        [Header("Conditions")]
        [SerializeField] private List<Condition> conditions = new List<Condition>();

        public DialogueTrigger goddamn_raccoon;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("I started!");
            waitForTime = DialogueSystem.Instance.waitForTime;
            if (waitForTime <= 0)
            {
                if (dialogueCSV != null)
                {//If we have a dialogue
                    DialogueSystem.OnDialogueEnd.AddListener(OnDialogueEnd);
                    if (fightConvo <= 0)
                        DialogueSystem.PlaySequence(dialogueCSV);
                    else
                        DialogueSystem.PlaySequence(fightDialogues[fightConvo-1]);
                }
            }
            else
                StartCoroutine(WaitABit());
        }

        IEnumerator WaitABit()
        {
            yield return new WaitForSeconds(waitForTime);
            if (dialogueCSV != null)
            {//If we have a dialogue
                DialogueSystem.OnDialogueEnd.AddListener(OnDialogueEnd);
                DialogueSystem.PlaySequence(dialogueCSV);
            }
        }

        /// <summary>
        /// Callback reciever for ehrn dialogue ends.
        /// Writes to flags if it is set
        /// </summary>
        private void OnDialogueEnd()
        {
            if (!fight)
            {
                if (writeToFlagId != "")
                {
                    DialogueFlags.SetFlag(writeToFlagId, writeToFlagValue);
                }
                DialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEnd);//We shouldn't recieve this if we aren't playing something.
                goddamn_raccoon.Trigger();
                Destroy(this);
            }
            else
			{
                Destroy(this);
            }
        }
    }
}
