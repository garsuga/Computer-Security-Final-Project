using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebsiteTower : HackableTower
{
    public Transform enemyStartTransform;
    public int moneyAmount;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        GameController.instance.OnRoundBegin += (roundNum) =>
        {
            if (!isPlaced)
                return;

            if (this != null && enabled)
            {
                DatabaseTower[] databases = GameObject.FindObjectsOfType<DatabaseTower>();
                float databaseMultiplier = 1f;
                foreach (DatabaseTower database in databases) {
                    if(database != null && database.enabled)
                    {
                        if ((database.transform.position - transform.position).sqrMagnitude < database.databaseMaxDist * database.databaseMaxDist)
                        {
                            databaseMultiplier += database.databaseIncreasePercent;
                        }
                    }
                }

                GameController.instance.Money += Mathf.RoundToInt(AdjustForDistance((transform.position - enemyStartTransform.position).magnitude, moneyAmount) * databaseMultiplier);
            }
        };
    }

    int AdjustForDistance(float distance, float moneyAmount)
    {
        return Mathf.RoundToInt(distance / 20 * moneyAmount);
    }
}
