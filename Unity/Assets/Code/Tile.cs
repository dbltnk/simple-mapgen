using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public enum T { empty, start, apple, floor, wall };
    public T Type;
    Map map;

    void Awake()
    {
        map = GameObject.Find("MapRoot").GetComponent<Map>();
        //if (GetRightNeighbour() != null) {
        //    print(string.Concat(X, "-", Y, ":", GetRightNeighbour().X, "-", GetRightNeighbour().Y));
        //}
        //if (GetLeftNeighbour() != null) {
        //    print(string.Concat(X, "-", Y, ":", GetLeftNeighbour().X, "-", GetLeftNeighbour().Y));
        //}
        //if (GetTopNeighbour() != null) {
        //    print(string.Concat(X, "-", Y, ":", GetTopNeighbour().X, "-", GetTopNeighbour().Y));
        //}
        //if (GetBottomNeighbour() != null) {
        //    print(string.Concat(X, "-", Y, ":", GetBottomNeighbour().X, "-", GetBottomNeighbour().Y));
        //}

        //if (X == 7 && Y == 14) {
        //    List<Tile> neighbours = GetAllNeighbours();
        //    foreach (Tile n in neighbours) {
        //        print(string.Concat(X, "-", Y, ":", n.X, "-", n.Y));
        //    }
        //}

    }

    Tile GetRightNeighbour()
    {
        if (X < map.Width - 1) {
            return map.GetTileByCoordinates(X + 1, Y);
        }

        Debug.LogWarning(string.Concat("Right neighbour for tile-", X, "-", Y, " does not exist."));
        return null;
    }

    Tile GetLeftNeighbour () {
        if (X > 0) {
            return map.GetTileByCoordinates(X - 1, Y);
        }

        Debug.LogWarning(string.Concat("Left neighbour for tile-", X, "-", Y, " does not exist."));
        return null;
    }

    Tile GetTopNeighbour () {
        if (Y < map.Height - 1) {
            return map.GetTileByCoordinates(X, Y + 1);
        }

        Debug.LogWarning(string.Concat("Top neighbour for tile-", X, "-", Y, " does not exist."));
        return null;
    }

    Tile GetBottomNeighbour () {
        if (Y > 0) {
            return map.GetTileByCoordinates(X, Y - 1);
        }

        Debug.LogWarning(string.Concat("Bottom neighbour for tile-", X, "-", Y, " does not exist."));
        return null;
    }

    public List<Tile> GetAllNeighbours () {
        List<Tile> neighbours = new List<Tile>();

        if (GetRightNeighbour() != null) {
            neighbours.Add(GetRightNeighbour());
        }
        if (GetLeftNeighbour() != null) {
            neighbours.Add(GetLeftNeighbour());
        }
        if (GetTopNeighbour() != null) {
            neighbours.Add(GetTopNeighbour());
        }
        if (GetBottomNeighbour() != null) {
            neighbours.Add(GetBottomNeighbour());
        }

        if (neighbours.Count == 0) {
            Debug.LogWarning(string.Concat("No neighbours exit for tile", X, "-", Y));
        }

        return neighbours;
    }
}
