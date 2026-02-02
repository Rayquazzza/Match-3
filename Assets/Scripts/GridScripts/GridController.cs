using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] public List<MatchPattern> AvailablePatterns = new List<MatchPattern>();

    private LevelData currentLevel;


    [Space(10)]
    [Header("CANDY MANAGEMENT")]
    [SerializeField] private GameObject baseCandyPrefab;

    public GameObject BaseCandyPrefab
    {
        get { return baseCandyPrefab; }
    }

    [SerializeField] private float spacing = 1f;

    public float Spacing
    {
        get { return spacing; }
    }

    [SerializeField] private float shiftSpeed = 0.2f;

    public float ShiftSpeed
    {
        get { return shiftSpeed; }
    }


    [Space(10)]
    [Header("BACKGROUND")]
    [SerializeField] private GameObject bgTilePrefab;
    [SerializeField] private Transform bgParent;

    private bool isProcessing = false;

    public bool IsProcessing
    {
        get { return isProcessing; }
    }

    public GridData Grid { get; private set; }
    public MatchChecker Match { get; private set; }
    public GridSpawner Spawner { get; private set; }
    public MatchProcessor Processor { get; private set; }
    public GridVisualizer Visualizer { get; private set; }
    public GridShifter Shifter { get; private set; }
    public GridSwap Swap { get; private set; }

    private Vector2Int lastSwapPos;

    public Vector2Int LastSwapPos
    {
        get { return lastSwapPos; }
    }


    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }

    private void InitializeLevel(LevelData data)
    {
        currentLevel = data;
        Grid = new GridData(data.width, data.height);

        // --- Conversion du LevelSlot[] en bool[,] pour GridData ---
        //bool[,] activeMap = new bool[data.width, data.height];
        //for (int i = 0; i < data.grid.Length; i++)
        //{
        //    int x = i % data.width;
        //    int y = i / data.width;
        //    activeMap[x, y] = data.grid[i].isValid;
        //}
        //Grid.SetActiveGrid(activeMap);
        Visualizer = new GridVisualizer(this,data.width, data.height, spacing);
        Spawner = new GridSpawner(this, Grid);
        Processor = new MatchProcessor(this, Grid);
        Shifter = new GridShifter(this);
        Swap = new GridSwap(this);
        Match = new MatchChecker(data.width, data.height);

        Spawner.ResetGrid();

        //Setup level grid
        currentLevel.SetupGrid(this);

        SetupBackground();
    }

    /// <summary>
    /// Generate grid when game state changes to IN_GAME
    /// </summary>
    private void GameStateChanged(E_GameState state)
    {
        if (state == E_GameState.IN_GAME)
        {
            Spawner.GenerateGrid();
        }
    }


    /// <summary>
    /// Setup the background size according to the grid size
    /// </summary>
    private void SetupBackground()
    {
        // 1. Nettoyer l'ancien background
        if (bgParent != null)
        {
            foreach (Transform child in bgParent) Destroy(child.gameObject);
        }

        // 2. Générer le background case par case
        for (int i = 0; i < currentLevel.grid.Length; i++)
        {
            LevelSlot slot = currentLevel.grid[i];

            // IMPORTANT : On ne crée un fond que si la case est VALID (pas un trou)
            if (slot.isValid)
            {
                // On convertit l'index 1D en coordonnées X,Y
                int x = i % currentLevel.width;
                int y = i / currentLevel.width;

                Vector3 pos = Visualizer.GetWorldPosition(x, y);
                pos.z = 1f; // On le met derrière les bonbons

                GameObject bg = Instantiate(bgTilePrefab, pos, Quaternion.identity);

                if (bgParent != null) bg.transform.SetParent(bgParent);
            }
        }
    }


    /// <summary>
    /// Change isProcessing bool value to indicate if the grid is currently processing matches or shifts
    /// </summary>
    public void SetIsProcessing(bool value)
    {
        isProcessing = value;
    }

    public void SetLastSwapPos(Vector2Int pos)
    {
        lastSwapPos = pos;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }


    // Subscribe and Unsubscribe to necessary events
    private void SubscribeToEvents()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
        GameServiceLocator.Get<ILevelService>().OnLoadLevelData += InitializeLevel;
    }

    private void UnsubscribeFromEvents()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged -= GameStateChanged;
        GameServiceLocator.Get<ILevelService>().OnLoadLevelData -= InitializeLevel;
    }
}


