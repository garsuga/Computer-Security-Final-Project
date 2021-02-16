using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebsiteTower : TowerBehavior
{
    public Transform enemyStartTransform;
    public int moneyAmount;
    public float chanceToBeHacked = .25f;
    private bool isHacked = false;
    public float databaseMaxDist = 2.5f;
    public float databaseIncreasePercent = .25f;

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

        shootBehavior.OnObjectEnterRange += (obj) => TryGetHacked(obj);
        GameController.instance.OnRoundBegin += (roundNum) =>
        {
            if (this != null && enabled)
            {
                GameObject[] databases = GameObject.FindGameObjectsWithTag("Database");
                int databasesInRange = 0;
                foreach (GameObject databaseObject in databases) {
                    DatabaseTower dbBehavior = databaseObject.GetComponent<DatabaseTower>();
                    if(dbBehavior != null && dbBehavior.enabled)
                    {
                        if ((databaseObject.transform.position - transform.position).sqrMagnitude < databaseMaxDist * databaseMaxDist)
                        {
                            databasesInRange += 1;
                        }
                    }
                }

                GameController.instance.Money += Mathf.RoundToInt(AdjustForDistance((transform.position - enemyStartTransform.position).magnitude, moneyAmount) * (1 + databaseIncreasePercent * databasesInRange));
            }
        };
    }

    int AdjustForDistance(float distance, float moneyAmount)
    {
        return Mathf.RoundToInt(distance / 20 * moneyAmount);
    }

    void TryGetHacked(GameObject enemy)
    {
        if(enabled && Random.value < chanceToBeHacked)
        {
            Debug.Log("Web tower was hacked!");
            this.transform.Rotate(new Vector3(0, 0, 45), Space.Self);
            enabled = false;
            isHacked = true;
        }
    }
}
