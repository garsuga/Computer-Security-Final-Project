using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrypoMiningTower : TowerBehavior
{
    public int moneyAmount;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        GameController.instance.OnRoundEnd += (roundNum) =>
        {
            if (!isPlaced)
                return;
            GameController.instance.Money += moneyAmount;
        };
    }
}
