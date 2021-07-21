using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class pathfinder : MonoBehaviour
{
    public struct Node
    {
        public int2 coord;
        public int2 parent;
        public int gScore;
        public int hScore;
    }

    Hashtable obstacles;
    Node start, end;
    int giveUp = 1000;

    public Tilemap map;
    public Tile defaultTile;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        obstacles = new Hashtable();
        start = new Node { coord = int2.zero, parent = int2.zero, gScore = int.MaxValue, hScore = int.MaxValue };
    }

    // Update is called once per frame
    void Update()
    {
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
}
