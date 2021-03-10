using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseTower : HackableTower
{
    [Header("Database Tower Settings")]
    public float databaseMaxDist = 2.5f;
    public float databaseIncreasePercent = .25f;

    public float lineRendererWidth = 1;

    private LineRenderer currentLineRenderer;

    public DatabaseTower() : base()
    {
        OnPlaced += (towerRoot) => currentLineRenderer = GameController.DrawCircle(towerRoot, Vector3.zero, databaseMaxDist, lineRendererWidth, Color.magenta, false);
        OnHacked += (enemy) =>
        {
            if (currentLineRenderer != null)
                currentLineRenderer.enabled = false;
        };
        OnUnhacked += () =>
        {
            if (currentLineRenderer != null)
                currentLineRenderer.enabled = true;
        };
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    
}
