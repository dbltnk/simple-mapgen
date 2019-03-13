using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    List<GameObject> tiles;
    public GameObject PrefabTile;
    public Sprite SpriteEmpty;
    public Sprite SpriteStart;
    public Sprite SpriteApple;
    public Sprite SpriteWall;
    public Sprite SpriteFloor;
    public int Width;
    public int Height;
    public float GridX;
    public float GridY;
    public int AmountStarts;
    public int AmountApples;
    bool dirty = false;
    public float ChanceOfFatRooms;

    void Start()
    {
        tiles = new List<GameObject>();

        // set up empty map
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                GameObject go = Instantiate(PrefabTile);
                go.transform.SetParent(transform);
                tiles.Add(go);
                Tile t = go.GetComponent<Tile>();
                t.X = x;
                t.Y = y;
                t.Type = Tile.T.empty;
                go.name = string.Concat("tile", "-", t.Type, "-", x, "-", y);
                go.transform.position = new Vector3(t.X * GridX, t.Y * GridY, 0f);
            }
        }

        List<GameObject> tilesToFill = new List<GameObject>(tiles);

        // add starting points
        int countStart = 0;
        while (countStart < AmountStarts) {

            int r = Random.Range(0, tilesToFill.Count);
            int x = tilesToFill[r].GetComponent<Tile>().X;
            int y = tilesToFill[r].GetComponent<Tile>().Y;

            if (x != 0 && x < Width - 1 && y != 0 && y < Height - 1) {
                tilesToFill.RemoveAt(r);
                GetTileByCoordinates(x, y).Type = Tile.T.start;
                countStart++;
            }
        }

        // add apples
        int countApples = 0;
        while (countApples < AmountApples) {

            int r = Random.Range(0, tilesToFill.Count);
            int x = tilesToFill[r].GetComponent<Tile>().X;
            int y = tilesToFill[r].GetComponent<Tile>().Y;

            if (x != 0 && x < Width - 1 && y != 0 && y < Height - 1) {
                tilesToFill.RemoveAt(r);
                GetTileByCoordinates(x, y).Type = Tile.T.apple;
                countApples++;
            }
        }

        // find the shortest path between every apple and every start

        List<Tile> tilesToGround = new List<Tile>();
        List<Tile> tilesToConnect = new List<Tile>();
        for (int i = 0; i < tiles.Count; i++) {
            Tile t = tiles[i].GetComponent<Tile>();
            if (t.Type == Tile.T.start || t.Type == Tile.T.apple) {
                //print(string.Concat("Tile-", t.X, "-", t.Y));
                tilesToConnect.Add(t);
            }
        }

        List<Tile> tilesToExpand = new List<Tile>();
        tilesToExpand.AddRange(tilesToConnect);

        Tile tileCenter = tilesToConnect[0];
        tilesToConnect.RemoveAt(0);

        Tile tileCurrent = null;
        while (tilesToConnect.Count > 0) {
            tileCurrent = tilesToConnect[0];
            tilesToConnect.RemoveAt(0);
            int deltaX = tileCurrent.X - tileCenter.X;
            int deltaY = tileCurrent.Y - tileCenter.Y;

            //print(string.Concat("Tile-center-", tileCenter.X, "-", tileCenter.Y));
            //print(string.Concat("Tile-current-", tileCurrent.X, "-", tileCurrent.Y));
            //print(deltaX);
            //print(deltaY);

            Tile tileLastGrounded = null;
            // go right/left and add all
            if (deltaX != 0) {
                int direction = deltaX > 0 ? 1 : -1;
                for (int i = 0; i <= Mathf.Abs(deltaX); i++) {
                    // do not add start and end tiles
                    Tile tileToGround = GetTileByCoordinates(tileCenter.X + i * direction, tileCenter.Y);
                    if (!AreTilesEqual(tileToGround, tileCenter) && tileToGround.Type != Tile.T.start && tileToGround.Type != Tile.T.apple) {
                        // NTH TODO: ONLY ADD IF IS NOT ALREADY IN THERE
                        tilesToGround.Add(tileToGround);
                        tileLastGrounded = tileToGround;
                    }
                }
            }

            // go up/down and add all
            if (deltaY != 0) {
                int direction = deltaY > 0 ? 1 : -1;
                for (int i = 0; i <= Mathf.Abs(deltaY); i++) {
                    // do not add start and end tiles

                    Tile tileToGround = null;
                    if (tileLastGrounded != null) {
                        tileToGround = GetTileByCoordinates(tileLastGrounded.X, tileLastGrounded.Y + i * direction);
                    }
                    else {
                        tileToGround = GetTileByCoordinates(tileCenter.X, tileCenter.Y + i * direction);
                    }

                    if (!AreTilesEqual(tileToGround, tileCenter) && tileToGround.Type != Tile.T.start && tileToGround.Type != Tile.T.apple) {
                        // NTH TODO: ONLY ADD IF IS NOT ALREADY IN THERE
                        tilesToGround.Add(tileToGround);
                    }
                }
            }

        }

        // mark those fields as floor
        foreach (Tile t in tilesToGround) {
            t.Type = Tile.T.floor;
        }

        // make some rooms a bit bigger
        foreach (Tile t in tilesToExpand) {
            float r = Random.Range(0f, 1f);
            bool expand = (r <= ChanceOfFatRooms) ? true : false;
            if (expand) {
                foreach (Tile ti in t.Expand()) {
                    if (ti.Type == Tile.T.empty && ti.IsOnTheEdge() == false) ti.Type = Tile.T.floor;
                }
            }
        }

        // mark everything else as walls
        foreach (GameObject go in tilesToFill) {
            Tile t = go.GetComponent<Tile>();
            if (t.Type == Tile.T.empty) t.Type = Tile.T.wall;
        }

        // TODO: remove all unnecessary walls
        List<Tile> tilesToBeRemoved = new List<Tile>();
        foreach (GameObject go in tilesToFill) {
            Tile t = go.GetComponent<Tile>();
            if (t.Type == Tile.T.wall) {
                List<Tile> tilesToCheck = new List<Tile>();
                tilesToCheck.AddRange(t.GetAllNeighbours());
                tilesToCheck.AddRange(t.GetAllCorners());
                bool hasNonWallNeighbours = false;
                foreach (Tile n in tilesToCheck) {
                    if (n.Type != Tile.T.wall) hasNonWallNeighbours = true;
                }
                if (hasNonWallNeighbours == false) tilesToBeRemoved.Add(t);
            }
        }
        foreach (Tile t in tilesToBeRemoved) {
            t.Type = Tile.T.empty;
        }

        dirty = true;
    }

    public Tile GetTileByCoordinates(int x, int y)
    {
        Tile tR = null;
        foreach (GameObject go in tiles) {
            Tile t = go.GetComponent<Tile>();
            if (t.X == x && t.Y == y) {
                tR = t;
            }
        }
        return tR;
    }

    public bool AreTilesEqual (Tile a, Tile b) {
        return (a.X == b.X && a.Y == b.Y) ? true : false;
    }

    private void Update () {
        if (dirty) PaintAndName();
    }

    private void PaintAndName () {
        foreach (GameObject tile in tiles) {
            Tile t = tile.GetComponent<Tile>();
            tile.name = string.Concat("tile", "-", t.Type, "-", t.X, "-", t.Y);

            SpriteRenderer r = tile.GetComponent<SpriteRenderer>();
            if (t.Type == Tile.T.empty) {
                r.sprite = SpriteEmpty;
            }
            else if (t.Type == Tile.T.start) {
                r.sprite = SpriteStart;
            } 
            else if (t.Type == Tile.T.apple) {
                r.sprite = SpriteApple;
            } 
            else if (t.Type == Tile.T.wall) {
                r.sprite = SpriteWall;
            } 
            else if (t.Type == Tile.T.floor) {
                r.sprite = SpriteFloor;
            } 
            else {
                Debug.LogError(string.Concat("Tile-", t.X, "-", t.Y, " has no type."));
            }

            dirty = false;
        }
    }
}
