using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class TowerBehavior : MonoBehaviour
{
    /// <summary>
    /// From https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
    /// </summary>
    /// <param name="container">GameObject to create LineRenderer on</param>
    /// <param name="radius">Circle radius in relative space</param>
    /// <param name="lineWidth">Line width in relative space</param>
    public static LineRenderer DrawCircle(GameObject container, float radius, float lineWidth)
    {
        int segments = 360;
        LineRenderer line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        int pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);

        return line;
    }

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
