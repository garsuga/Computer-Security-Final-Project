using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wave
{
    //private Dictionary<GameObject, int> enemySets;
    private List<GameObject> flattenedWave;

    private int enemiesPerTick;

    public Wave(Dictionary<GameObject, int> enemySets, int enemiesPerTick)
    {
        this.flattenedWave = Flatten(enemySets);
        this.enemiesPerTick = enemiesPerTick;
    }

    /// <summary>
    /// If the wave has enemies left to spawn.
    /// </summary>
    public bool HasEnemies
    {
        get {
            return flattenedWave.Count > 0;
        }
    }

    private List<GameObject> Flatten(Dictionary<GameObject, int> enemySets)
    {
        List<GameObject> flat = new List<GameObject>();
        foreach(GameObject key in enemySets.Keys)
        {
            for(int i = 0; i < enemySets[key]; i++)
            {
                flat.Add(key);
            }
        }

        return new List<GameObject>(flat.OrderBy(e => Random.value));
    }

    /// <summary>
    /// Get the enemies for a single wave tick in random order.
    /// </summary>
    /// <returns>List of enemies for a single wave tick in random order</returns>
    public List<GameObject> getNextSpawn()
    {
        List<GameObject> nextSpawn = new List<GameObject>();

        for(int i = 0; i < enemiesPerTick && flattenedWave.Count > 0; i++)
        {
            nextSpawn.Add(flattenedWave[0]);
            flattenedWave.RemoveAt(0);
        }

        return nextSpawn;
    }
}
