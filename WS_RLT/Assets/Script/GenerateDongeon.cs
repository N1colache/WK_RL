using System.Collections.Generic;
using UnityEngine;

public class GridDungeon : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        Boss,
        side
    }

    public class Room
    {
        public Vector2Int GridPosition;
        public Vector3 WorldPosition;

        public RoomType Type;

        public GameObject InstantiatedChunk;
    }
    [Header("Taille de la grille")]
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    [Header("Taille d'une salle")]
    [SerializeField] private float tileSize = 20f;


    [Header("Prefabs")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private List<GameObject> middleRoomPrefabs;
    [SerializeField] private List<GameObject> SideRoomPrefabs;

    [Header("Player")]
    [SerializeField] private Transform player;

    private Room[,] grid;

    private Vector2Int startPos = new Vector2Int(0, 0);
    private Vector2Int bossPos;
    private Vector2Int sidePos;

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateGrid();
        }
    }

    void GenerateGrid()
    {
        List<Room> Rightroom = new List<Room>();
        List<Room> Leftroom = new List<Room>();
        List<Room> sideRoom = new List<Room>();
        grid = new Room[width, height];

        bossPos = new Vector2Int(width - 1, height - 1);

    

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Room room = new Room();
                room.GridPosition = new Vector2Int(x, y);

                

                

                room.WorldPosition = new Vector3(
                    x * tileSize,
                    0,
                    y * tileSize
                );


                if (x == startPos.x && y == startPos.y)
                {
                    room.Type = RoomType.Start;
                }
                else if (x == bossPos.x && y == bossPos.y)
                {
                    room.Type = RoomType.Boss;
                }
                else if (y == sidePos.y && x == sidePos.x)
                {
                    room.Type = RoomType.side;
                }
                else
                {
                    room.Type = RoomType.Normal;
                }

                grid[x, y] = room;
            }
        }
        foreach (Room room in grid)
        {
            if (room.Type != RoomType.Start && room.Type != RoomType.Boss)
            {
                if (room.GridPosition.x == 0)
                {
                    Leftroom.Add(room);
                }
                else if (room.GridPosition.x == width - 1)
                {
                    Rightroom.Add(room);
                }
            }
            sideRoom.Add(room);
            
        }
        Debug.Log("Left rooms count : " + Leftroom.Count);
        Debug.Log("Right rooms count : " + Rightroom.Count);
        Debug.Log("Side room number" + sideRoom.Count);
        // Spawn player dans la start room
        player.position = grid[startPos.x, startPos.y].WorldPosition;

        
    }

    
   
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach (Room room in grid)
        {
            if (room == null) continue;

            switch (room.Type)
            {
                case RoomType.Start:
                    Gizmos.color = Color.green;
                    break;

                case RoomType.Boss:
                    Gizmos.color = Color.red;
                    break;
                case RoomType.side:
                    Gizmos.color = Color.yellow;
                    break;

                default:
                    Gizmos.color = Color.white;
                    break;
            }

            Gizmos.DrawWireCube(
                room.WorldPosition,
                new Vector3(tileSize, 1, tileSize)
            );
        }
    }
}
