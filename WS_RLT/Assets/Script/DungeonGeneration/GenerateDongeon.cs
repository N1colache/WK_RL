using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
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

        [SerializeField] public GameObject InstantiatedChunk;
        public Transform SpawnPoint;
    }
    [SerializeField] private GameObject _generationContainer;   
    public NavMeshSurface surface;
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
    [SerializeField] private GameObject player;
    private GameObject currentPlayer;
    [SerializeField] private GameObject playerCamera;

    private Room[,] grid;

    private Vector2Int startPos = new Vector2Int(0, 0);
    private Vector2Int bossPos;
    private Vector2Int sidePos;
    
    [SerializeField] private string spawnTag;

    private void Start()
    {
        playerCamera.SetActive(false);
        GenerateGrid();
        surface.BuildNavMesh();
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
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
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
                 
                                     
                if (room.GridPosition.y == 0 || room.GridPosition.y == height - 1)
                {
                    room.Type = RoomType.side;
                }
                                     
            }
        }
       
         InstantiateRoom();
         ConnectTeleporters();
       
         Room startRoom = grid[startPos.x, startPos.y];
         StartCoroutine(SpawnPlayerAfterGeneration(startRoom));


        




        
    }

    private void InstantiateRoom()
    {
        foreach (Room room in grid)
        {
            GameObject instance = null;

            if (room.Type == RoomType.Start)
            {
                instance = Instantiate(startRoomPrefab, room.WorldPosition, Quaternion.identity, _generationContainer.transform);
                
                // assigner le spawn point directement
                room.SpawnPoint = instance.transform.Find("PlayerSpawn");
                if (room.SpawnPoint == null)
                    Debug.LogError("Le prefab StartRoom doit avoir un enfant PlayerSpawn !");
                else
                    Debug.Log("Spawn trouvé à " + room.SpawnPoint.position);
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
        foreach (Room room in grid)
        {
            if (room.InstantiatedChunk == null)
                continue;

            Vector2Int pos = room.GridPosition;

            Teleporter[] teleporters = room.InstantiatedChunk.GetComponentsInChildren<Teleporter>();

            foreach (Teleporter tp in teleporters)
            {
                Vector2Int targetPos = pos;

                switch (tp.direction)
                {
                    case Teleporter.Direction.Right:
                        targetPos += Vector2Int.right;
                        break;

                    case Teleporter.Direction.Left:
                        targetPos += Vector2Int.left;
                        break;

                    case Teleporter.Direction.Up:
                        targetPos += Vector2Int.up;
                        break;

                    case Teleporter.Direction.Down:
                        targetPos += Vector2Int.down;
                        break;
                }

                if (targetPos.x >= 0 && targetPos.x < width &&
                    targetPos.y >= 0 && targetPos.y < height)
                {
                    Room targetRoom = grid[targetPos.x, targetPos.y];

                    Teleporter[] targetTps = targetRoom.InstantiatedChunk.GetComponentsInChildren<Teleporter>();

                    foreach (Teleporter targetTp in targetTps)
                    {
                        if (IsOpposite(tp.direction, targetTp.direction))
                        {
                            tp.SetDestination(targetTp.transform.position);
                            tp.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else
                {
                    tp.gameObject.SetActive(false);
                }
            }
        }
    }
    bool IsOpposite(Teleporter.Direction a, Teleporter.Direction b)
    {
        return (a == Teleporter.Direction.Right && b == Teleporter.Direction.Left) ||
               (a == Teleporter.Direction.Left && b == Teleporter.Direction.Right) ||
               (a == Teleporter.Direction.Up && b == Teleporter.Direction.Down) ||
               (a == Teleporter.Direction.Down && b == Teleporter.Direction.Up);
    }


    private IEnumerator SpawnPlayerAfterGeneration(Room startRoom)
    {
        yield return null; // attendre 1 frame pour que l'objet soit instancié

        if (currentPlayer == null)
            currentPlayer = Instantiate(player);

        // Récupérer CharacterController et Camera dans l'enfant
        CharacterController controller = currentPlayer.GetComponentInChildren<CharacterController>();
        Camera cam = Camera.main;

        if (controller == null)
        {
            Debug.LogError("CharacterController introuvable !");
            yield break;
        }

        if (cam == null)
        {
            Debug.LogError("Camera introuvable dans l'enfant du joueur !");
        }
        else
        {
            cam.gameObject.SetActive(true);
            playerCamera = cam.gameObject;
        }

        // Désactiver le controller avant de bouger le joueur
        controller.enabled = false;

        // Déplacer et orienter le joueur au spawn
        currentPlayer.transform.position = startRoom.SpawnPoint.position + Vector3.up * 0.1f;
        currentPlayer.transform.rotation = startRoom.SpawnPoint.rotation;

        // Réactiver le controller
        controller.enabled = true;
        currentPlayer.SetActive(true);
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
