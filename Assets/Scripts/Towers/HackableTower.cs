using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableTower : TowerBehavior
{
    [Header("Hackable Tower Settings")]
    public float chanceToBeHacked = 1f;
    private bool isHacked = false;
    public GameObject hackedUiParent;

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
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        shootBehavior.OnObjectEnterRange += (enemy) => TryGetHacked(enemy);
        OnHacked += (enemy) => hackedUiParent.SetActive(true);
        OnUnhacked += () => hackedUiParent.SetActive(false);
    }

    void TryGetHacked(GameObject enemy)
    {
        if (enabled && Random.value < chanceToBeHacked)
        {
            Debug.Log("Tower was hacked!");
            enabled = false;
            isHacked = true;
            OnHacked?.Invoke(enemy);
        }
    }
}
