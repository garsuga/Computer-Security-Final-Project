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
    public static LineRenderer DrawCircle(GameObject container, Vector3 offset, float radius, float lineWidth, Color color, bool useWorldSpace)
    {
        int segments = 360;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius);
        }

        return DrawPath(container, offset, points, lineWidth, color, useWorldSpace);
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

        return DrawPath(container, offset, points, lineWidth, color, useWorldSpace);
    }

    /// <summary>
    /// Creates a LineRenderer component in the given GameObject with the following positions
    /// </summary>
    /// <param name="container">GameObject to create LineRenderer on</param>
    /// <param name="positions">Positions of LineRenderer</param>
    /// <param name="lineWidth">Line width in relative space</param>
    /// <returns></returns>
    public static LineRenderer DrawPath(GameObject container, Vector3 offset, Vector3[] positions, float lineWidth, Color color, bool useWorldSpace)
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

    public static GameObject EmitText(GameObject container, Vector3 offset, string text, float lifetime, Color color, int fontSize, Vector3 velocity)
    {
        GameObject emittedText = Instantiate<GameObject>(instance.emittedTextPrefab);
        emittedText.transform.parent = container != null ? container.transform : null;
        emittedText.transform.localPosition = offset;

        EmittedTextBehavior emittedController = emittedText.GetComponent<EmittedTextBehavior>();
        emittedController.text = text;
        emittedController.lifetimeSeconds = lifetime;
        emittedController.color = color;
        emittedController.fontSize = fontSize;
        emittedController.velocity = velocity;
        return emittedText;
    }

    public static GameObject ShowWarning(string text)
    {
        GameObject warning = Instantiate<GameObject>(GameController.instance.warningPrefab);
        WarningController controller = warning.GetComponent<WarningController>();
        controller.SetWarningText(text).FadeIn(3);
        warning.transform.parent = null;
        warning.transform.position = new Vector3(0,0,-3);
        return warning;
    }

    public GameObject AlertText(string text, Color color)
    {
        return EmitText(this.waveAlertTransform, Vector3.zero, text, 1f, color, 50, new Vector3(0, .5f));
    }

    public static GameController instance;

    public GameController(): base()
    {
        instance = this;
    }

    [Header("Enemy References")]
    public GameObject virusEnemy;
    public GameObject phishingEnemy;
    public GameObject spamEnemy;
    public GameObject mitmEnemy;

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
    public GameObject waveAlertTransform;
    public GameObject warningPrefab;

    [Header("Difficulty Settings")]
    public float enemyScaleBase = 1.2f;
    public float speedScaleBase = 1.2f;
    public float tickScaleBase = 0.8f;

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

    public delegate void Callback();

    private TowerGrid towerGrid;
    private MouseObserverBevahior mouseObserver;
    public GameObject[] enemyPath = new GameObject[0];
    public Dictionary<int, WaveSetting[]> waveMap;
    public Dictionary<int, Callback> waveEvents;
    private Camera mainCamera;
    public int waveNum = 0;

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
        mouseObserver.OnMouseClick += MouseClick;

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
        DrawPath(gridOrigin, Vector3.zero, positions, .02f, Color.red, true);
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

    void MouseClick(int button, Vector3 pressScreenPos, Vector3 releaseScreenPos)
    {
        if (currentTowerHolding != null)
            Destroy(currentTowerHolding);
        currentTowerHolding = null;
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
        /**
         * Dictionary int -> Wave...
         * 
         * The integer is the first wave which can spawn from each group.
         * This is used to lock difficult attacks behind time restrictions.
         * 
         * Each wave has a Dictionary GameObject -> int
         * The integer is the count of enemies and the GameObject is the prefab to spawn.
         * Each wave accepts a 'enemies per tick' and Callable as well.
         * 
         * Callable is optional and allows code to be run each time that wave starts.
         * The 'enemies per tick' dictates the relative rate enemies spawn.
         * Ticks are scaled with difficulty and become shorter.
         */
        waveMap = new Dictionary<int, WaveSetting[]>()
        {
            { 0, new WaveSetting[]
                {
                    new WaveSetting("large-virus", new Dictionary<GameObject, int>()
                    {
                        { virusEnemy, 20 }
                    }, 5, null),
                    new WaveSetting("large-virus-mitm", new Dictionary<GameObject, int>()
                    {
                        { virusEnemy, 20 },
                        { mitmEnemy, 1 }
                    }, 4, null),
                }
            },
            { 5, new WaveSetting[]
                {
                    new WaveSetting("small-virus-mitm-phishing", new Dictionary<GameObject, int>()
                    {
                        { mitmEnemy, 1 },
                        { virusEnemy, 10 },
                        { phishingEnemy, 1 }
                    }, 5, null)
                }
            },
            { 7, new WaveSetting[]
                {
                    new WaveSetting("large-phishing", new Dictionary<GameObject, int>()
                    {
                        { phishingEnemy, 10 }
                    }, 3, () => AlertText("Phishing Scams!", Color.red))
                }
            },
            { 10, new WaveSetting[]
                {
                    new WaveSetting("large-ddos", new Dictionary<GameObject, int>()
                    {
                        { spamEnemy, 40 }
                    }, 10, () => AlertText("DDoS Attack!", Color.red)),
                }
            },
            { 15, new WaveSetting[]
                {
                    new WaveSetting("medium-ddos-small-virus-phishing", new Dictionary<GameObject, int>()
                    {
                        { virusEnemy, 10 },
                        { spamEnemy, 30 },
                        { phishingEnemy, 10 }
                    }, 8, () => AlertText("DDoS Attack!", Color.red))
                }
            }
        };

        waveEvents = new Dictionary<int, Callback>()
        {
            { 4, () => ShowWarning("Phishing attacks being wave 5.\n\nPurchase a Phishing Detection Tower.") },
            { 8, () => ShowWarning("DDoS attacks begin wave 10.\n\nPuchase Mitigation Towers.") }
        };
    }

    private List<WaveSetting> AggregateWaves(int nextWaveNum)
    {
        List<WaveSetting> selectedWaves = new List<WaveSetting>();
        foreach (var kvp in waveMap)
        {
            if(kvp.Key <= nextWaveNum)
            {
                selectedWaves.AddRange(kvp.Value);
            }
        }

        return selectedWaves;
    }

    private WaveSetting SelectWave(int waveNum)
    {
        List<WaveSetting> agg = AggregateWaves(waveNum);
        return agg[UnityEngine.Random.Range(0, agg.Count - 1)];
    }

    private Queue<GameObject> enemiesSpawnedInWave = new Queue<GameObject>();

    IEnumerator SpawnWaves()
    {
        waveNum = 0;

        while(true)
        {
            waveNum++;

            if(waveEvents.ContainsKey(waveNum))
            {
                waveEvents[waveNum].Invoke();
            }

            WaveSetting wave = SelectWave(waveNum);

            Debug.Log("Begin Wave " + waveNum + " (" + wave.Name + ")");

            OnRoundBegin?.Invoke(waveNum);
            waveInfoController.WaveNum = waveNum;

            //AlertText(wave.Name, Color.magenta);

            wave.WaveCallable?.Invoke();

            ScaledWave scaledWave = new ScaledWave(wave, Mathf.Pow(enemyScaleBase, waveNum - 1));

            while (scaledWave.HasEnemies)
            {
                List<GameObject> nextSet = scaledWave.GetNextSpawn();

                // single tick
                foreach (GameObject prefab in nextSet)
                {
                    // single spawn
                    GameObject newEnemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    enemiesSpawnedInWave.Enqueue(newEnemy);

                    yield return new WaitForSeconds((timePerTick * Mathf.Pow(tickScaleBase, waveNum - 1)) / nextSet.Count);
                }

                yield return new WaitForSeconds(timeBetweenTicks * Mathf.Pow(tickScaleBase, waveNum - 1));
            }

            while (enemiesSpawnedInWave.Count > 0)
            {
                while (enemiesSpawnedInWave.Count > 0 && enemiesSpawnedInWave.Peek() == null)
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
