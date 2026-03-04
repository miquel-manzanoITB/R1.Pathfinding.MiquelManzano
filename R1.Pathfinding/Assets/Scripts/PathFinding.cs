using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PathFinding script using A* algorithm.
/// Uses coroutines to visualize:
///   - Closed list nodes (visited) with one token color
///   - Final path nodes with a different token color
/// </summary>
public class PathFinding : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  Inspector references                                                //
    // ------------------------------------------------------------------ //
    [Header("Token Prefabs")]
    [Tooltip("Token used to mark nodes added to the Closed List")]
    public GameObject closedListToken;

    [Tooltip("Token used to mark nodes that belong to the final path")]
    public GameObject finalPathToken;

    [Header("Timing")]
    [Tooltip("Seconds between each step of the algorithm visualisation")]
    public float stepDelay = 0.05f;

    // ------------------------------------------------------------------ //
    //  Private state                                                       //
    // ------------------------------------------------------------------ //
    private List<Way> _openList;    // Llista Oberta
    private List<Node> _closedList;  // Llista Tancada

    // ------------------------------------------------------------------ //
    //  Public entry point – call this from GameManager                    //
    // ------------------------------------------------------------------ //
    /// <summary>
    /// Starts the A* coroutine from <paramref name="startNode"/> to
    /// <paramref name="endNode"/>.
    /// </summary>
    public void FindPath(Node startNode, Node endNode)
    {
        StartCoroutine(AStarCoroutine(startNode, endNode));
    }

    // ================================================================== //
    //  A* Coroutine                                                        //
    // ================================================================== //
    private IEnumerator AStarCoroutine(Node startNode, Node endNode)
    {
        // ---- Initialise lists ---------------------------------------- //
        _openList = new List<Way>();
        _closedList = new List<Node>();

        // Add the starting node to the open list with accumulated cost = 0
        Way startWay = new Way(startNode, 0f);
        startWay.ACUMulatedCost = 0f;
        _openList.Add(startWay);

        startNode.NodeParent = null;

        bool pathFound = false;

        // ---- Main loop ----------------------------------------------- //
        while (_openList.Count > 0)
        {
            // 1. Pick the Way with the lowest f = accumulatedCost + heuristic
            Way currentWay = GetBestWay();

            if (currentWay == null)
                break;

            Node currentNode = currentWay.NodeDestiny;

            // 2. Remove from Open List
            _openList.Remove(currentWay);

            // 3. Check if we reached the goal
            if (currentNode == endNode)
            {
                pathFound = true;
                break;
            }

            // 4. Add to Closed List and visualise
            _closedList.Add(currentNode);
            SpawnToken(closedListToken, currentNode.RealPosition);

            yield return new WaitForSeconds(stepDelay);

            // 5. Expand neighbours
            foreach (Way neighbourWay in currentNode.WayList)
            {
                Node neighbour = neighbourWay.NodeDestiny;

                // Skip if already in the Closed List
                if (_closedList.Contains(neighbour))
                    continue;

                float newCost = currentWay.ACUMulatedCost + neighbourWay.Cost;

                // Check if the neighbour is already in the Open List
                Way existingWay = GetWayInOpenList(neighbour);

                if (existingWay == null)
                {
                    // Not yet in Open List – add it
                    Way newWay = new Way(neighbour, neighbourWay.Cost);
                    newWay.ACUMulatedCost = newCost;
                    neighbour.NodeParent = currentNode;
                    _openList.Add(newWay);
                }
                else if (newCost < existingWay.ACUMulatedCost)
                {
                    // Found a cheaper path – update
                    existingWay.ACUMulatedCost = newCost;
                    neighbour.NodeParent = currentNode;
                }
            }
        }

        // ---- Reconstruct and visualise the final path ---------------- //
        if (pathFound)
        {
            List<Node> path = GetFinalPath(endNode);
            Debug.Log($"[PathFinding] Path found! {path.Count} nodes.");

            foreach (Node n in path)
            {
                SpawnToken(finalPathToken, n.RealPosition);
                yield return new WaitForSeconds(stepDelay);
            }
        }
        else
        {
            Debug.LogWarning("[PathFinding] No path found.");
        }
    }

    // ================================================================== //
    //  Llista Oberta helpers                                               //
    // ================================================================== //

    /// <summary>
    /// Returns the Way in the Open List with the lowest f-score
    /// (accumulated cost + heuristic of destination node).
    /// </summary>
    private Way GetBestWay()
    {
        Way best = null;
        float bestScore = float.MaxValue;

        foreach (Way w in _openList)
        {
            float f = w.ACUMulatedCost + w.NodeDestiny.Heuristic;
            if (f < bestScore)
            {
                bestScore = f;
                best = w;
            }
        }

        return best;
    }

    /// <summary>
    /// Returns the Way in the Open List that leads to <paramref name="node"/>,
    /// or <c>null</c> if it is not present.
    /// </summary>
    private Way GetWayInOpenList(Node node)
    {
        foreach (Way w in _openList)
        {
            if (w.NodeDestiny == node)
                return w;
        }
        return null;
    }

    // ================================================================== //
    //  Llista Tancada helpers                                              //
    // ================================================================== //

    /// <summary>
    /// Returns <c>true</c> if <paramref name="node"/> is already in the
    /// Closed List.
    /// </summary>
    private bool IsInClosedList(Node node)
    {
        return _closedList.Contains(node);
    }

    // ================================================================== //
    //  Camí final                                                          //
    // ================================================================== //

    /// <summary>
    /// Reconstructs the final path by following parent references from
    /// <paramref name="endNode"/> back to the start.
    /// Returns the path ordered from start to end.
    /// </summary>
    private List<Node> GetFinalPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.NodeParent;
        }

        path.Reverse(); // start → end
        return path;
    }

    // ================================================================== //
    //  Visualisation helper                                                //
    // ================================================================== //

    /// <summary>
    /// Instantiates a token prefab at the given world position.
    /// Returns the spawned GameObject so the caller can manage it if needed.
    /// </summary>
    private GameObject SpawnToken(GameObject prefab, Vector2 position)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[PathFinding] Token prefab is not assigned!");
            return null;
        }
        return Instantiate(prefab, position, Quaternion.identity);
    }
}