using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width = 8;
    [SerializeField]
    private int height = 8;

    private int[,] grid;

    private void Awake()
    {
        grid = new int[width, height];
    }

    public void InitializeGrid(int[,] initialGrid, Vector2Int startPosition)
    {
        // Clear the grid first
        Array.Clear(grid, 0, grid.Length);

        int initialRows = initialGrid.GetLength(0);
        int initialCols = initialGrid.GetLength(1);

        // Place the initial pattern in the grid based on the starting position
        for (int x = 0; x < initialRows; x++)
        {
            for (int y = 0; y < initialCols; y++)
            {
                int targetX = (startPosition.x + x) % width;
                int targetY = (startPosition.y + y) % height;
                grid[targetX, targetY] = initialGrid[x, y];
            }
        }
    }

    public void PrintGrid()
    {
        for (int y = 0; y < height; y++)
        {
            string line = "";
            for (int x = 0; x < width; x++)
            {
                line += grid[x, y] == 1 ? "X " : ". ";
            }
            Debug.Log(line);
        }
        Debug.Log("        ");
    }

    public void NextGeneration()
    {
        int[,] newGrid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int liveNeighbors = CountLiveNeighbors(x, y);

                if (grid[x, y] == 1)
                {
                    newGrid[x, y] = (liveNeighbors == 2 || liveNeighbors == 3) ? 1 : 0;
                }
                else
                {
                    newGrid[x, y] = (liveNeighbors == 3) ? 1 : 0;
                }
            }
        }

        Array.Copy(newGrid, grid, newGrid.Length);
    }

    private int CountLiveNeighbors(int x, int y)
    {
        int liveNeighbors = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int nx = (x + dx + width) % width;
                int ny = (y + dy + height) % height;

                liveNeighbors += grid[nx, ny];
            }
        }

        return liveNeighbors;
    }

    public string GetGridState()
    {
        string gridState = "";
        for (int y = 0; y < height; y++)
        {
            string line = "";
            for (int x = 0; x < width; x++)
            {
                line += grid[x, y] == 1 ? "X " : ". ";
            }
            gridState += line.Trim() + "\n";
        }
        gridState += "<EOF>"; // Add a delimiter to indicate the end of the response
        return gridState;
    }
}
