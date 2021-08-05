using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Objects From Scene"), SerializeField]
    private SizeController widthController;
    [SerializeField]
    private SizeController heightController;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private RectTransform workZone;

    [Header("Prefabs"), SerializeField]
    private CellController cellPrefab;

    private GameObject existingMaze;
    private Vector2 playerStartSize;
    private Vector2 playerStartPosition;

    private void Start()
    {
        playerStartSize = player.transform.localScale;

        if (!RectTransformUtility.RectangleContainsScreenPoint(workZone, player.transform.position))
        {
            while (!RectTransformUtility.RectangleContainsScreenPoint(workZone, player.transform.position))
            {
                player.transform.position = new Vector2(player.transform.position.x + Time.deltaTime, player.transform.position.y);
            }
            player.transform.position = new Vector2(player.transform.position.x + player.transform.localScale.x, player.transform.position.y);
        }

        playerStartPosition = player.transform.position;
    }

    public void OnGenerateNewMazeClicked()
    {
        GenerateNewMaze();
    }

    public void GenerateNewMaze()
    {
        //Destroy previous maze if it was
        if (existingMaze != null)
        {
            Destroy(existingMaze);
        }

        //Create maze without way
        Cell[,] newMaze = new Cell[widthController.GetValue(), heightController.GetValue()];
        for (int x = 0; x < newMaze.GetLength(0); x++)
        {
            for (int y = 0; y < newMaze.GetLength(1); y++)
            {
                newMaze[x, y] = new Cell()
                {
                    X = x,
                    Y = y,
                    LeftWall = true,
                    BottomWall = true
                };
            }
        }

        //Destroy useless walls on sides
        for (int x = 0; x < newMaze.GetLength(0); x++)
        {
            newMaze[x, heightController.GetValue() - 1].LeftWall = false;
        }
        for (int y = 0; y < newMaze.GetLength(1); y++)
        {
            newMaze[widthController.GetValue() - 1, y].BottomWall = false;
        }

        BuildMazeWay(newMaze);
        CreateMazeFinish(newMaze);

        //Create visual of maze
        List<CellController> cells = new List<CellController>();
        for (int x = 0; x < newMaze.GetLength(0); x++)
        {
            for (int y = 0; y < newMaze.GetLength(1); y++)
            {
                var newCell = Instantiate(cellPrefab, new Vector2(x, y), Quaternion.identity);
                newCell.leftWall.SetActive(newMaze[x, y].LeftWall);
                newCell.bottomWall.SetActive(newMaze[x, y].BottomWall);
                newCell.finish.SetActive(newMaze[x, y].Finish);
                cells.Add(newCell);
            }
        }

        //Create maze parent object to easy scale and move
        GameObject mazeObject = new GameObject();
        mazeObject.name = "Maze";
        foreach (var cell in cells)
        {
            cell.transform.SetParent(mazeObject.transform);
        }

        //Set start position
        existingMaze = mazeObject;
        existingMaze.transform.position = new Vector2(playerStartPosition.x - 0.5f, playerStartPosition.y - 0.5f);

        //Set player start size and position
        player.transform.localScale = playerStartSize;
        player.transform.position = playerStartPosition;

        //If the maze does not fit completely into the screen, adjust it to fit the screen
        if (!cells.TrueForAll(x => x.IsVisibleOnScreen()))
        {
            FitToScreen(cells);
        }
    }

    private void CreateMazeFinish(Cell[,] maze)
    {
        Cell furthest = maze[0, 0];

        //Determine if there is a finish (the farthest cell) on the X axis
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            if (maze[x, maze.GetLength(1) - 2].DistanceFromStart > furthest.DistanceFromStart)
            {
                furthest = maze[x, maze.GetLength(1) - 2];
            }

            if (maze[x, 0].DistanceFromStart > furthest.DistanceFromStart)
            {
                furthest = maze[x, 0];
            }
        }

        //Determine if there is a finish (the farthest cell) on the Y axis
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            if (maze[maze.GetLength(0) - 2, y].DistanceFromStart > furthest.DistanceFromStart)
            {
                furthest = maze[maze.GetLength(0) - 2, y];
            }

            if (maze[0, y].DistanceFromStart > furthest.DistanceFromStart)
            {
                furthest = maze[0, y];
            }
        }

        //Set FINISH ​​at the farthest cell to the TRUE and break the corresponding wall to exit
        furthest.Finish = true;
        if (furthest.X == 0)
        {
            furthest.LeftWall = false;
        }
        else if (furthest.Y == 0)
        {
            furthest.BottomWall = false;
        }
        else if (furthest.X == maze.GetLength(0) - 2)
        {
            maze[furthest.X + 1, furthest.Y].LeftWall = false;
        }
        else if (furthest.Y == maze.GetLength(1) - 2)
        {
            maze[furthest.X, furthest.Y + 1].BottomWall = false;
        }
    }

    private void BuildMazeWay(Cell[,] maze)
    {
        //Set start cell
        Cell current = maze[0, 0];
        current.Visited = true;
        current.DistanceFromStart = 0;

        Stack<Cell> visitedCells = new Stack<Cell>();
        do
        {
            List<Cell> unvisitedCells = new List<Cell>();
            int x = current.X;
            int y = current.Y;

            //Find all unistied neighbour cells
            if (x > 0 && !maze[x - 1, y].Visited)
            {
                unvisitedCells.Add(maze[x - 1, y]);
            }
            if (y > 0 && !maze[x, y - 1].Visited)
            {
                unvisitedCells.Add(maze[x, y - 1]);
            }
            if (x < maze.GetLength(0) - 2 && !maze[x + 1, y].Visited)
            {
                unvisitedCells.Add(maze[x + 1, y]);
            }
            if (y < maze.GetLength(1) - 2 && !maze[x, y + 1].Visited)
            {
                unvisitedCells.Add(maze[x, y + 1]);
            }

            if (unvisitedCells.Count > 0)
            {
                //Choose random unvisited neighbour to move into it and continue way
                Cell randomCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];

                //To build next way part
                DestroyWall(current, randomCell);

                randomCell.Visited = true;
                current = randomCell;
                visitedCells.Push(randomCell);
                randomCell.DistanceFromStart = visitedCells.Count;
            }
            else
            {
                if (visitedCells.Count > 0)
                {
                    //Move back
                    current = visitedCells.Pop();
                }
            }

        } while (visitedCells.Count > 0);
    }

    private void DestroyWall(Cell current, Cell next)
    {
        if (current.X == next.X)
        {
            //If next cell lower than current
            if (current.Y > next.Y)
            {
                //Destroy current bottom wall
                current.BottomWall = false;
            }
            else
            {
                //Destroy next bottom wall
                next.BottomWall = false;
            }
        }
        else
        {
            //If next cell righter than current
            if (current.X > next.X)
            {
                //Destroy current left wall
                current.LeftWall = false;
            }
            else
            {
                //Destroy next left wall
                next.LeftWall = false;
            }
        }
    }

    private void FitToScreen(List<CellController> cells)
    {
        while (!cells.TrueForAll(x => x.IsVisibleOnScreen()))
        {
            //Shrinking the maze a little
            existingMaze.transform.localScale = new Vector2(existingMaze.transform.localScale.x - Time.deltaTime, existingMaze.transform.localScale.y - Time.deltaTime);
            //Move the player a little so that after fitting to the screen, the player is at the start of the maze
            player.transform.position = new Vector2(player.transform.position.x - Time.deltaTime / 2, player.transform.position.y - Time.deltaTime / 2);
        }
        //Shrink the player to fit the maze size
        player.transform.localScale = new Vector2(existingMaze.transform.localScale.x / 2, existingMaze.transform.localScale.y / 2);
    }
}