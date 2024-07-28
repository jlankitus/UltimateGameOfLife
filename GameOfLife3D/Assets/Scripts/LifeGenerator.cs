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
        return allGenerations;
    }

    public string GetGridState()
    {
        return gridManager.GetGridState();
    }
}
