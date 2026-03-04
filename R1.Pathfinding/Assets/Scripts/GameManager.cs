using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager – creates the grid of nodes and kicks off the A* pathfinding.
/// Attach PathFinding component on this same GameObject (or assign it).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    public int Size;
    public BoxCollider2D Panel;

    [Header("Tokens")]
    [Tooltip("Generic node token (just to show the grid)")]
    public GameObject token;

    [Tooltip("Token placed on the start node")]
    public GameObject startToken;

    [Tooltip("Token placed on the goal/end node")]
    public GameObject endToken;

    [Header("References")]
    public PathFinding pathFinding; // drag the PathFinding component here

    private Node[,] NodeMatrix;
    private int startPosx, startPosy;
    private int endPosx, endPosy;

    void Awake()
    {
        Instance = this;
        Calculs.CalculateDistances(Panel, Size);
    }

    void Start()
    {
        // Random start / end (different row AND column)
        startPosx = Random.Range(0, Size);
        startPosy = Random.Range(0, Size);
        do
        {
            endPosx = Random.Range(0, Size);
            endPosy = Random.Range(0, Size);
        } while (endPosx == startPosx || endPosy == startPosy);

        NodeMatrix = new Node[Size, Size];
        CreateNodes();

        // Mark start and goal visually
        if (startToken != null)
            Instantiate(startToken, NodeMatrix[startPosx, startPosy].RealPosition, Quaternion.identity);
        else
            Debug.LogWarning("[GameManager] startToken not assigned!");

        if (endToken != null)
            Instantiate(endToken, NodeMatrix[endPosx, endPosy].RealPosition, Quaternion.identity);
        else
            Debug.LogWarning("[GameManager] endToken not assigned!");

        // Start A*
        if (pathFinding != null)
        {
            pathFinding.FindPath(NodeMatrix[startPosx, startPosy],
                                 NodeMatrix[endPosx, endPosy]);
        }
        else
        {
            Debug.LogError("[GameManager] PathFinding reference not set!");
        }
    }

    public void CreateNodes()
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                NodeMatrix[i, j] = new Node(i, j, Calculs.CalculatePoint(i, j));
                NodeMatrix[i, j].Heuristic =
                    Calculs.CalculateHeuristic(NodeMatrix[i, j], endPosx, endPosy);
            }

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                SetWays(NodeMatrix[i, j], i, j);

        DebugMatrix();
    }

    public void DebugMatrix()
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                Instantiate(token, NodeMatrix[i, j].RealPosition, Quaternion.identity);
                Debug.Log($"Element ({j}, {i})  Pos: {NodeMatrix[i, j].RealPosition}  H: {NodeMatrix[i, j].Heuristic}");
            }
    }

    public void SetWays(Node node, int x, int y)
    {
        node.WayList = new List<Way>();

        if (x > 0)
        {
            node.WayList.Add(new Way(NodeMatrix[x - 1, y], Calculs.LinearDistance));
            if (y > 0)
                node.WayList.Add(new Way(NodeMatrix[x - 1, y - 1], Calculs.DiagonalDistance));
        }
        if (x < Size - 1)
        {
            node.WayList.Add(new Way(NodeMatrix[x + 1, y], Calculs.LinearDistance));
            if (y > 0)
                node.WayList.Add(new Way(NodeMatrix[x + 1, y - 1], Calculs.DiagonalDistance));
        }
        if (y > 0)
            node.WayList.Add(new Way(NodeMatrix[x, y - 1], Calculs.LinearDistance));
        if (y < Size - 1)
        {
            node.WayList.Add(new Way(NodeMatrix[x, y + 1], Calculs.LinearDistance));
            if (x > 0)
                node.WayList.Add(new Way(NodeMatrix[x - 1, y + 1], Calculs.DiagonalDistance));
            if (x < Size - 1)
                node.WayList.Add(new Way(NodeMatrix[x + 1, y + 1], Calculs.DiagonalDistance));
        }
    }
}