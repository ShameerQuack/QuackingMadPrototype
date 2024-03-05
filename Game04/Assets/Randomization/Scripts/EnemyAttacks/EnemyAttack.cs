using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : ScriptableObject
{
    private IEnumerator preprocessing(Stats player, Stats enemy){
        yield return null;
    }

    private IEnumerator postprocessing(Stats player, Stats enemy){
        yield return null;
    }

    public void execute(Stats player, Stats enemy){
        // yield return StartCoroutine(preprocessing(player, enemy));
        // yield return StartCoroutine(process(player, enemy));
        // yield return StartCoroutine(postprocessing(player, enemy));
    }

    protected abstract IEnumerator process(Stats player, Stats enemy);
}
