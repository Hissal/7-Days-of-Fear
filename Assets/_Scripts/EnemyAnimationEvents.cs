using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void Start()
    {
        if (enemyAI == null)
            enemyAI = GetComponentInParent<EnemyAI>();
    }

    public void EnemyListen()
    {
        Debug.Log("EnemyListen");
        enemyAI.Listen();
    }
    public void EnemyStopListen()
    {
        Debug.Log("EnemyStopListen");
        enemyAI.StopListen();
    }
}
