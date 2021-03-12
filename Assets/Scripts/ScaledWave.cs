using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScaledWave
{
    private List<GameObject> flattenedWave;
    private WaveSetting waveSetting;

    public ScaledWave(WaveSetting setting, float enemyCountScale)
    {
        this.flattenedWave = Flatten(Scale(setting.EnemySets, enemyCountScale));
        this.waveSetting = setting;
    }

    private Dictionary<GameObject, int> Scale(Dictionary<GameObject, int> enemySets, float scale)
    {
        enemySets = new Dictionary<GameObject, int>(enemySets);

        foreach(GameObject key in new List<GameObject>(enemySets.Keys))
        {
            enemySets[key] = Mathf.RoundToInt(enemySets[key] * scale);
        }

        return enemySets;
    }

    private List<GameObject> Flatten(Dictionary<GameObject, int> enemySets)
    {
        List<GameObject> flat = new List<GameObject>();
        foreach (GameObject key in enemySets.Keys)
        {
            for (int i = 0; i < enemySets[key]; i++)
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
    public List<GameObject> GetNextSpawn()
    {
        List<GameObject> nextSpawn = new List<GameObject>();

        for (int i = 0; i < waveSetting.EnemiesPerTick && flattenedWave.Count > 0; i++)
        {
            nextSpawn.Add(flattenedWave[0]);
            flattenedWave.RemoveAt(0);
        }

        return nextSpawn;
    }

    /// <summary>
    /// If the wave has enemies left to spawn.
    /// </summary>
    public bool HasEnemies
    {
        get
        {
            return flattenedWave.Count > 0;
        }
    }
}
