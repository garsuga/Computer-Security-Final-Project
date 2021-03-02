using System;
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
            GameController.EmitText(gameObject, "+ " + ((Int32)moneyAmount).ToString("C"), 1f, Color.green, 50, new Vector3(0,.5f));
        };
    }
}
