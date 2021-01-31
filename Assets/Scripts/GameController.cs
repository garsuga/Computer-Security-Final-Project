using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] enemyPath = new GameObject[0];

    public Wave[] waves = new Wave[0];

    [Header("Enemy References")]
    public GameObject redEnemy;
    public GameObject blueEnemy;


    [Header("Wave Settings")]
    public float timeBetweenWaves = 15f;

    public float timePerTick = 1f;
    public float timeBetweenTicks = 1f;


    // Start is called before the first frame update
    void Start()
    {
        SetupWaves();

        StartCoroutine("SpawnWaves");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupWaves()
    {
        Dictionary<GameObject, int> wave1Enemies = new Dictionary<GameObject, int>();

        wave1Enemies.Add(redEnemy, 10);
        wave1Enemies.Add(blueEnemy, 5);

        Wave wave1 = new Wave(wave1Enemies, 5);

        Dictionary<GameObject, int> wave2Enemies = new Dictionary<GameObject, int>();

        wave2Enemies.Add(redEnemy, 20);
        wave2Enemies.Add(blueEnemy, 15);

        Wave wave2 = new Wave(wave2Enemies, 8);

        waves = new Wave[] { wave1, wave2 };
    }

    IEnumerator SpawnWaves()
    {
        foreach(Wave wave in waves) {
            // single wave
            while(wave.HasEnemies)
            {
                List<GameObject> nextSet = wave.getNextSpawn();

                // single tick
                foreach(GameObject prefab in nextSet)
                {
                    // single spawn
                    Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    yield return new WaitForSeconds(timePerTick / nextSet.Count);
                }

                yield return new WaitForSeconds(timeBetweenTicks);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GameObject last = null;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < enemyPath.Length; i++)
        {
            GameObject current = enemyPath[i];

            if (last != null)
            {
                Debug.DrawLine(last.transform.position, current.transform.position, Color.red);
            }

            last = current;

            Gizmos.DrawWireCube(current.transform.position, Vector3.one * .2f);
        }
    }
#endif
}
