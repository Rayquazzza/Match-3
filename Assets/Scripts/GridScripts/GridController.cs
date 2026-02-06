using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Space(10)]
    [Header("MATCH PATTERNS")]
    [SerializeField] public List<MatchPattern> AvailablePatterns = new List<MatchPattern>();

    public LevelData CurrentLevel { get; private set; }


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
        Debug.Log("Initializing Level: " + data.name);
        CurrentLevel = data;
        Grid = new GridData(data.width, data.height);

        bool[,] activeMap = new bool[data.width, data.height];
        for (int i = 0; i < data.grid.Length; i++)
        {
            int x = i % data.width;
            int y = i / data.width;
            activeMap[x, y] = data.grid[i].isValid;
        }
        
        Grid.SetActiveCells(activeMap);

        if (Grid.Width <= 0 || Grid.Height <= 0)
        {
            Debug.LogWarning("[GridController] La taille de la grille est à 0 ! Annulation de la génération.");
            return;
        }


        Visualizer = new GridVisualizer(this,data.width, data.height, spacing);
        Spawner = new GridSpawner(this, Grid);
        Processor = new MatchProcessor(this, Grid);
        Shifter = new GridShifter(this);
        Swap = new GridSwap(this);
        Match = new MatchChecker(data.width, data.height);

        Spawner.ResetGrid();


        SetupBackground();

        Camera.main.GetComponent<CameraFitter>().FitCameraToGrid(data.width, data.height,spacing);
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
        if (bgParent != null)
        {
            foreach (Transform child in bgParent) Destroy(child.gameObject);
        }

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                if (Grid.IsValidPos(x, y))
                {
                    Vector3 pos = Visualizer.GetWorldPosition(x, y);
                    pos.z = 1f;
                    GameObject bg = Instantiate(bgTilePrefab, pos, Quaternion.identity, bgParent);

                    float padding = 1f; 
                    bg.transform.localScale = new Vector3(spacing * padding, spacing * padding, 1);

                    bool up = Grid.IsValidPos(x, y + 1);
                    bool down = Grid.IsValidPos(x, y - 1);
                    bool left = Grid.IsValidPos(x - 1, y);
                    bool right = Grid.IsValidPos(x + 1, y);

                    bool tl = Grid.IsValidPos(x - 1, y + 1);
                    bool tr = Grid.IsValidPos(x + 1, y + 1);
                    bool bl = Grid.IsValidPos(x - 1, y - 1);
                    bool br = Grid.IsValidPos(x + 1, y - 1);

                    BackgroundTile tileScript = bg.GetComponent<BackgroundTile>();
                    if (tileScript != null)
                    {
                        tileScript.UpdateVisual(up, down ,left, right, tl, tr, bl, br);
                    }
                }
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
        if (Swap != null)
        {
            Swap.Dispose();
        }
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


