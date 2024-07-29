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

    private Dictionary<Vector2Int, GameObject> cellMap = new Dictionary<Vector2Int, GameObject>();

    public string GeneratePattern(string patternName)
    {
        // Clear previous cells and reset state
        foreach (var cell in cellMap.Values)
        {
            Destroy(cell);
        }
        cellMap.Clear();

        // Ignore case, and launch pattern, if a correct pattern name is supplied
        LifePattern pattern = patterns.Find(p => p.patternName.ToUpper() == patternName.ToUpper());
        if (pattern != null)
        {
            Debug.Log($"Generating pattern: {pattern.patternName}");
            return RenderPatternText(pattern);
        }
        // If pattern name was not supplied, or incorrectly supplied, default to RANDOM
        else
        {
            Debug.LogWarning("Invalid pattern supplied, generating default pattern: " + defaultPattern.patternName);
            return RenderPatternText(defaultPattern);
        }
    }

    public string RenderPatternText(LifePattern parsedPattern)
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

        // Start the animation coroutine
        StartCoroutine(AnimateGenerations3D(parsedPattern));

        return allGenerations;
    }

    public string GetGridState()
    {
        return gridManager.GetGridState();
    }

    private IEnumerator AnimateGenerations3D(LifePattern parsedPattern)
    {
        // Fade in all cells for the first generation
        yield return StartCoroutine(FadeInCells(parsedPattern));

        for (int gen = 1; gen <= generations; gen++)
        {
            yield return StartCoroutine(RenderGrid3D(parsedPattern));
            yield return new WaitForSeconds(animationTime);
            gridManager.NextGeneration();
        }
        
        // yield return StartCoroutine(RenderGrid3D(parsedPattern)); // Ensure final generation is rendered
    }

    private IEnumerator FadeInCells(LifePattern parsedPattern)
    {
        Debug.LogError("fading in cells...");
        int[,] initialGrid = gridManager.GetGrid2DArray();

        for (int x = 0; x < initialGrid.GetLength(0); x++)
        {
            for (int y = 0; y < initialGrid.GetLength(1); y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                GameObject cell;
                if (initialGrid[x, y] == 1)
                {
                    cell = Instantiate(parsedPattern.alivePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    cell.transform.localScale = Vector3.zero;
                    StartCoroutine(ScaleCell(cell, Vector3.zero, Vector3.one, parsedPattern.alivePrefab, position));
                    cell.tag = "ALIVE";
                }
                else
                {
                    cell = Instantiate(parsedPattern.deadPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    cell.transform.localScale = Vector3.zero;
                    StartCoroutine(ScaleCell(cell, Vector3.zero, Vector3.one, parsedPattern.deadPrefab, position));
                    cell.tag = "DEAD";
                }
                cellMap[position] = cell;
            }
        }

        yield return new WaitForSeconds(animationTime);
    }

    private IEnumerator RenderGrid3D(LifePattern parsedPattern)
    {
        int[,] finalGrid = gridManager.GetGrid2DArray();
        Dictionary<Vector2Int, GameObject> newCellMap = new Dictionary<Vector2Int, GameObject>();

        for (int x = 0; x < finalGrid.GetLength(0); x++)
        {
            for (int y = 0; y < finalGrid.GetLength(1); y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                GameObject cell;
                bool isAlive = finalGrid[x, y] == 1;

                if (cellMap.ContainsKey(position))
                {
                    cell = cellMap[position];
                    bool wasAlive = cell.tag == "ALIVE";
                    if (wasAlive != isAlive)
                    {
                        if (wasAlive)
                        {
                            // Alive to Dead transition
                            StartCoroutine(ScaleCell(cell, Vector3.one, Vector3.zero, parsedPattern.deadPrefab, position));
                            cell.tag = "DEAD";
                        }
                        else
                        {
                            // Dead to Alive transition
                            StartCoroutine(ScaleCell(cell, Vector3.zero, Vector3.one, parsedPattern.alivePrefab, position));
                            cell.tag = "ALIVE";
                        }
                    }
                    newCellMap[position] = cell;
                }
                else
                {
                    Debug.LogError("not in map!");
                    if (isAlive)
                    {
                        cell = Instantiate(parsedPattern.alivePrefab, new Vector3(x, 0, y), Quaternion.identity);
                        cell.transform.localScale = Vector3.zero;
                        StartCoroutine(ScaleCell(cell, Vector3.zero, Vector3.one, parsedPattern.alivePrefab, position));
                        cell.tag = "ALIVE";
                    }
                    else
                    {
                        cell = Instantiate(parsedPattern.deadPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        cell.transform.localScale = Vector3.zero;
                        StartCoroutine(ScaleCell(cell, Vector3.zero, Vector3.one, parsedPattern.deadPrefab, position));
                        cell.tag = "DEAD";
                    }
                    newCellMap[position] = cell;
                }
            }
        }

        cellMap = newCellMap;
        yield return null;
    }

    private IEnumerator ScaleCell(GameObject cell, Vector3 fromScale, Vector3 toScale, GameObject nextPrefab, Vector2Int position)
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            if (cell != null)
            {
                cell.transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / animationTime);
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        if (cell != null)
        {
            cell.transform.localScale = toScale;
        }

        if (toScale == Vector3.zero && cell != null)
        {
            cell.transform.localScale = Vector3.one;
            cell.tag = nextPrefab.tag;
        }
    }
}
