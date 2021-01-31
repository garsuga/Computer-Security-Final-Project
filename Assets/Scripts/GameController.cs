using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Enemy References")]
    public GameObject redEnemy;
    public GameObject blueEnemy;


    [Header("Wave Settings")]
    public float timeBetweenWaves = 15f;

    public float timePerTick = 1f;
    public float timeBetweenTicks = 1f;

    [Header("Tower Grid Settings")]
    public GameObject gridOrigin;
    public float gridScale = .2f;
    public int gridHeight = 5;
    public int gridWidth = 5;
    public bool[] gridDisabledPositions;

    [Header("Virtual Mouse Settings")]
    public GameObject virtualMouseGameObject;
    public float mouseGameObjectZ = 1;

    [Header("Tower Settings")]
    public float towerInstantiateZ = 0;

    private TowerGrid towerGrid;
    private MouseObserverBevahior mouseObserver;
    public GameObject[] enemyPath = new GameObject[0];
    public Wave[] waves = new Wave[0];
    private Camera mainCamera;

    private GameObject currentTowerHolding;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        towerGrid = new TowerGrid(gridWidth, gridHeight, gridScale, gridOrigin.transform.position, gridDisabledPositions);

        mouseObserver = GetComponent<MouseObserverBevahior>();

        mouseObserver.OnMouseDrag += MouseDrag;
        mouseObserver.OnMousePress += MousePress;

        SetupWaves();
        StartCoroutine("SpawnWaves");
    }

    // Update is called once per frame
    void Update()
    {
        if(virtualMouseGameObject != null)
        {
            virtualMouseGameObject.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            virtualMouseGameObject.transform.position = new Vector3(virtualMouseGameObject.transform.position.x, virtualMouseGameObject.transform.position.y, mouseGameObjectZ);
        }
    }

    List<GameObject> GetTowersAtPosition(Vector3 positionWorld)
    {
        List<GameObject> hits = new List<GameObject>();

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        
        foreach(GameObject tower in towers)
        {
            BoxCollider2D collider = tower.GetComponent<BoxCollider2D>();
            if(collider.OverlapPoint(positionWorld))
            {
                hits.Add(tower);
            }
        }

        return hits;
    }

    void MousePress(int button, Vector3 positionScreen)
    {
        Vector3 positionWorld = mainCamera.ScreenToWorldPoint(positionScreen);
        List<GameObject> clickedTowers = GetTowersAtPosition(positionWorld);
        if(clickedTowers.Count > 0)
        {
            // TODO: Evaluate
            currentTowerHolding = Instantiate<GameObject>(clickedTowers[0], virtualMouseGameObject.transform);
            currentTowerHolding.transform.localPosition = Vector3.zero;

            TowerBehavior towerBehavior = currentTowerHolding.GetComponent<TowerBehavior>();
            towerBehavior.enabled = false;
        }
    }

    void MouseDrag(int button, Vector3 startPosScreen, Vector3 currentPosScreen, float time, bool isOver)
    {
        if (currentTowerHolding != null) {
            if (isOver)
            {
                
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(currentPosScreen);

                TowerGrid.Position gridPosition = towerGrid.toGridPosition(worldPos);

                if(gridPosition != null && !towerGrid.isGridPosDisabled(gridPosition) && !towerGrid.isGridPosOccupied(gridPosition))
                {
                    // can place tower
                    TowerBehavior towerBehavior = currentTowerHolding.GetComponent<TowerBehavior>();
                    towerBehavior.enabled = true;

                    currentTowerHolding.transform.parent = null;

                    Vector3 snappedPosition = towerGrid.snapToGrid(worldPos, towerInstantiateZ);
                    currentTowerHolding.transform.position = snappedPosition;

                    towerGrid.setGridPositionOccupied(gridPosition, true);
                } else
                {
                    Destroy(currentTowerHolding);
                }

                
                currentTowerHolding = null;
            }
        }
    }

    void SetupWaves()
    {
        Dictionary<GameObject, int> wave1Enemies = new Dictionary<GameObject, int>();

        wave1Enemies.Add(redEnemy, 10);
        wave1Enemies.Add(blueEnemy, 5);

        Wave wave1 = new Wave(wave1Enemies, 5);

        Dictionary<GameObject, int> wave2Enemies = new Dictionary<GameObject, int>();

        wave2Enemies.Add(redEnemy, 20);
        wave2Enemies.Add(blueEnemy, 15);

        Wave wave2 = new Wave(wave2Enemies, 8);

        waves = new Wave[] { wave1, wave2 };
    }

    IEnumerator SpawnWaves()
    {
        foreach(Wave wave in waves) {
            // single wave
            while(wave.HasEnemies)
            {
                List<GameObject> nextSet = wave.getNextSpawn();

                // single tick
                foreach(GameObject prefab in nextSet)
                {
                    // single spawn
                    Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    yield return new WaitForSeconds(timePerTick / nextSet.Count);
                }

                yield return new WaitForSeconds(timeBetweenTicks);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // draw paths
        GameObject last = null;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < enemyPath.Length; i++)
        {
            GameObject current = enemyPath[i];

            if (last != null)
            {
                Debug.DrawLine(last.transform.position, current.transform.position, Color.red);
            }

            last = current;

            Gizmos.DrawWireCube(current.transform.position, Vector3.one * .2f);
        }

        // draw wave info

        Gizmos.color = Color.white;

        // draw tower grid info

        if(towerGrid != null)
        {
            for(int x = 0; x < towerGrid.Width + 1; x++)
            {
                Debug.DrawLine(towerGrid.toWorldPosition(new TowerGrid.Position(x, 0)), towerGrid.toWorldPosition(new TowerGrid.Position(x, towerGrid.Height)), Color.green);
            }

            for (int y = 0; y < towerGrid.Height + 1; y++)
            {
                Debug.DrawLine(towerGrid.toWorldPosition(new TowerGrid.Position(0, y)), towerGrid.toWorldPosition(new TowerGrid.Position(towerGrid.Width, y)), Color.green);
            }

            for(int y = 0; y < towerGrid.Height; y++)
            {
                for(int x = 0; x < towerGrid.Width; x++)
                {
                    if(towerGrid.isGridPosDisabled(new TowerGrid.Position(x, y)))
                    {
                        DrawX(towerGrid.toWorldPosition(new TowerGrid.Position(x, y)), towerGrid.Scale, towerGrid.Scale, Color.red);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw an X shape
    /// </summary>
    /// <param name="corner">Bottom-left corner of the X in world space</param>
    /// <param name="width">Width of the X in world space</param>
    /// <param name="height">Height of the X in world space</param>
    /// <param name="color">Color of the X</param>
    void DrawX(Vector3 corner, float width, float height, Color color)
    {
        float x1 = corner.x;
        float x2 = corner.x + width;

        float y1 = corner.y;
        float y2 = corner.y + height;

        Debug.DrawLine(new Vector3(x1, y1), new Vector3(x2, y2), color);
        Debug.DrawLine(new Vector3(x1, y2), new Vector3(x2, y1), color);
    }
#endif
}
