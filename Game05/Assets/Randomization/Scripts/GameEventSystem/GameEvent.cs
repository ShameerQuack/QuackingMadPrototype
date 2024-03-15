using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = 
		new List<GameEventListener>();
    public List<GameEvent> concurrentGameEvents;

    public void Raise()
    {
        for(int i = listeners.Count -1; i >= 0; i--){
          listeners[i].OnEventRaised();
        }
        for(int i = concurrentGameEvents.Count -1; i >= 0; i--){
          concurrentGameEvents[i].Raise();
        }
    }

    public void RegisterListener(GameEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(GameEventListener listener)
    { listeners.Remove(listener); }
    
}
