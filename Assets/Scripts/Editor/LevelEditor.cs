using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelData data = (LevelData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Open Level Editor Window", GUILayout.Height(40)))
        {
            CreateGridEditorWindow.OpenWithConfig(data);
        }
        GUI.backgroundColor = Color.white;
    }
}