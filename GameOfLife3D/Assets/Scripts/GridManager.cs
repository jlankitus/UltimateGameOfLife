using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int gridWidth = 8;
    [SerializeField]
    private int gridHeight = 8;

    // Our simulation of life, with characters represented as integers 0 (dead) or 1 (alive).
    private int[,] grid;

    private void Awake()
    {
        grid = new int[gridWidth, gridHeight];
        Debug.Log($"Grid initialized with dimensions {gridWidth}x{gridHeight}");
    }

    public void InitializeGrid(int[,] initialGrid, Vector2Int startPosition)
    {
        // Clear the grid first
        Array.Clear(grid, 0, grid.Length);

        int initialRows = initialGrid.GetLength(0);
        int initialCols = initialGrid.GetLength(1);

        // Place the initial pattern in the grid based on the starting position
        // We use modulo to ensure if part of the pattern is placed outside of the grid, it 'pac mans'
        // to the other side. (0,0) is top left, (8,8) is bottom right
        for (int x = 0; x < initialRows; x++)
        {
            for (int y = 0; y < initialCols; y++)
            {
                int targetX = (startPosition.x + x) % gridWidth;
                int targetY = (startPosition.y + y) % gridHeight;
                grid[targetX, targetY] = initialGrid[x, y];
            }
        }
        Debug.Log("Initialized grid:");
        PrintGrid();
    }

    // We print the grid in 'row major order' because humans like it that way :) 
    public void PrintGrid()
    {
        string gridLog = "";
        for (int y = 0; y < gridHeight; y++)
        {
            string line = "";
            for (int x = 0; x < gridWidth; x++)
            {
                line += grid[x, y] == 1 ? "X " : ". ";
            }
            gridLog += line + "\n";
        }
        Debug.Log(gridLog);
    }

    public void NextGeneration()
    {
        // Create a new grid to hold the next generation's states
        int[,] newGrid = new int[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Count the number of live neighbors around the current cell
                int liveNeighbors = CountLiveNeighbors(x, y);

                // If alive:
                if (grid[x, y] == 1)
                {
                    // Rule 1: Any live cell with fewer than two live neighbors dies (underpopulation)
                    // Rule 2: Any live cell with two or three live neighbors lives on to the next generation
                    // Rule 3: Any live cell with more than three live neighbors dies (overpopulation)
                    newGrid[x, y] = (liveNeighbors == 2 || liveNeighbors == 3) ? 1 : 0;
                }
                else
                {
                    // Rule 4: Any dead cell with exactly three live neighbors becomes a live cell (reproduction)
                    newGrid[x, y] = (liveNeighbors == 3) ? 1 : 0;
                }
            }
        }

        Array.Copy(newGrid, grid, newGrid.Length);
        Debug.Log("Generated next generation:");
        PrintGrid();
    }

    private int CountLiveNeighbors(int x, int y)
    {
        int liveNeighbors = 0;

        // Check all the neighbors around each (x,y) cell for signs of life
        // d = distance, if we cared about the whole street, we would increase this, for neighbors this just means 'next to' in both directions
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Don't check yourself for signs of life, it can cause an existential crisis
                if (dx == 0 && dy == 0)
                    continue;

                // (nx,ny) = neighbor's coordinate (we test each one in a loop)
                int nx = (x + dx + gridWidth) % gridWidth;
                int ny = (y + dy + gridHeight) % gridHeight;

                // If the neighbor lives, it adds to the count, if it's dead, we add it but it's 0
                liveNeighbors += grid[nx, ny];
            }
        }

        return liveNeighbors;
    }

    // Row major order printing in proper formatting: X or .
    public string GetGridState()
    {
        string gridState = "";
        for (int y = 0; y < gridHeight; y++)
        {
            string line = "";
            for (int x = 0; x < gridWidth; x++)
            {
                line += grid[x, y] == 1 ? "X " : ". ";
            }
            gridState += line.Trim() + "\n";
        }
        return gridState;
    }
    public int[,] GetGrid2DArray()
    {
        return grid;
    }
}
