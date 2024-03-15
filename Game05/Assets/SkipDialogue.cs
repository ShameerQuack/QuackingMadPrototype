using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative {
    public class SkipDialogue : MonoBehaviour
    {
        public GameObject[] thingsToClose;
        public DialogueOnStart otherThing;
        // Start is called before the first frame update
        void Start()
        {
            if (PlayerPrefs.GetInt("ChangingBookmark") == 1)
            {
                foreach (GameObject thing in thingsToClose)
                {
                    thing.SetActive(false);
                }
                otherThing.enabled = false;
                GameMaster.Instance.BookMarkSelectionManager.startSelection();
            }
        }
    }
}
