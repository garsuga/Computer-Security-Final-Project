using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// From https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
    /// </summary>
    /// <param name="container">GameObject to create LineRenderer on</param>
    /// <param name="radius">Circle radius in relative space</param>
    /// <param name="lineWidth">Line width in relative space</param>
    public static LineRenderer DrawCircle(GameObject container, float radius, float lineWidth, Color color, bool useWorldSpace)
    {
        int segments = 360;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius);
        }

        return DrawPath(container, points, lineWidth, color, useWorldSpace);
    }

    /// <summary>
    /// Draws a square around the game object with the given radius
    /// </summary>
    /// <param name="container">GameObject to create LineRenderer on</param>
    /// <param name="offset">Offset to add to the square</param>
    /// <param name="radius">Radius of square in relative space</param>
    /// <param name="lineWidth">Line width in relative space</param>
    /// <returns></returns>
    public static LineRenderer DrawSquare(GameObject container, Vector3 offset, float radius, float lineWidth, Color color, bool useWorldSpace)
    {
        Vector3[] points = new Vector3[] { 
            new Vector3(-radius, -radius) + offset,
            new Vector3(radius, -radius) + offset,
            new Vector3(radius, radius) + offset,
            new Vector3(-radius, radius) + offset,
            new Vector3(-radius, -radius) + offset
        };

        return DrawPath(container, points, lineWidth, color, useWorldSpace);
    }

    /// <summary>
    /// Creates a LineRenderer component in the given GameObject with the following positions
    /// </summary>
    /// <param name="container">GameObject to create LineRenderer on</param>
    /// <param name="positions">Positions of LineRenderer</param>
    /// <param name="lineWidth">Line width in relative space</param>
    /// <returns></returns>
    public static LineRenderer DrawPath(GameObject container, Vector3[] positions, float lineWidth, Color color, bool useWorldSpace)
    {
        GameObject containerVirtChild = new GameObject("LineRenderer Holder");
        containerVirtChild.transform.parent = container.transform;
        containerVirtChild.transform.localPosition = Vector3.zero;

        LineRenderer line = containerVirtChild.AddComponent<LineRenderer>();
        line.useWorldSpace = useWorldSpace;
        line.positionCount = positions.Length;

        line.startColor = color;
        line.endColor = color;

        line.SetPositions(positions);
        line.material = GameController.instance.lineRendererMaterial;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, lineWidth);
        curve.AddKey(1, lineWidth);

        line.widthCurve = curve;

        return line;
    }

    public static GameObject EmitText(GameObject container, string text, float lifetime, Color color, int fontSize, Vector3 velocity)
    {
        GameObject emittedText = Instantiate<GameObject>(instance.emittedTextPrefab);
        emittedText.transform.parent = container.transform;
        emittedText.transform.localPosition = Vector3.zero;

        EmittedTextBehavior emittedController = emittedText.GetComponent<EmittedTextBehavior>();
        emittedController.text = text;
        emittedController.lifetimeSeconds = lifetime;
        emittedController.color = color;
        emittedController.fontSize = fontSize;
        emittedController.velocity = velocity;
        return emittedText;
    }

    public static GameController instance;

    public GameController(): base()
    {
        instance = this;
    }

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

    [Header("Player Properties")]
    public int _money = 10000;
    public PlayerHealth playerHealth;
    public Text playerMoneyTextElement;

    [Header("Game Over Settings")]
    public string gameOverSceneName = "FailedScene";

    [Header("Display Settings")]
    public Material lineRendererMaterial;
    public GameObject emittedTextPrefab;
    public WaveInfoController waveInfoController;

    public int Money
    {
        get
        {
            return _money;
        }

        set
        {
            int oldMoney = _money;
            _money = value;

            OnMoneyChanged?.Invoke(oldMoney, _money);
        }
    }

    private TowerGrid towerGrid;
    private MouseObserverBevahior mouseObserver;
    public GameObject[] enemyPath = new GameObject[0];
    public Wave[] waves = new Wave[0];
    private Camera mainCamera;

    private GameObject currentTowerHolding;

    public delegate void RoundEvent(int roundNum);
    public delegate void EnemyExitedEvent(GameObject whoExited);
    public delegate void MoneyChangedEvent(int oldMoney, int newMoney);

    public RoundEvent OnRoundEnd
    {
        get;set;
    }

    public RoundEvent OnRoundBegin
    {
        get;set;
    }

    public EnemyExitedEvent OnEnemyExited
    {
        get;set;
    }

    public MoneyChangedEvent OnMoneyChanged
    {
        get;set;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        OnEnemyExited += (whoExited) => playerHealth.AdjustHealth();
        PlayerHealth.OnHealthChanged += (newHealth) =>
        {
            if (newHealth <= 0)
            {
                SceneManager.LoadScene(gameOverSceneName);
            }
        };

        OnMoneyChanged += (oldMoney, newMoney) => playerMoneyTextElement.text = ((Int32)newMoney).ToString("C");
        Money = Money;

        towerGrid = new TowerGrid(gridWidth, gridHeight, gridScale, gridOrigin.transform.position, gridDisabledPositions);

        mouseObserver = GetComponent<MouseObserverBevahior>();

        mouseObserver.OnMouseDrag += MouseDrag;
        mouseObserver.OnMousePress += MousePress;

        SetupWaves();
        StartCoroutine("SpawnWaves");

        CreateTowerPlacementLineRenderers();
        CreateEnemyPathLineRenderer();
    }

    private void CreateTowerPlacementLineRenderers()
    {
        for(int x = 0; x < towerGrid.Width; x++)
        {
            for (int y = 0; y < towerGrid.Height; y++) {
                if(!towerGrid.isGridPosDisabled(new TowerGrid.Position(x, y)))
                {
                    DrawSquare(gridOrigin, towerGrid.toWorldPosition(new TowerGrid.Position(x, y)) + new Vector3(.5f, .5f, 0), .5f, .02f, Color.green, true);
                }
            }
        }
    }

    private void CreateEnemyPathLineRenderer()
    {
        Vector3[] positions = new Vector3[enemyPath.Length];
        for(int i = 0; i < enemyPath.Length; i++)
        {
            positions[i] = enemyPath[i].transform.position;
        }
        DrawPath(gridOrigin, positions, .02f, Color.red, true);
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
                TowerCanBeLiftedBehavior canBeLiftedBehavior = tower.GetComponent<TowerCanBeLiftedBehavior>();
                if (canBeLiftedBehavior != null && canBeLiftedBehavior.enabled)
                {
                    GameObject tempTower = tower;
                    while(tempTower.transform.parent != null)
                    {
                        tempTower = tempTower.transform.parent.gameObject;
                    }
                    hits.Add(tempTower);
                }
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
            GameObject clickedTower = clickedTowers[0];
            TowerBehavior _preTowerBehavior = clickedTower.GetComponentInChildren<TowerBehavior>();
            if(_preTowerBehavior != null && _preTowerBehavior.towerCost <= Money)
            {
                // TODO: Evaluate
                currentTowerHolding = Instantiate<GameObject>(clickedTowers[0], virtualMouseGameObject.transform);
                currentTowerHolding.transform.localPosition = Vector3.zero;

                TowerBehavior towerBehavior = currentTowerHolding.GetComponentInChildren<TowerBehavior>();
                towerBehavior.enabled = false;
            }
        }
    }

    void MouseDrag(int button, Vector3 startPosScreen, Vector3 currentPosScreen, float time, bool isOver)
    {
        if (currentTowerHolding != null) {
            if (isOver)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(currentPosScreen);

                TowerGrid.Position gridPosition = towerGrid.toGridPosition(worldPos);

                if (gridPosition != null && !towerGrid.isGridPosDisabled(gridPosition) && !towerGrid.isGridPosOccupied(gridPosition))
                {
                    // can place tower
                    TowerBehavior towerBehavior = currentTowerHolding.GetComponentInChildren<TowerBehavior>();
                    towerBehavior.enabled = true;
                    towerBehavior.isPlaced = true;
                    towerBehavior.OnPlaced?.Invoke(currentTowerHolding);

                    currentTowerHolding.transform.parent = null;

                    Vector3 snappedPosition = towerGrid.snapToGrid(worldPos, towerInstantiateZ);
                    currentTowerHolding.transform.position = snappedPosition;

                    towerGrid.setGridPositionOccupied(gridPosition, true);

                    Money -= towerBehavior.towerCost;
                    ///
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

    private Queue<GameObject> enemiesSpawnedInWave = new Queue<GameObject>();

    IEnumerator SpawnWaves()
    {
        int waveNum = 0;
        foreach(Wave wave in waves) {
            waveNum++;
            OnRoundBegin?.Invoke(waveNum);
            waveInfoController.WaveNum = waveNum;
            // single wave
            while(wave.HasEnemies)
            {
                List<GameObject> nextSet = wave.getNextSpawn();

                // single tick
                foreach(GameObject prefab in nextSet)
                {
                    // single spawn
                    GameObject newEnemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    enemiesSpawnedInWave.Enqueue(newEnemy);

                    yield return new WaitForSeconds(timePerTick / nextSet.Count);
                }

                yield return new WaitForSeconds(timeBetweenTicks);
            }

            while(enemiesSpawnedInWave.Count > 0)
            {
                while(enemiesSpawnedInWave.Count > 0 && enemiesSpawnedInWave.Peek() == null)
                {
                    enemiesSpawnedInWave.Dequeue();
                }

                yield return new WaitForSeconds(.5f);
            }

            OnRoundEnd?.Invoke(waveNum);

            waveInfoController.StartTimer(timeBetweenWaves);

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
                Debug.DrawLine(last.transform.position, current.transform.position, Color.cyan);
            }

            last = current;

            Gizmos.DrawWireCube(current.transform.position, Vector3.one * .2f);
        }

        // draw wave info

        Gizmos.color = Color.white;

        // draw tower grid info

        if(towerGrid != null)
        {
            /*for(int x = 0; x < towerGrid.Width + 1; x++)
            {
                Debug.DrawLine(towerGrid.toWorldPosition(new TowerGrid.Position(x, 0)), towerGrid.toWorldPosition(new TowerGrid.Position(x, towerGrid.Height)), Color.green);
            }

            for (int y = 0; y < towerGrid.Height + 1; y++)
            {
                Debug.DrawLine(towerGrid.toWorldPosition(new TowerGrid.Position(0, y)), towerGrid.toWorldPosition(new TowerGrid.Position(towerGrid.Width, y)), Color.green);
            }*/

            for(int y = 0; y < towerGrid.Height; y++)
            {
                for(int x = 0; x < towerGrid.Width; x++)
                {
                    if(!towerGrid.isGridPosDisabled(new TowerGrid.Position(x, y)))
                    {
                        DrawSquare(towerGrid.toWorldPosition(new TowerGrid.Position(x, y)), towerGrid.Scale, towerGrid.Scale, Color.green);
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

    void DrawSquare(Vector3 corner, float width, float height, Color color)
    {
        float x1 = corner.x;
        float x2 = corner.x + width;

        float y1 = corner.y;
        float y2 = corner.y + height;

        Debug.DrawLine(new Vector3(x1, y1), new Vector3(x2, y1), color);
        Debug.DrawLine(new Vector3(x2, y1), new Vector3(x2, y2), color);
        Debug.DrawLine(new Vector3(x2, y2), new Vector3(x1, y2), color);
        Debug.DrawLine(new Vector3(x1, y2), new Vector3(x1, y1), color);
    }
#endif
}
