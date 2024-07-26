using UnityEngine;

[CreateAssetMenu(fileName = "NewLifePattern", menuName = "LifePattern")]
public class LifePattern : ScriptableObject
{
    public string patternName;
    public GameObject patternPrefab;
    public int width = 8;
    public int height = 8;
    public int[] patternGrid; // Flattened grid
    public Vector2Int startingPosition; // Specify the starting position

    public bool randomize;

    // Cool trick to default the patternName to the object name.
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(patternName))
        {
            patternName = name;
        }

        if (patternGrid == null || patternGrid.Length != width * height)
        {
            patternGrid = new int[width * height];
        }
    }

    public int[,] GetPatternGrid2D()
    {
        int[,] grid2D = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid2D[x, y] = patternGrid[y * width + x];
            }
        }
        return grid2D;
    }

    public void RandomizeGrid()
    {
        System.Random rnd = new System.Random();
        for (int i = 0; i < patternGrid.Length; i++)
        {
            patternGrid[i] = rnd.Next(2);
        }
    }
}
