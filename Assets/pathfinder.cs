using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

public class pathfinder : MonoBehaviour
{
    public struct Node
    {
        public int2 coord;
        public int2 parent;
        public int gScore;
        public int hScore;
    }

    public List<Node> openSet;
    Hashtable closedSet;
    public List<Node> cameFrom;

    Hashtable obstacles;
    Node start, end;
    int giveUp = 1000;

    public Tilemap map;
    public Tile defaultTile;
    public Camera cam;

    void Start()
    {
        obstacles = new Hashtable();
        start = new Node { coord = int2.zero, parent = int2.zero, gScore = int.MaxValue, hScore = int.MaxValue };
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            ClearTiles();
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            PlaceStart();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            PlaceEnd();
        }

        if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
        {
            PlaceObstacle();
        }

        if (Input.GetKey(KeyCode.B))
        {
            DrawSomething();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PathFind(start, end);
        }
    }

    void PlaceStart()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCell = map.WorldToCell(mouseWorldPos);
        int2 coord = new int2 { x = mouseCell.x, y = mouseCell.y };

        if (!obstacles.ContainsKey(coord) && !coord.Equals(end.coord))
        {
            map.SetTile(new Vector3Int(start.coord.x, start.coord.y, 0), null);

            start.coord = coord;
            map.SetTile(mouseCell, defaultTile);
            map.SetTileFlags(mouseCell, TileFlags.None);
            map.SetColor(mouseCell, Color.green);
        }
    }

    void PlaceEnd()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCell = map.WorldToCell(mouseWorldPos);
        int2 coord = new int2 { x = mouseCell.x, y = mouseCell.y };

        if (!obstacles.ContainsKey(coord) && !coord.Equals(start.coord))
        {
            map.SetTile(new Vector3Int(end.coord.x, end.coord.y, 0), null);

            end.coord = coord;
            map.SetTile(mouseCell, defaultTile);
            map.SetTileFlags(mouseCell, TileFlags.None);
            map.SetColor(mouseCell, Color.red);
        }

    }

    void PlaceObstacle()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCell = map.WorldToCell(mouseWorldPos);
        int2 coord = new int2 { x = mouseCell.x, y = mouseCell.y };

        if (obstacles.ContainsKey(coord))
        {
            map.SetTile(new Vector3Int(coord.x, coord.y, 0), null);
            obstacles.Remove(coord);
        }
        else if (!coord.Equals(start.coord) && !coord.Equals(end.coord))
        {
            obstacles.Add(coord, true);
            map.SetTile(mouseCell, defaultTile);
            map.SetTileFlags(mouseCell, TileFlags.None);
            map.SetColor(mouseCell, Color.black);
        }
    }

    void DrawSomething()
    {
        int2 zerothCoord = new int2 {x=0, y=0};

        if (!zerothCoord.Equals(start.coord) && !zerothCoord.Equals(end.coord))
        {
            int DifferenceInTheX = Mathf.Abs(end.coord.x - start.coord.x);
            int DifferenceInTheY = Mathf.Abs(end.coord.y - start.coord.y);

            for(int Xtrack = 1; Xtrack < DifferenceInTheX; Xtrack++)
            {
                if (start.coord.x > end.coord.x)
                {
                    map.SetTile(new Vector3Int(start.coord.x + Xtrack*-1, start.coord.y, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + Xtrack*-1, start.coord.y, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + Xtrack*-1, start.coord.y, 0), Color.blue);
                }else
                {
                    map.SetTile(new Vector3Int(start.coord.x + Xtrack, start.coord.y, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + Xtrack, start.coord.y, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + Xtrack, start.coord.y, 0), Color.blue);
                }
            }

            for (int Ytrack = 1; Ytrack < DifferenceInTheY; Ytrack++)
            {
                if ((start.coord.y > end.coord.y) && start.coord.x > end.coord.x)
                {
                    map.SetTile(new Vector3Int(start.coord.x+DifferenceInTheX*-1, start.coord.y + Ytrack*-1, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + DifferenceInTheX*-1, start.coord.y + Ytrack * -1, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + DifferenceInTheX*-1, start.coord.y + Ytrack * -1, 0), Color.blue);
                }
                else if (start.coord.y > end.coord.y)
                {
                    map.SetTile(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack*-1, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack*-1, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack*-1, 0), Color.blue);
                }
                else if(start.coord.x > end.coord.x)
                {
                    map.SetTile(new Vector3Int(start.coord.x + DifferenceInTheX*-1, start.coord.y + Ytrack, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + DifferenceInTheX*-1, start.coord.y + Ytrack, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + DifferenceInTheX*-1, start.coord.y + Ytrack, 0), Color.blue);
                }
                else
                {
                    map.SetTile(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(start.coord.x + DifferenceInTheX, start.coord.y + Ytrack, 0), Color.blue);
                }
            }
        }
        else
        {
            return;
        }
    }

    void ClearTiles()
    {
        map.ClearAllTiles();

    }

    void PathFind(Node start, Node end)
    {
        openSet = new List<Node>();
        closedSet = new Hashtable();
        cameFrom = new List<Node>();

        Node current = start;

        current.gScore = 0;
        current.hScore = SquaredDistance(current.coord, end.coord);

        openSet.Add(current);

        while(openSet.Count > 0 || giveUp < 0)
        {
        
            Node currentBest = ClosestNode();
            cameFrom.Add(currentBest);

            int2 currentBestCoord = currentBest.coord;
            int2 endCoord = end.coord;

            if (CheckToSeeIfAtGoal(end, currentBest))
            {
                DrawThePath(cameFrom);
                break;
            }

            if (giveUp <= 0)
            {
                break;
            }

            List<Node> neighbors = GetNeighbors(currentBest);
            foreach(Node n in neighbors)
            {
                if (!openSet.Contains(n))
                {
                    
                    map.SetTile(new Vector3Int(n.coord.x, n.coord.y, 0), defaultTile);
                    map.SetTileFlags(new Vector3Int(n.coord.x, n.coord.y, 0), TileFlags.None);
                    map.SetColor(new Vector3Int(n.coord.x, n.coord.y, 0), Color.grey);
                    

                    openSet.Add(n);
                }
            }

            if (!closedSet.ContainsKey(currentBest.coord))
            {
                closedSet.Add(currentBest.coord, true);
            }
            openSet.Remove(currentBest);
            giveUp = giveUp - 1;
            print(giveUp);
        }
    }

    public int SquaredDistance(int2 coordA, int2 coordB)
    {
        int a = coordB.x - coordA.x;
        int b = coordB.y - coordA.y;
        return a * a + b * b;
    }

    public bool CheckToSeeIfAtGoal(Node end, Node currentBest)
    {
        int2[] offsets = new int2[8];

        offsets[0] = new int2(0, 1);
        offsets[1] = new int2(1, 1);
        offsets[2] = new int2(1, 0);
        offsets[3] = new int2(1, -1);
        offsets[4] = new int2(0, -1);
        offsets[5] = new int2(-1, -1);
        offsets[6] = new int2(-1, 0);
        offsets[7] = new int2(-1, 1);

        for (int i = 0; i < offsets.Length; i++)
        {
            int2 trialNode = end.coord + offsets[i];
            if (trialNode.x == currentBest.coord.x && trialNode.y == currentBest.coord.y)
            {
                return true;
            }
        }
        return false;
    }

    public List<Node> GetNeighbors(Node currentBest)
    {
        List<Node> neighbors = new List<Node>();

        int2[] offsets = new int2[8];

        offsets[0] = new int2(0, 1);
        offsets[1] = new int2(1, 1);
        offsets[2] = new int2(1, 0);
        offsets[3] = new int2(1, -1);
        offsets[4] = new int2(0, -1);
        offsets[5] = new int2(-1, -1);
        offsets[6] = new int2(-1, 0);
        offsets[7] = new int2(-1, 1);

        for (int i = 0; i < offsets.Length; i++)
        {
            int2 coordToCheck = currentBest.coord + offsets[i];

            if (!obstacles.ContainsKey(coordToCheck) && !closedSet.ContainsKey(coordToCheck))
            {
                Node neighbour = new Node
                {
                    coord = coordToCheck,
                    parent = currentBest.coord,
                    gScore = currentBest.gScore +
                                SquaredDistance(currentBest.coord, currentBest.coord + offsets[i]),
                    hScore = SquaredDistance(currentBest.coord + offsets[i], end.coord)
                };

                neighbors.Add(neighbour);
            }
        }

        return neighbors;
    }

    public Node ClosestNode()
    {
        Node result = new Node();
        int fScore = int.MaxValue;

        foreach(Node n in openSet)
        {
            if (n.gScore + n.hScore < fScore)
            {
                result = n;
                fScore = n.gScore + n.hScore;
            }
        }
        return result;
    }

    void DrawThePath(List<Node> cameFroms)
    {
        Node LastPlace = cameFroms[cameFroms.Count - 1];

        map.SetTile(new Vector3Int(LastPlace.coord.x, LastPlace.coord.y, 0), defaultTile);
        map.SetTileFlags(new Vector3Int(LastPlace.coord.x, LastPlace.coord.y, 0), TileFlags.None);
        map.SetColor(new Vector3Int(LastPlace.coord.x, LastPlace.coord.y, 0), Color.magenta);

        int2 parentCoord = LastPlace.parent;
        cameFroms.RemoveAt(cameFroms.Count - 1);

        for (int i = cameFroms.Count-1; i >= 0; i--)
        {
            Node currentToCheck = cameFroms[i];
            if (currentToCheck.coord.x == parentCoord.x && currentToCheck.coord.y == parentCoord.y)
            {
                map.SetTile(new Vector3Int(currentToCheck.coord.x, currentToCheck.coord.y, 0), defaultTile);
                map.SetTileFlags(new Vector3Int(currentToCheck.coord.x, currentToCheck.coord.y, 0), TileFlags.None);
                map.SetColor(new Vector3Int(currentToCheck.coord.x, currentToCheck.coord.y, 0), Color.magenta);

                parentCoord = currentToCheck.parent;

                cameFroms.RemoveAt(i);
            }
            else
            {
                cameFroms.RemoveAt(i);
            }
        }
    }
}
