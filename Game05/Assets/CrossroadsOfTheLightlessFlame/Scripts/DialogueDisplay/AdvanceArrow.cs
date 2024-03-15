using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Narrative
{
    /// <summary>
    /// Behaviour class for a textbox arrow.
    /// </summary>
    public class AdvanceArrow : MonoBehaviour
    {
        [SerializeField] private Image image;
        private static DialogueBox _instance;
        public static DialogueBox Instance { get { return _instance; } }

        // Start is called before the first frame update
        void Start()
        {
            SetVisible(false);//start invisible
        }

        /// <summary>
        /// Sets whether er arrow should be visible.
        /// </summary>
        public void SetVisible(bool isVisible)
        {
            if (DialogueBox.midFight)
                isVisible = false;
            image.enabled = isVisible;
        }
    }
}