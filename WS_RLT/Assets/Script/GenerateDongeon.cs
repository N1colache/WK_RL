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
        public Transform SpawnPoint;
    }
    [SerializeField] private GameObject _generationContainer;    
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
        foreach (Transform child in _generationContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
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
                 
                                     
                if (room.GridPosition.y == 0 || room.GridPosition.y == width - 1)
                {
                    room.Type = RoomType.side;
                }
                                     
            }
        }
        List<Room> leftTPRoom = new List<Room>();
        List<Room> rightTPRoom = new List<Room>();
        List<Room> TopTPRoom = new List<Room>();

        foreach (Room room in grid)
        {
            if (room.GridPosition.x + 1 < width)
            {
                Debug.Log("Left Room");
                leftTPRoom.Add(room);
            }

            if (room.GridPosition.x - 1 < width)
            {
                Debug.Log("Right Room");
                rightTPRoom.Add(room);
            }

            if (room.GridPosition.y + 1 < height)
            {
               Debug.Log("Top Room"); 
               TopTPRoom.Add(room);
            }
        }
         InstantiateRoom();
         ConnectTeleporters();
        Debug.Log("Left rooms count : " + Leftroom.Count);
        Debug.Log("Right rooms count : " + Rightroom.Count);
        Debug.Log("Side room number" + sideRoom.Count);
        // Spawn player dans la start room
        Room startRoom = grid[startPos.x, startPos.y];

        if (startRoom.InstantiatedChunk != null)
        {
            Transform spawnPoint = startRoom.InstantiatedChunk.transform.Find("PlayerSpawn");

            if (spawnPoint != null)
            {
                player.position = spawnPoint.position;
            }
            else
            {
                // Fallback si pas de spawn point
                player.position = startRoom.WorldPosition;
                Debug.LogWarning("Aucun PlayerSpawn trouvé dans la start room !");
            }
        }
        




        
    }

    private void InstantiateRoom()
    {
        foreach (Room room in grid)
        {
            GameObject instance = null;

            if (room.Type == RoomType.Start)
            {
                instance = Instantiate(startRoomPrefab, room.WorldPosition, Quaternion.identity, _generationContainer.transform);
            }
            else if (room.Type == RoomType.Boss)
            {
                instance = Instantiate(bossRoomPrefab, room.WorldPosition, Quaternion.identity, _generationContainer.transform);
            }
            else if (room.Type == RoomType.side)
            {
                instance = Instantiate(SideRoomPrefabs[Random.Range(0, SideRoomPrefabs.Count)], room.WorldPosition, Quaternion.identity, _generationContainer.transform);
            }
            else
            {
                instance = Instantiate(middleRoomPrefabs[Random.Range(0, middleRoomPrefabs.Count)], room.WorldPosition, Quaternion.identity, _generationContainer.transform);
            }

            // ICI LA LIGNE CRITIQUE
            room.InstantiatedChunk = instance;
        }
    }


    void ConnectTeleporters()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Room room = grid[x, y];

                if (room.InstantiatedChunk == null)
                    continue;

                // Récupérer les TP dans le prefab
                Teleporter tpRight = room.InstantiatedChunk.transform.Find("TeleporterRight")?.GetComponent<Teleporter>();
                Teleporter tpUp = room.InstantiatedChunk.transform.Find("TeleporterUp")?.GetComponent<Teleporter>();

                // ---- TP VERS LA DROITE ----
                if (x + 1 < width)
                {
                    if (tpRight != null)
                    {
                        Vector3 dest = grid[x + 1, y].WorldPosition;
                        tpRight.SetDestination(dest);
                        tpRight.gameObject.SetActive(true);

                        Debug.Log($"TP droite connecté : {room.GridPosition} → {x + 1},{y}");
                    }
                }
                else
                {
                    if (tpRight != null)
                        tpRight.gameObject.SetActive(false);
                }

                // ---- TP VERS LE HAUT ----
                if (y + 1 < height)
                {
                    if (tpUp != null)
                    {
                        Vector3 dest = grid[x, y + 1].WorldPosition;
                        tpUp.SetDestination(dest);
                        tpUp.gameObject.SetActive(true);

                        Debug.Log($"TP haut connecté : {room.GridPosition} → {x},{y + 1}");
                    }
                }
                else
                {
                    if (tpUp != null)
                        tpUp.gameObject.SetActive(false);
                }
            }
        }
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
