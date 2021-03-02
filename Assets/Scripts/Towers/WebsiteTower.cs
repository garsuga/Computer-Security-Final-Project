using System;
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

        GameController.instance.OnRoundEnd += (roundNum) =>
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
                int moneyToAdd = Mathf.RoundToInt(AdjustForDistance((transform.position - enemyStartTransform.position).magnitude, moneyAmount) * databaseMultiplier);
                GameController.instance.Money += moneyToAdd;
                GameController.EmitText(gameObject, "+ " + ((Int32)moneyToAdd).ToString("C"), 1f, Color.green, 50, new Vector3(0, .5f));
            }
        };
    }

    float AdjustForDistance(float distance, float moneyAmount)
    {
        return distance / 20 * moneyAmount;
    }
}
