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

    // 'animationTime' between generations, 'scaleTime' for scaling transition,
    // 'fadeCurve' adds style, linear is out of fashion. Small stagger delay for even more style points
    [SerializeField]
    private float animationTime = 1.0f;
    private float scaleTime = 0.5f;
    [SerializeField]
    private AnimationCurve fadeCurve;
    [SerializeField]
    private float staggerDelay = 0.05f; // Delay between each cell's animation start

    private List<int[,]> generationStates = new List<int[,]>(); // historical record of generations, used for animating

    public string GeneratePattern(string patternName)
    {
        // Clear previous cells and states
        // TODO: use object pooling if moving to mobile
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        generationStates.Clear();

        // Find the pattern, use default if not found
        LifePattern pattern = patterns.Find(p => p.patternName.ToUpper() == patternName.ToUpper());
        if (pattern == null)
        {
            Debug.LogWarning("Invalid pattern supplied, generating default pattern: " + defaultPattern.patternName);
            pattern = defaultPattern;
        }

        Debug.Log($"Generating pattern: {pattern.patternName}");
        return RenderPatternText(pattern);
    }

    // Initialize the grid with a 'LifePattern' (random, blinker, beacon, etc.)
    // Generate each generation, and capture it in 'generationStates'
    // Return grid as a string, then kick off the animation sequence
    public string RenderPatternText(LifePattern parsedPattern)
    {
        gridManager.InitializeGrid(parsedPattern.GetPatternGrid2D(), parsedPattern.startingPosition);

        string allGenerations = gridManager.GetGridState() + "\n";
        generationStates.Add((int[,])gridManager.GetGrid2DArray().Clone());

        for (int i = 1; i < generations; i++)
        {
            gridManager.NextGeneration();
            allGenerations += gridManager.GetGridState() + "\n";
            generationStates.Add((int[,])gridManager.GetGrid2DArray().Clone());
        }

        allGenerations += "<EOF>";
        StartCoroutine(AnimateGenerations3D(parsedPattern));

        return allGenerations;
    }

    private IEnumerator AnimateGenerations3D(LifePattern parsedPattern)
    {
        for (int gen = 0; gen < generationStates.Count; gen++)
        {
            Debug.Log($"Rendering generation {gen}");
            yield return StartCoroutine(FadeOutCells());
            yield return StartCoroutine(FadeInCells(parsedPattern, generationStates[gen]));
            yield return new WaitForSeconds(animationTime);
        }
    }

    private IEnumerator FadeInCells(LifePattern parsedPattern, int[,] grid)
    {
        Debug.Log("Fading in cells...");
        List<IEnumerator> coroutines = new List<IEnumerator>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                // Adjust the y coordinate to match the appearance of the debug output
                int adjustedY = grid.GetLength(1) - 1 - y;

                GameObject cell = Instantiate(grid[x, adjustedY] == 1 ? parsedPattern.alivePrefab : parsedPattern.deadPrefab,
                                              new Vector3(x, 0, y), Quaternion.identity, transform);
                cell.transform.localScale = Vector3.zero;
                coroutines.Add(ScaleCell(cell, Vector3.zero, Vector3.one, scaleTime, staggerDelay * (x * grid.GetLength(1) + y)));
            }
        }

        foreach (var coroutine in coroutines)
        {
            StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(scaleTime + staggerDelay * (grid.GetLength(0) * grid.GetLength(1)));
    }

    private IEnumerator FadeOutCells()
    {
        Debug.Log("Fading out cells...");
        List<IEnumerator> coroutines = new List<IEnumerator>();

        foreach (Transform child in transform)
        {
            coroutines.Add(ScaleCell(child.gameObject, Vector3.one, Vector3.zero, scaleTime, staggerDelay * (child.GetSiblingIndex())));
        }

        foreach (var coroutine in coroutines)
        {
            StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(scaleTime + staggerDelay * transform.childCount);

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator ScaleCell(GameObject cell, Vector3 fromScale, Vector3 toScale, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (cell != null)
            {
                float t = elapsedTime / duration;
                float scale = fadeCurve.Evaluate(t);
                cell.transform.localScale = Vector3.Lerp(fromScale, toScale, scale);
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        if (cell != null)
        {
            cell.transform.localScale = toScale;
        }
    }
}
