using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeGenerator : MonoBehaviour
{
    [SerializeField]
    private List<LifePattern> patterns;

    [SerializeField]
    private LifePattern defaultPattern;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private int generations = 4;

    [SerializeField]
    private float animationTime = 1.0f; // Time between generations in seconds

    private List<GameObject> activeCells = new List<GameObject>();

    public string GeneratePattern(string patternName)
    {
        // Ignore case, and launch pattern, if a correct pattern name is supplied
        LifePattern pattern = patterns.Find(p => p.patternName.ToUpper() == patternName.ToUpper());
        if (pattern != null)
        {
            Debug.Log($"Generating pattern: {pattern.patternName}");
            return RenderPattern(pattern);
        }
        // If pattern name was not supplied, or incorrectly supplied, default to RANDOM
        else
        {
            Debug.LogWarning("Invalid pattern supplied, generating default pattern: " + defaultPattern.patternName);
            return RenderPattern(defaultPattern);
        }
    }

    public string RenderPattern(LifePattern parsedPattern)
    {
        Debug.Log("Rendering " + parsedPattern.patternName);
        gridManager.InitializeGrid(parsedPattern.GetPatternGrid2D(), parsedPattern.startingPosition);

        string allGenerations = gridManager.GetGridState() + "\n";

        for (int i = 1; i < generations; i++) // start at 1 because the initial state is the first generation
        {
            gridManager.NextGeneration();
            allGenerations += gridManager.GetGridState() + "\n";
        }

        allGenerations += "<EOF>"; // Add a delimiter to indicate the end of the response
        Debug.Log("All generations:\n" + allGenerations);

        // Call the new 3D rendering function after all generations are calculated
        StartCoroutine(AnimateGenerations3D(parsedPattern));

        return allGenerations;
    }

    public string GetGridState()
    {
        return gridManager.GetGridState();
    }

    // New 3D Animation Functionality
    private IEnumerator AnimateGenerations3D(LifePattern parsedPattern)
    {
        for (int gen = 0; gen < generations; gen++)
        {
            RenderGrid3D(parsedPattern);
            yield return new WaitForSeconds(animationTime);
            gridManager.NextGeneration();
        }
        RenderGrid3D(parsedPattern); // Ensure final generation is rendered
    }

    private void RenderGrid3D(LifePattern parsedPattern)
    {
        int[,] finalGrid = gridManager.GetGrid2DArray();

        // Clear previous cells
        foreach (var cell in activeCells)
        {
            Destroy(cell);
        }
        activeCells.Clear();

        for (int x = 0; x < finalGrid.GetLength(0); x++)
        {
            for (int y = 0; y < finalGrid.GetLength(1); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject cell;

                if (finalGrid[x, y] == 1)
                {
                    cell = Instantiate(parsedPattern.alivePrefab, position, Quaternion.identity);
                }
                else
                {
                    cell = Instantiate(parsedPattern.deadPrefab, position, Quaternion.identity);
                }

                activeCells.Add(cell);
            }
        }
    }
}
