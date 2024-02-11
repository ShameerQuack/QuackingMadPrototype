using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Narrative
{
    public class DialogueChoices : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public DialogueButton[] buttons;

        /// <summary> The csv file containing the dialogue to be played. </summary>
        [SerializeField] private TextAsset[] dialogueCSVs;//Need to have at most the number of buttons
        [SerializeField] private string[] choiceTexts;//The button text to use, and must match the number above
        [SerializeField] private Condition[] postChoiceConditionEffects;//The button text to use, and must match the number above
        
        
        [Header("Conditions to Activate")]
        [SerializeField] private List<Condition> conditions = new List<Condition>();

        //Whether or not this choice has been activated
        private bool activated = false;

        //button choice
        private int _choice = -1;

        public GameObject blueBookmark;
        public GameObject redBookmark;
        public GameObject rp1, rp2, rp3, bp1, bp2, bp3;

        //Plays at the Start
        void Start(){
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

        void Update(){
            //Check conditions
            if (!AreConditionsTrue())
            {
                return; //Cancel activation if any conditions fail
            }
            else{
                if(!activated){
                    //Activate choice
                    for(int i = 0; i<dialogueCSVs.Length; i++){
                       buttons[i].gameObject.SetActive(true); 
                       buttons[i].text.text = choiceTexts[i];
                    }
                    activated = true;
                }

            }
        }

        /// <summary>
        /// Callback reciever for when dialogue ends.
        /// Writes to flags if it is set
        /// </summary>
        private void OnDialogueEnd()
        {
            DialogueFlags.SetFlag(postChoiceConditionEffects[_choice].flagID, postChoiceConditionEffects[_choice].expectedValue);
            DialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEnd);//We shouldn't recieve this if we aren't playing something.
            Destroy(this);
        }


        public void ChoiceSelected(int choice)
        {
            _choice = choice;
            for(int i = 0; i<dialogueCSVs.Length; i++){
                buttons[i].gameObject.SetActive(false); 
            }
            blueBookmark.SetActive(false);
            redBookmark.SetActive(false);
            rp1.SetActive(false);
            rp2.SetActive(false);
            rp3.SetActive(false);
            bp1.SetActive(false);
            bp2.SetActive(false);
            bp3.SetActive(false);
            DialogueSystem.PlaySequence(dialogueCSVs[choice]);
            DialogueSystem.OnDialogueEnd.AddListener(OnDialogueEnd);

        }
    }
}