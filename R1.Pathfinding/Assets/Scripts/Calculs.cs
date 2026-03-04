using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculs
{
    public static float LinearDistance;   // cost of moving straight (1 cell)
    public static float DiagonalDistance; // cost of moving diagonally (√2 cells)
    public static Vector2 FirstPosition;

    public static void CalculateDistances(BoxCollider2D coll, float Size)
    {
        // The space each cell occupies in world units
        LinearDistance = coll.size.x / Size;

        // Diagonal is longer: Pythagoras → √(L² + L²) = L × √2 ≈ L × 1.4142
        // This reflects the real-world ratio you described:
        //   straight ≈ 1 cm  →  diagonal ≈ 1.414 cm  (you said ~1.5 cm, same idea)
        DiagonalDistance = LinearDistance * Mathf.Sqrt(2f);

        FirstPosition = new Vector2(
            -Size / 4f + LinearDistance / 2f - 0.1f,
             Size / 4f - LinearDistance / 2f + 0.1f);
    }

    public static Vector2 CalculatePoint(int x, int y)
    {
        return FirstPosition + new Vector2(x * LinearDistance, -y * LinearDistance);
    }

    public static float CalculateHeuristic(Node node, int finalx, int finaly)
    {
        // Euclidean distance – consistent with the actual movement costs above
        return Vector2.Distance(
            CalculatePoint(node.PositionX, node.PositionY),
            CalculatePoint(finalx, finaly));
    }
}