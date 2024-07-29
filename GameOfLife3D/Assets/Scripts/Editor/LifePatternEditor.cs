using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LifePattern))]
public class LifePatternEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LifePattern lifePattern = (LifePattern)target;

        lifePattern.patternName = EditorGUILayout.TextField("Pattern Name", lifePattern.patternName);
        lifePattern.startingPosition = EditorGUILayout.Vector2IntField("Starting Position", lifePattern.startingPosition);
        lifePattern.width = EditorGUILayout.IntField("Width", lifePattern.width);
        lifePattern.height = EditorGUILayout.IntField("Height", lifePattern.height);
        lifePattern.alivePrefab = (GameObject)EditorGUILayout.ObjectField("Alive Prefab", lifePattern.alivePrefab, typeof(GameObject), false);
        lifePattern.deadPrefab = (GameObject)EditorGUILayout.ObjectField("Dead Prefab", lifePattern.deadPrefab, typeof(GameObject), false);

        if (lifePattern.patternGrid == null || lifePattern.patternGrid.Length != lifePattern.width * lifePattern.height)
        {
            lifePattern.patternGrid = new int[lifePattern.width * lifePattern.height];
        }

        for (int y = 0; y < lifePattern.height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < lifePattern.width; x++)
            {
                lifePattern.patternGrid[y * lifePattern.width + x] = EditorGUILayout.IntField(lifePattern.patternGrid[y * lifePattern.width + x], GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Randomize Grid"))
        {
            lifePattern.RandomizeGrid();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(lifePattern);
        }
    }
}
