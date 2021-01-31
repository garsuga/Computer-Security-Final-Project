using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    Dictionary<GameObject, int> enemySets;

    int enemiesPerTick;

    public Wave(Dictionary<GameObject, int> enemySets, int enemiesPerTick)
    {
        this.enemySets = enemySets;
        this.enemiesPerTick = enemiesPerTick;
    }

    public bool HasEnemies
    {
        get {
            return enemySets.Count > 0; 
        }
    }

    public List<GameObject> getNextSpawn()
    {
        List<GameObject> nextSpawn = new List<GameObject>();

        GameObject[] keys = new GameObject[enemySets.Keys.Count];
        enemySets.Keys.CopyTo(keys, 0);

        for(int i = 0; i < enemiesPerTick && enemySets.Count > 0; i++)
        {
            int rIndex = (int)(Random.value * enemySets.Keys.Count);

            GameObject nextEnemy = keys[rIndex];

            int leftInWave = enemySets[nextEnemy];
            leftInWave--;

            enemySets.Remove(nextEnemy);
            if (leftInWave > 0)
            {
                enemySets.Add(nextEnemy, leftInWave);
            }

            nextSpawn.Add(nextEnemy);
        }

        return nextSpawn;
    }
}
