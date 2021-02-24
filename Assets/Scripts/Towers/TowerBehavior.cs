using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class TowerBehavior : MonoBehaviour
{
    [Header("General Tower Settings")]
    public int towerCost = 1000;
    public Text towerCostText;
    public GameObject towerShopUiParent;
    public bool isPlaced = false;

    public delegate void OnPlacedEvent(GameObject towerRoot);

    public OnPlacedEvent OnPlaced
    {
        get;set;
    }

    public TowerBehavior() {
        OnPlaced += (towerRoot) =>
        {
            TowerCanBeLiftedBehavior canBeLiftedBehavior = towerRoot.GetComponentInChildren<TowerCanBeLiftedBehavior>();
            canBeLiftedBehavior.enabled = false;

            shootBehavior.enabled = true;

            towerShopUiParent.SetActive(false);
        };
    }

    /// <summary>
    /// Reference to the shoot behavior which handles boilerplate shooting mechanics
    /// </summary>
    public TowerShootBehavior shootBehavior;
    // Start is called before the first frame update
    public virtual void Start()
    {
        towerCostText.text = ((Int32)towerCost).ToString("C");
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
