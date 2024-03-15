using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Narrative
{
    /// <summary>
    /// A component class for the dialogue textbox display.
    /// </summary>
    public class DialogueBox : MonoBehaviour
    {
        //Event Callbacks
        public UnityEvent onAdvance;

        //Sub-object references
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private AdvanceArrow advanceArrow;
        [SerializeField] private AdvanceArrow advanceArrow2;
        [SerializeField] private Animator animator;

        //Object properties
        public float charactersPerSecond = 45.0f;//how many characters are displayed per second
        private float currentCharacter = 0f; //current position where
        private int textLength = 0;//cached content length

        public bool inButton = false;

        //State tracking variables and getters, useful for timing
        //serialized so the animator can edit them but shouldn't be modified otherwise
        [SerializeField] [HideInInspector] private bool isOpen = false;
        [SerializeField] [HideInInspector] private bool isActive = false;
        public bool IsOpen { get { return isOpen; } }
        public bool IsActive { get { return isActive; } }
        private float waitForTime = -1.0f;

        public AudioSource talkSound;
        public MusicLoop transitionMusic;
        public int enemyIntroChangePoint = 0;
        public int enemyIntroCheck = 0;
        public Image angryFace;
        public Sprite visualAnger;
        public Animator fullBody;

        public static bool midFight = false;
        private bool waiting = false;
        public float fightWait;

        // Start is called before the first frame update
        private void Start()
        {
            midFight = false;
            waitForTime = DialogueSystem.Instance.waitForTime;
            if (waitForTime <= 0)
            {
                //Connect onAdvance event to hide the advancearrow object on invoke
                onAdvance.AddListener(() => advanceArrow.SetVisible(false));
                onAdvance.AddListener(() => advanceArrow2.SetVisible(false));
            }
            else
                StartCoroutine(WaitABit());
        }

        IEnumerator WaitABit()
        {
            yield return new WaitForSeconds(waitForTime);
            //Connect onAdvance event to hide the advancearrow object on invoke
            onAdvance.AddListener(() => advanceArrow.SetVisible(false));
            onAdvance.AddListener(() => advanceArrow2.SetVisible(false));
        }

        // Update is called once per frame
        void Update()
        {
            if (isOpen)
            {
                //Update state
                CheckInput();
                UpdateText();
            }
        }

        /// <summary>
        /// Checks player input during frame update
        /// </summary>
        private void CheckInput()
        {
            //Input for advancing textbox
            if (Input.anyKey && !midFight)
            {
                if (isEndOfText() && !inButton)
                {
                    Debug.Log("why");
                    AdvanceLine();
                }
            }
            else if (midFight && !waiting)
			{
                StartCoroutine(WaitForEnd(fightWait));
                waiting = true;
            }
        }

        IEnumerator WaitForEnd(float waitTime)
		{
            yield return new WaitForSeconds(waitTime);
            waiting = false;
            AdvanceLine();
        }

        /// <summary>
        /// Advances per character text.
        /// </summary>
        private void UpdateText()
        {
            if (currentCharacter < textLength)
            {
                //Advance visible characters
                currentCharacter += Time.deltaTime * charactersPerSecond;
                textLabel.maxVisibleCharacters = Mathf.FloorToInt(currentCharacter);

                if (isEndOfText())
                {
                    talkSound.Stop();
                    advanceArrow.SetVisible(true);
                    advanceArrow2.SetVisible(true);
                }
            }
        }

        /// <summary>
        /// Opens the textbox with playing the open animation
        /// </summary>
        public void OpenTextbox()
        {
            talkSound.Play();
            animator.SetBool("isOpen", true);
            advanceArrow.SetVisible(false);
            advanceArrow2.SetVisible(false);
            nameLabel.text = "";
            isActive = true;
            if (midFight)
			{
                angryFace.sprite = visualAnger;
            }
        }

        public void ChangeText(TextMeshProUGUI newText)
		{
            textLabel = newText;
		}

        /// <summary>
        /// Closes the textbox by playing the close animation
        /// </summary>
        public void CloseTextbox()
        {
            animator.SetBool("isOpen", false);
            talkSound.Stop();
        }

        /// <summary>
        /// Sets the line and resets the per character scrolling
        /// </summary>
        /// <param name="sourceText"></param>
        public void SetLine(string sourceText)
        {
            textLabel.SetText(sourceText);

            //Reset scroll
            textLabel.maxVisibleCharacters = 0;
            currentCharacter = 0f;
            textLength = sourceText.Length;
        }

        /// <summary>
        /// Sets the speaker name.
        /// </summary>
        /// <param name="sourceText">The name to be displayed</param>
        public void SetName(string sourceText)
        {
            if (nameLabel.text != sourceText && nameLabel.text != "")
            {
                //Jostle the textbox when the speaker changes
                animator.SetTrigger("jostle");
            }

            //Apply text to label
            nameLabel.SetText(sourceText);
        }


        /// <summary>
        /// Checks if the per character scrolling reached the end of the text.
        /// </summary>
        /// <returns></returns>
        private bool isEndOfText()
        {
            return (currentCharacter >= textLength);
        }

        /// <summary>
        /// Called when the line advances. Invokes the onAdvance event.
        /// </summary>
        public void AdvanceLine()
        {
            if (enemyIntroChangePoint > 0 && !midFight)
			{
                enemyIntroCheck += 1;
                if (enemyIntroCheck >= enemyIntroChangePoint)
				{
                    enemyIntroChangePoint = -1;
                    transitionMusic.Transition();
                    angryFace.sprite = visualAnger;
                    fullBody.SetTrigger("Get Mad");
				}
			}
            talkSound.Play();
            advanceArrow.SetVisible(false);
            advanceArrow2.SetVisible(false);
            onAdvance.Invoke();
        }
    }
}