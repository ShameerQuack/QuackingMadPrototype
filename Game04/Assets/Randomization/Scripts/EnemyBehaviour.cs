using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviourType { RANDOM, SEQUENTIAL }
public class EnemyBehaviour : MonoBehaviour
{
    public List<GameObject> enemyAttacks;
    [SerializeField]
    private EnemyBehaviourType behaviourType;
    private int sequentialBehaviourIndex;

    void Start(){
        sequentialBehaviourIndex = 0;
    }
    public void OnEnemyTurn(){
        if (behaviourType == EnemyBehaviourType.RANDOM){
            // enemyAttacks[Random.Range(0, enemyAttacks.Count())].execute();
        } else if (behaviourType == EnemyBehaviourType.SEQUENTIAL){
            // enemyAttacks[sequentialBehaviourIndex % enemyAttacks.Count()].execute();
            // sequentialBehaviourIndex += 1;
        }
    }
}
