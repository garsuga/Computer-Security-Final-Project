using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseTower : TowerBehavior
{
    public float chanceToBeHacked = 1f;
    private bool isHacked = false;

    public bool Hacked
    {
        get
        {
            return isHacked;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        shootBehavior.OnObjectEnterRange += (enemy) => TryGetHacked(enemy);
    }

    void TryGetHacked(GameObject enemy)
    {
        if (enabled && Random.value < chanceToBeHacked)
        {
            Debug.Log("Web tower was hacked!");
            this.transform.Rotate(new Vector3(0, 0, 45), Space.Self);
            enabled = false;
            isHacked = true;
        }
    }
}
