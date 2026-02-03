using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateGridEditorWindow : EditorWindow
{
    private LevelData currentLevelData;

    private GridItemData selectedBase;
    private OverlayItemData selectedOverlay;

    private List<GridItemData> allBaseItems = new List<GridItemData>();
    private List<OverlayItemData> allOverlays = new List<OverlayItemData>();
    private bool showOverlays = true;

    private Vector2 scrollPos;
    private Vector2 paletteScrollPos;

    private int tempWidth;
    private int tempHeight;
    private bool dimensionsInitialized = false;

    private enum EditLayer { Base, Overlay }
    private EditLayer currentLayer = EditLayer.Base;

    private GUISkin myCustomSkin;

    [MenuItem("Tools/Level Editor")]
    public static void OpenWindow()
    {
        CreateGridEditorWindow window = GetWindow<CreateGridEditorWindow>("Level Editor");
        window.minSize = new Vector2(450, 600);
        window.Show();
        window.LoadAllItems();
    }


    private void OnEnable()
    {
        myCustomSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor/GUISkin/MyEditorSkin.guiskin");
    }


    private void LoadAllItems()
    {
        allBaseItems.Clear();
        allOverlays.Clear();

        string[] baseGuids = AssetDatabase.FindAssets("t:GridItemData");
        foreach (var guid in baseGuids)
            allBaseItems.Add(AssetDatabase.LoadAssetAtPath<GridItemData>(AssetDatabase.GUIDToAssetPath(guid)));

        string[] overlayGuids = AssetDatabase.FindAssets("t:OverlayItemData");
        foreach (var guid in overlayGuids)
            allOverlays.Add(AssetDatabase.LoadAssetAtPath<OverlayItemData>(AssetDatabase.GUIDToAssetPath(guid)));
    }

    public static void OpenWithConfig(LevelData data)
    {
        CreateGridEditorWindow window = GetWindow<CreateGridEditorWindow>("Level Editor");
        window.currentLevelData = data;
        window.dimensionsInitialized = false;
        window.Show();
    }

    private void OnGUI()
    {

        Rect windowRect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(windowRect, new Color(0.12f, 0.12f, 0.12f));

        if (myCustomSkin != null)
        {
            GUI.skin = myCustomSkin;
        }


        DrawHeader();

        if (currentLevelData == null)
        {
            currentLevelData = (LevelData)EditorGUILayout.ObjectField("Niveau à éditer", currentLevelData, typeof(LevelData), false);
            if (GUILayout.Button("Rafraîchir la liste des Items")) LoadAllItems();
            return;
        }

        DrawConfigSection();
        EditorGUILayout.Space(10);
        DrawVisualPalette();
        EditorGUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawGrid();
        EditorGUILayout.EndScrollView();

        if (Event.current.type == EventType.MouseDrag) Repaint();
    }

    private void DrawVisualPalette()
    {
        EditorGUILayout.BeginVertical("window");

        EditorGUILayout.BeginHorizontal();

        // overlay visibility
        string eyeIcon = showOverlays ? "d_VisibilityOn" : "d_VisibilityOff";
        if (GUILayout.Button(EditorGUIUtility.IconContent(eyeIcon), GUILayout.Width(35), GUILayout.Height(25)))
        {
            showOverlays = !showOverlays;
        }

        // Refresh button
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh"), GUILayout.Width(35), GUILayout.Height(25)))
        {
            LoadAllItems();
        }

        // LayerMode Toolbar
        currentLayer = (EditLayer)GUILayout.Toolbar((int)currentLayer, new string[] { "Base Layer", "Overlay Layer" }, GUILayout.Height(25));

        EditorGUILayout.EndHorizontal();

        // Asset List
        paletteScrollPos = EditorGUILayout.BeginScrollView(paletteScrollPos, GUILayout.Height(95));
        EditorGUILayout.BeginHorizontal();

        if (currentLayer == EditLayer.Base)
        {
            DrawPaletteButton(null); // Eraser
            foreach (var item in allBaseItems) DrawPaletteButton(item);
        }
        else
        {
            DrawPaletteButton(null); // Eraser
            foreach (var item in allOverlays) DrawPaletteButton(item);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draw a button for the palette representing a GridItemData or OverlayItemData.
    /// </summary>
    private void DrawPaletteButton(ScriptableObject item)
    {
        bool isSelected = false;
        Texture2D iconTex = null;

        if (item == null)
            isSelected = (currentLayer == EditLayer.Base ? selectedBase == null : selectedOverlay == null);
        else
        {
            isSelected = (item == selectedBase || item == selectedOverlay);
            if (item is GridItemData b) iconTex = AssetPreview.GetAssetPreview(b.icon);
            else if (item is OverlayItemData o) iconTex = AssetPreview.GetAssetPreview(o.icon);
        }

        if (isSelected) GUI.backgroundColor = new Color(0.2f, 0.6f, 1f);

        GUIContent content = new GUIContent(iconTex);
        if (item == null) content.text = "Gomme";

        if (GUILayout.Button(content, GUILayout.Width(65), GUILayout.Height(65)))
        {
            if (currentLayer == EditLayer.Base) selectedBase = item as GridItemData;
            else selectedOverlay = item as OverlayItemData;
        }
        GUI.backgroundColor = Color.white;
    }


    /// <summary>
    /// Draw the editable grid based on the current LevelData.
    /// </summary>
    private void DrawGrid()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical("box");
        for (int y = currentLevelData.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < currentLevelData.width; x++)
            {
                DrawTile(y * currentLevelData.width + x);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw a single tile in the grid at the specified index.
    /// </summary>
    private void DrawTile(int index)
    {
        LevelSlot slot = currentLevelData.grid[index];
        Rect tileRect = GUILayoutUtility.GetRect(55, 55);

        Event e = Event.current;

        // --- CLICK AND DRAG LOGIC ---


        if (tileRect.Contains(e.mousePosition))
        {
            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                Undo.RecordObject(currentLevelData, "Paint Tile");

                if (e.button == 1) // Right click to erase
                {
                    
                    if (currentLayer == EditLayer.Base)
                    {
                        if (slot.baseItem != null)
                        {
                            
                            slot.baseItem = null;
                        }
                        else
                        {
                            slot.isValid = false;
                        }
                    }
                    else 
                    {
                        slot.overlayItem = null;
                    }
                }
                else if (e.button == 0) // Left click to paint
                {

                    slot.isValid = true;

                    if (currentLayer == EditLayer.Base) slot.baseItem = selectedBase;
                    else slot.overlayItem = selectedOverlay;
                }

                EditorUtility.SetDirty(currentLevelData);
                e.Use();
            }
        }

        // --- VISUALS ---


        if (!slot.isValid)
        {
            EditorGUI.DrawRect(tileRect, new Color(0.1f, 0.1f, 0.1f, 0.5f)); 
            return; 
        }

        GUI.Box(tileRect, "", GUI.skin.button);

        float padding = 8f;
        Rect innerRect = new Rect(tileRect.x + padding, tileRect.y + padding, tileRect.width - (padding * 2), tileRect.height - (padding * 2));

        if (slot.baseItem != null && slot.baseItem.icon != null)
        {
            GUI.DrawTexture(innerRect, AssetPreview.GetAssetPreview(slot.baseItem.icon), ScaleMode.ScaleToFit);
        }
        else
        {
            GUI.Label(innerRect, "Random", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 9, normal = { textColor = Color.gray } });
        }

        if (showOverlays && slot.overlayItem != null && slot.overlayItem.icon != null)
        {
            float overlayPadding = padding + 4f;
            Rect overlayRect = new Rect(tileRect.x + overlayPadding, tileRect.y + overlayPadding, tileRect.width - (overlayPadding * 2), tileRect.height - (overlayPadding * 2));
            GUI.DrawTexture(overlayRect, AssetPreview.GetAssetPreview(slot.overlayItem.icon), ScaleMode.ScaleToFit);
        }
    }


    /// <summary>
    /// Draw the configuration section of the editor window.
    /// </summary>
    private void DrawConfigSection()
    {
        GUILayout.BeginVertical("box");

        DrawSectionTitle("CONFIGURATION", "SettingsIcon");

        currentLevelData = (LevelData)EditorGUILayout.ObjectField("Fichier Cible", currentLevelData, typeof(LevelData), false, GUILayout.Height(20));

        GUILayout.BeginHorizontal();

        GUILayout.Label("Largeur :", GUILayout.Width(60));
        string wStr = GUILayout.TextField(tempWidth.ToString(), GUILayout.Width(50));
        int.TryParse(wStr, out tempWidth);

        GUILayout.Space(20);

        GUILayout.Label("Hauteur :", GUILayout.Width(60));
        string hStr = GUILayout.TextField(tempHeight.ToString(), GUILayout.Width(50));
        int.TryParse(hStr, out tempHeight);

        GUILayout.EndHorizontal();

        if (currentLevelData != null && (tempWidth != currentLevelData.width || tempHeight != currentLevelData.height))
        {
            if (GUILayout.Button("VALIDER LES NOUVELLES DIMENSIONS", GUILayout.Height(25)))
                ResizeGrid(tempWidth, tempHeight);
        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draw the header of the editor window.
    /// </summary>
    private void DrawHeader()
    {
        EditorGUILayout.Space(15);

        GUIStyle headerStyle = GUI.skin.GetStyle("HeaderTitle");
        if (headerStyle != null)
        {
            GUILayout.Label("🍭 MATCH-3 DESIGN", headerStyle);
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(0.4f, 1f, 0.4f);

            if (GUILayout.Button("💾 SAUVEGARDER LE NIVEAU", GUILayout.Width(200), GUILayout.Height(30)))
            {
                SaveCurrentLevel();

                GUI.FocusControl(null);
            }
            GUI.backgroundColor = Color.white;

            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
    }

    private void SaveCurrentLevel()
    {
        if (currentLevelData != null)
        {
            EditorUtility.SetDirty(currentLevelData);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=green>Succès :</color> Le niveau <b>{currentLevelData.name}</b> a été enregistré sur le disque !");
        }
    }


    /// <summary>
    /// Draw a section title with an icon.
    /// </summary>
    private void DrawSectionTitle(string title, string iconName)
    {
        EditorGUILayout.BeginHorizontal();
        GUIContent icon = EditorGUIUtility.IconContent(iconName);
        GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
        GUILayout.Label(title);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }

    /// <summary>
    /// Resize the grid in the current LevelData to the specified dimensions.
    /// </summary>
    private void ResizeGrid(int newWidth, int newHeight)
    {
        Undo.RecordObject(currentLevelData, "Resize Grid");
        LevelSlot[] oldGrid = currentLevelData.grid;
        int oldWidth = currentLevelData.width;
        int oldHeight = currentLevelData.height;

        currentLevelData.width = newWidth;
        currentLevelData.height = newHeight;
        currentLevelData.grid = new LevelSlot[newWidth * newHeight];

        for (int i = 0; i < currentLevelData.grid.Length; i++)
            currentLevelData.grid[i] = new LevelSlot();

        if (oldGrid != null)
        {
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    if (x < oldWidth && y < oldHeight)
                        currentLevelData.grid[y * newWidth + x] = oldGrid[y * oldWidth + x];
                }
            }
        }
        EditorUtility.SetDirty(currentLevelData);
        AssetDatabase.SaveAssets();
    }
}