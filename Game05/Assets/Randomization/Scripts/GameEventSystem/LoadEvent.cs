using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LoadEvent : ScriptableObject
{
    private List<LoadEventListener> listeners = 
		new List<LoadEventListener>();

    public void Raise(SaveData saveData)
    {
        for(int i = listeners.Count -1; i >= 0; i--){
          listeners[i].OnEventRaised(saveData);
        }
    }

    public void RegisterListener(LoadEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(LoadEventListener listener)
    { listeners.Remove(listener); }
    
}