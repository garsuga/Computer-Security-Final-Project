using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableTower : TowerBehavior
{
    [Header("Hackable Tower Settings")]
    public float chanceToBeHacked = 1f;
    private bool isHacked = false;
    public GameObject hackedUiParent;
    public RangeObjectDetectionBehavior hackObjectDetection;

    public delegate void OnHackedEvent(GameObject hackedBy);
    public delegate void OnUnhackedEvent();

    public OnHackedEvent OnHacked
    {
        get;set;
    }

    public OnUnhackedEvent OnUnhacked
    {
        get;set;
    }

    public bool Hacked
    {
        get
        {
            return isHacked;
        }

        set
        {
            isHacked = value;
            if(!isHacked)
                OnUnhacked?.Invoke();
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        hackObjectDetection.OnObjectEnterRange += (enemy) => TryGetHacked(enemy);
        OnHacked += (enemy) => hackedUiParent.SetActive(true);
        OnHacked += (enemy) => GameController.EmitText(null, gameObject.transform.position, "Hacked!", 1f, Color.red, 50, new Vector3(0, .5f));
        OnUnhacked += () => hackedUiParent.SetActive(false);
    }

    void TryGetHacked(GameObject enemy)
    {
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();

        if (enabled && !Hacked && Random.value < chanceToBeHacked && !enemyBehavior.IsDead)
        {
            Debug.Log("Tower was hacked!");
            //enabled = false;
            isHacked = true;
            OnHacked?.Invoke(enemy);
            enemyBehavior.isEarlyDestroyed = true;
            Destroy(enemy);
        }
    }
}
