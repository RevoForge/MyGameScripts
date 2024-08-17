using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited;
        public bool[] walls = new bool[4]; // top, right, bottom, left
    }
    public class Wall
    {
        public int x;
        public int y;
        public int direction;

        public Wall(int x, int y, int direction)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
        }
    }
    public Cell[,] grid;
    public GameObject wallPrefab;

    private void Start()
    {
        StartCoroutine(GenerateMaze());
    }

    private IEnumerator GenerateMaze()
    {
        grid = new Cell[20, 20];
        HashSet<Wall> walls = new();
        bool[,] visited = new bool[20, 20];
        int startX = Random.Range(0, 20);
        int startY = Random.Range(0, 20);
        grid[startX, startY].visited = true;
        AddWallsToList(startX, startY, walls, visited);

        while (walls.Count > 0)
        {
            var wall = walls.FirstOrDefault();
            walls.Remove(wall);
            int x = wall.x;
            int y = wall.y;
            int direction = wall.direction;

            int nx = x, ny = y;
            switch (direction)
            {
                case 0: ny--; break;
                case 1: nx++; break;
                case 2: ny++; break;
                case 3: nx--; break;
            }

            if (nx >= 0 && nx < 20 && ny >= 0 && ny < 20 && !visited[nx, ny])
            {
                grid[x, y].walls[direction] = false;
                grid[nx, ny].walls[(direction + 2) % 4] = false;
                visited[nx, ny] = true;
                AddWallsToList(nx, ny, walls, visited);
            }
        }

        List<(Vector3 position, Quaternion rotation)> wallsToInstantiate = new List<(Vector3, Quaternion)>();

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                Vector3 cellPosition = new Vector3(x, 0.5f, y);
                for (int direction = 0; direction < 4; direction++)
                {
                    if (grid[x, y].walls[direction])
                    {
                        Quaternion rotation = Quaternion.Euler(0, direction * 90, 0);
                        wallsToInstantiate.Add((cellPosition, rotation));
                    }
                }
            }
        }

        foreach (var (position, rotation) in wallsToInstantiate)
        {
            Instantiate(wallPrefab, position, rotation);
        }

        yield return null;
    }

    private void AddWallsToList(int x, int y, HashSet<Wall> walls, bool[,] visited)
    {
        int[,] directions = { { 0, -1, 0 }, { 1, 0, 1 }, { 0, 1, 2 }, { -1, 0, 3 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];
            int dir = directions[i, 2];

            if (x + dx >= 0 && x + dx < 20 && y + dy >= 0 && y + dy < 20 && !visited[x + dx, y + dy])
            {
                walls.Add(new Wall(x, y, dir));
            }
        }
    }
}