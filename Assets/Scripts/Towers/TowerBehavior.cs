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

    [TextArea]
    public string towerName;
    [TextArea]
    public string towerDescription;

    public TowerHoverTextController hoverTextController;

    public GameObject towerShopUiParent;
    public bool isPlaced = false;
    public GameObject shopHoverDescParent;
    /// <summary>
    /// Reference to the shoot behavior which handles boilerplate shooting mechanics
    /// </summary>
    public RangeObjectDetectionBehavior shootObjectDetection;
    public Collider2D shopGrabCollider;

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
            shootObjectDetection.enabled = true;
            towerShopUiParent.SetActive(false);
        };
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        towerCostText.text = ((Int32)towerCost).ToString("C");

        hoverTextController.SetDescriptionText(towerDescription);
        hoverTextController.SetTitleText(towerName);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(shopGrabCollider != null && shopGrabCollider.OverlapPoint(mousePos))
        {
            shopHoverDescParent?.SetActive(true);
        } else
        {
            shopHoverDescParent?.SetActive(false);
        }
    }
}
