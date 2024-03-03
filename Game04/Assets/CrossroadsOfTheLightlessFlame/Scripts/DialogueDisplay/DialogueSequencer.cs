using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Narrative
{

    /// <summary>
    /// An inline struct to contain dialogue activation conditions
    /// </summary>
    [System.Serializable]
    public struct Condition
    {
        [Tooltip("Which flag to check")]
        public string flagID;

        [Tooltip("If the flag matches this value this condition will succeed.")]
        public bool expectedValue;
    }

    /// <summary>
    /// Component that controls the sequencing of a dialogue.
    /// </summary>
    public class DialogueSequencer : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onStarted;
        public UnityEvent onFinish;

        //Sub-Object references
        [SerializeField] private DialogueBox textbox;
        [SerializeField] private TextMeshProUGUI originalTextLabel;
        [SerializeField] private TextMeshProUGUI otherTextLabel;
        [SerializeField] private DialogueChoices choices;

        //Object properties
        private DialogueSequence currentDialog;  //set when we are playing
        private int currentLine = 0;  //current line in the sequence we are playing
        private bool isPlaying = false;

        public GameObject skipButton;
        public GameObject skipButton2;
        public GameObject speaker1;
        public GameObject speaker2;
        public string speaker1Name = "";
        public string speaker2Name = "";

        /// <summary>
        /// Starts playing the given sequence
        /// </summary>
        /// <param name="dialogue">Dialogue resource to play</param>
        public void PlaySequence(DialogueSequence dialogue)
        {
            if (dialogue.IsEmpty())
            {
                Debug.LogWarning("Playing sequence stopped because DialogueSequence was empty");
                return;
            }

            //Set our state
            currentDialog = dialogue;
            currentLine = 0;
            isPlaying = true;

            //Open and play
            textbox.OpenTextbox();
            ParseLine(currentLine);

            onStarted.Invoke();
        }

        /// <summary>
        /// Event reciever that advances the dialogue to the next line or closes if the dialogue is finished
        /// </summary>
        public void onSequenceAdvanced()
        {
            gameObject.GetComponent<AudioSource>().Play();
            bool hasNext = currentDialog.HasLine(currentLine + 1);
            if (hasNext)
            {
                //Start next line
                currentLine++;
                ParseLine(currentLine);
            }
            else
            {
                //Finished, close textbox
                textbox.CloseTextbox();
                onFinish.Invoke();
            }
        }

        public void Skip()
		{
            textbox.CloseTextbox();
            onFinish.Invoke();
            skipButton.SetActive(false);
            skipButton2.SetActive(false);
        }

        /// <summary>
        /// Applies the current line.
        /// </summary>
        /// <param name="lineNum">Line number</param>
        private void ParseLine(int lineNum)
        {
            //Apply to textbox speaker name
            string name = currentDialog.GetRowName(lineNum);
            if (name == speaker1Name)
            {
                print("eee");
                speaker2.SetActive(false);
                speaker1.SetActive(true);
                textbox.ChangeText(originalTextLabel);
            }
            else if (name == speaker2Name)
            {
                print("e");
                speaker1.SetActive(false);
                speaker2.SetActive(true);
                textbox.ChangeText(otherTextLabel);
            }
            //Apply to textbox
            textbox.SetLine(currentDialog.GetRowDialogue(lineNum));
            if (name != "")//Only apply if not empty
            {
                print(name);
                
                textbox.SetName(name);
            }
            
            
        }

        /// <summary>
        /// Checks if the sequencer is playing a sequence
        /// </summary>
        public bool IsPlaying()
        {
            return textbox.IsActive;
        }

    }
}