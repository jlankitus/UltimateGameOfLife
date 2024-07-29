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

    [SerializeField]
    private float scaleTime = 0.5f; // Time for scaling transition

    [SerializeField]
    private AnimationCurve fadeCurve; // Animation curve for fade

    [SerializeField]
    private float staggerDelay = 0.05f; // Delay between each cell's animation start

    public string GeneratePattern(string patternName)
    {
        // Clear previous cells
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Find the pattern
        LifePattern pattern = patterns.Find(p => p.patternName.ToUpper() == patternName.ToUpper());
        if (pattern == null)
        {
            Debug.LogWarning("Invalid pattern supplied, generating default pattern: " + defaultPattern.patternName);
            pattern = defaultPattern;
        }

        Debug.Log($"Generating pattern: {pattern.patternName}");
        return RenderPatternText(pattern);
    }

    public string RenderPatternText(LifePattern parsedPattern)
    {
        gridManager.InitializeGrid(parsedPattern.GetPatternGrid2D(), parsedPattern.startingPosition);

        string allGenerations = gridManager.GetGridState() + "\n";
        for (int i = 1; i < generations; i++)
        {
            gridManager.NextGeneration();
            allGenerations += gridManager.GetGridState() + "\n";
        }

        allGenerations += "<EOF>";
        StartCoroutine(AnimateGenerations3D(parsedPattern));

        return allGenerations;
    }

    private IEnumerator AnimateGenerations3D(LifePattern parsedPattern)
    {
        for (int gen = 0; gen < generations; gen++)
        {
            Debug.Log($"Rendering generation {gen}");
            yield return StartCoroutine(FadeOutCells());
            gridManager.NextGeneration();
            yield return StartCoroutine(FadeInCells(parsedPattern));
            yield return new WaitForSeconds(animationTime);
        }
    }

    private IEnumerator FadeInCells(LifePattern parsedPattern)
    {
        Debug.Log("Fading in cells...");
        int[,] grid = gridManager.GetGrid2DArray();
        List<IEnumerator> coroutines = new List<IEnumerator>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject cell = Instantiate(grid[x, y] == 1 ? parsedPattern.alivePrefab : parsedPattern.deadPrefab,
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
