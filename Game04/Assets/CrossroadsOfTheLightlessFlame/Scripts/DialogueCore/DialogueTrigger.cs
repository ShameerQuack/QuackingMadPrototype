using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative
{
    /// <summary>
    /// A script that will activate a dialogue when triggered.
    /// Must be triggered manually but it will attempt to automatically connect to a ClickableObject script.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        public bool repeatable = false;//whether or not this dialogue can repeat

        /// <summary> The csv file containing the dialogue to be played. </summary>
        [SerializeField] private TextAsset dialogueCSV;

        

        [Header("Conditions")]
        [SerializeField] private List<Condition> conditions = new List<Condition>();


        [Header("Set Dialogue Flag After Finishing Dialogue")]
        [Tooltip("Which flag to assign after finishing this dialogue.")]
        [SerializeField]
        private string writeToFlagId = "";
        [SerializeField] private bool writeToFlagValue = false;

        public GameObject blueBookmark;
        public GameObject redBookmark;
        public GameObject choiceBox;

        public GameObject rp1, rp2, rp3, bp1, bp2, bp3;

        /// <summary>
        /// Call this to activate the dialogue. If condition are set they must all be satisfied.
        /// </summary>
        public void Trigger()
        {

            blueBookmark.SetActive(true);
            redBookmark.SetActive(true);
            choiceBox.SetActive(true);
            rp1.SetActive(true);
            rp2.SetActive(true);
            rp3.SetActive(true);
            bp1.SetActive(true);
            bp2.SetActive(true);
            bp3.SetActive(true);

        }

        /// <summary>
        /// Evaluates whether all conditions are satisfied.
        /// </summary>
        private bool AreConditionsTrue()
        {
            foreach (Condition condition in conditions)
            {
                if (DialogueFlags.GetFlagValue(condition.flagID) != condition.expectedValue)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Callback reciever for when dialogue ends.
        /// Writes to flags if it is set
        /// </summary>
        private void OnDialogueEnd()
        {
            if (writeToFlagId != "")
            {
                DialogueFlags.SetFlag(writeToFlagId, writeToFlagValue);
            }
            DialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEnd);//We shouldn't recieve this if we aren't playing something.
            if(!repeatable){
                Destroy(this);
            }
        }
    }
}