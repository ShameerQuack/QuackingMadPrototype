using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class LoadGameEvent : UnityEvent<SaveData> {}
public class LoadEventListener : MonoBehaviour
{
    public LoadEvent Event;
    public LoadGameEvent Response;

    private void OnEnable()
    { Event.RegisterListener(this); }

    private void OnDisable()
    { Event.UnregisterListener(this); }

    public void OnEventRaised(SaveData saveData)
    { Response.Invoke(saveData); }
}