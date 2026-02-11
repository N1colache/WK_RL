using System.Collections.Generic;
using UnityEngine;

public class GridDungeonV2 : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        Boss,
        Side
    }

    public class Room
    {
        public Vector2Int GridPosition;
        public Vector3 WorldPosition;
        public RoomType Type;

        public GameObject InstantiatedChunk;
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
    [SerializeField] private List<GameObject> sideRoomPrefabs;

    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;

    private Room[,] grid;

    private Vector2Int startPos = new Vector2Int(0, 0);
    private Vector2Int bossPos;

    private void Start()
    {
        playerCamera.SetActive(false);
        GenerateDungeon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateDungeon();
        }
    }

    void GenerateDungeon()
    {
        int maxTries = 20;
        int tries = 0;

        bool valid = false;

        while (!valid && tries < maxTries)
        {
            tries++;

            ClearOldDungeon();

            CreateGridData();
            InstantiateRooms();
            ConnectTeleporters();

            Room startRoom = grid[startPos.x, startPos.y];
            Room bossRoom = grid[bossPos.x, bossPos.y];

            bool startOk = RoomHasActiveTeleporter(startRoom);
            bool bossOk = RoomHasActiveTeleporter(bossRoom);
            bool pathOk = IsPathFromStartToBoss();

            valid = startOk && bossOk && pathOk;

            if (!valid)
            {
                Debug.LogWarning("Dungeon invalide → régénération...");
            }
        }

        if (!valid)
        {
            Debug.LogError("Impossible de générer un donjon valide !");
            return;
        }

        PlacePlayerAtStart();
        Debug.Log("Dungeon généré avec succès !");
    }

    void ClearOldDungeon()
    {
        foreach (Transform child in _generationContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void CreateGridData()
    {
        grid = new Room[width, height];
        bossPos = new Vector2Int(width - 1, height - 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Room room = new Room();

                room.GridPosition = new Vector2Int(x, y);
                room.WorldPosition = new Vector3(x * tileSize, 0, y * tileSize);

                if (x == startPos.x && y == startPos.y)
                    room.Type = RoomType.Start;
                else if (x == bossPos.x && y == bossPos.y)
                    room.Type = RoomType.Boss;
                else if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    room.Type = RoomType.Side;
                else
                    room.Type = RoomType.Normal;

                grid[x, y] = room;
            }
        }
    }

    void InstantiateRooms()
    {
        foreach (Room room in grid)
        {
            GameObject prefabToUse = null;

            switch (room.Type)
            {
                case RoomType.Start:
                    prefabToUse = startRoomPrefab;
                    break;

                case RoomType.Boss:
                    prefabToUse = bossRoomPrefab;
                    break;

                case RoomType.Side:
                    prefabToUse = sideRoomPrefabs[Random.Range(0, sideRoomPrefabs.Count)];
                    break;

                default:
                    prefabToUse = middleRoomPrefabs[Random.Range(0, middleRoomPrefabs.Count)];
                    break;
            }

            GameObject instance = Instantiate(
                prefabToUse,
                room.WorldPosition,
                Quaternion.identity,
                _generationContainer.transform
            );

            room.InstantiatedChunk = instance;
        }
    }

    void ConnectTeleporters()
    {
        foreach (Room room in grid)
        {
            Vector2Int pos = room.GridPosition;

            Teleporter tpRight = room.InstantiatedChunk.transform.Find("TeleporterRight")?.GetComponent<Teleporter>();
            Teleporter tpLeft = room.InstantiatedChunk.transform.Find("TeleporterLeft")?.GetComponent<Teleporter>();
            Teleporter tpUp = room.InstantiatedChunk.transform.Find("TeleporterUp")?.GetComponent<Teleporter>();
            Teleporter tpDown = room.InstantiatedChunk.transform.Find("TeleporterDown")?.GetComponent<Teleporter>();

            ConnectSingleTP(tpRight, pos.x + 1, pos.y, "TeleporterLeft");
            ConnectSingleTP(tpLeft, pos.x - 1, pos.y, "TeleporterRight");
            ConnectSingleTP(tpUp, pos.x, pos.y + 1, "TeleporterDown");
            ConnectSingleTP(tpDown, pos.x, pos.y - 1, "TeleporterUp");
        }
    }

    void ConnectSingleTP(Teleporter tp, int x, int y, string targetName)
    {
        if (tp == null) return;

        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            tp.gameObject.SetActive(false);
            return;
        }

        Room targetRoom = grid[x, y];

        Teleporter targetTp = targetRoom.InstantiatedChunk.transform
            .Find(targetName)?.GetComponent<Teleporter>();

        if (targetTp != null)
        {
            tp.SetDestination(targetTp.transform.position);
            tp.gameObject.SetActive(true);
        }
        else
        {
            tp.gameObject.SetActive(false);
        }
    }

    bool RoomHasActiveTeleporter(Room room)
    {
        Teleporter[] tps = room.InstantiatedChunk.GetComponentsInChildren<Teleporter>();

        foreach (var tp in tps)
        {
            if (tp.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    bool IsPathFromStartToBoss()
    {
        Queue<Room> queue = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        Room startRoom = grid[startPos.x, startPos.y];
        Room bossRoom = grid[bossPos.x, bossPos.y];

        queue.Enqueue(startRoom);
        visited.Add(startRoom);

        while (queue.Count > 0)
        {
            Room current = queue.Dequeue();

            if (current == bossRoom)
                return true;

            Vector2Int pos = current.GridPosition;

            TryAddNeighbor(pos.x + 1, pos.y, queue, visited);
            TryAddNeighbor(pos.x - 1, pos.y, queue, visited);
            TryAddNeighbor(pos.x, pos.y + 1, queue, visited);
            TryAddNeighbor(pos.x, pos.y - 1, queue, visited);
        }

        return false;
    }

    void TryAddNeighbor(int x, int y, Queue<Room> queue, HashSet<Room> visited)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return;

        Room neighbor = grid[x, y];

        if (!visited.Contains(neighbor))
        {
            queue.Enqueue(neighbor);
            visited.Add(neighbor);
        }
    }

    void PlacePlayerAtStart()
    {
        Room startRoom = grid[startPos.x, startPos.y];

        Transform spawnPoint = startRoom.InstantiatedChunk.transform.Find("PlayerSpawn");

        Vector3 spawnPosition = spawnPoint != null
            ? spawnPoint.position
            : startRoom.WorldPosition;

        player.transform.position = spawnPosition;
        playerCamera.SetActive(true);
    }
}
