using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MITMBehavior : EnemyBehavior
{
    [Header("MITM Attack Settings")]
    public int maxEnemiesCanTeleport = 5;

    public override void Start()
    {
        base.Start();

        OnEnemyDeath += () =>
        {
            List<EnemyBehavior> enemies = new List<EnemyBehavior>(FindObjectsOfType<EnemyBehavior>());

            GameController.EmitText(null, gameObject.transform.position, "MITM Attack!", 1f, Color.red, 50, new Vector3(0, .5f));

            enemies = new List<EnemyBehavior>(enemies.FindAll(e => e != this).OrderBy(e => Random.value));

            for(int i = 0; i < maxEnemiesCanTeleport && i < enemies.Count; i++)
            {
                enemies[i].transform.position = this.transform.position;
            }
        };
    }

    public override void Update()
    {
        base.Update();
    }
}
