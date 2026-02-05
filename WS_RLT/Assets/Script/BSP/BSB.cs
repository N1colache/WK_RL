using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Bsp : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        Boss,
        Special
    }

    public class Room
    {
        public Vector2 Size;
        public Vector2 Center;

        public RoomType Type;

        public GameObject ChunkPrefab;
        public GameObject InstantiatedChunk;

        public bool IsInstantiated;
    }
    [SerializeField] private GameObject _generationContainer;
    [SerializeField] private Transform _player;

    [SerializeField][Range(0, 500)] private float _sizeX;
    [SerializeField][Range(0, 500)] private float _sizeY;

    [SerializeField][Range(1, 50)] private float _minSizeX;
    [SerializeField][Range(1, 50)] private float _minSizeY;

    [Header("Chunks Normaux")]
    [SerializeField] private List<GameObject> _chunkPrefab5X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X10;
    [SerializeField] private List<GameObject> _chunkPrefab20X20;

    [Header("Chunks Spéciaux")]
    [SerializeField] private GameObject _startRoomPrefab;
    [SerializeField] private GameObject _bossRoomPrefab;
    [SerializeField] private GameObject _specialRoomPrefab;

    private List<Room> _rooms;
    private List<Room> _cuttableRooms;

    private List<Room> _mainPath;
    private int _currentRoomIndex = 0;

    private void Start()
    {
        GeneratePattern();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GeneratePattern();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }

        UpdateVisibleRooms();
    }

    private void GeneratePattern()
    {
       

        foreach (Transform child in _generationContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Room rootRoom = new Room
        {
            Size = new Vector2(_sizeX, _sizeY),
            Center = new Vector2(0, 0)
        };

        _rooms = new List<Room> { rootRoom };
        _cuttableRooms = new List<Room> { rootRoom };

        while (_cuttableRooms.Count > 0)
        {
            Room roomToCut = _cuttableRooms[0];
            _cuttableRooms.RemoveAt(0);

            bool canCutVertically = roomToCut.Size.x / 2.0f >= _minSizeX;
            bool canCutHorizontally = roomToCut.Size.y / 2.0f >= _minSizeY;

            if (!canCutVertically && !canCutHorizontally)
            {
                _rooms.Add(roomToCut);
                continue;
            }

            bool doCutVertical = canCutVertically && (!canCutHorizontally || Random.Range(0, 2) == 0);

            _rooms.Remove(roomToCut);

            if (doCutVertical)
            {
                float newWidth = Random.Range(_minSizeX, roomToCut.Size.x - _minSizeX);

                Room left = new Room();
                Room right = new Room();

                left.Size = new Vector2(newWidth, roomToCut.Size.y);
                right.Size = new Vector2(roomToCut.Size.x - newWidth, roomToCut.Size.y);

                float offset = (roomToCut.Size.x / 2f) - (left.Size.x / 2f);
                left.Center = new Vector2(roomToCut.Center.x - offset, roomToCut.Center.y);

                offset = (roomToCut.Size.x / 2f) - (right.Size.x / 2f);
                right.Center = new Vector2(roomToCut.Center.x + offset, roomToCut.Center.y);

                _cuttableRooms.Add(left);
                _cuttableRooms.Add(right);
            }
            else
            {
                float newHeight = Random.Range(_minSizeY, roomToCut.Size.y - _minSizeY);

                Room top = new Room();
                Room bottom = new Room();

                top.Size = new Vector2(roomToCut.Size.x, newHeight);
                bottom.Size = new Vector2(roomToCut.Size.x, roomToCut.Size.y - newHeight);

                float offset = (roomToCut.Size.y / 2f) - (top.Size.y / 2f);
                top.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y - offset);

                offset = (roomToCut.Size.y / 2f) - (bottom.Size.y / 2f);
                bottom.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y + offset);

                _cuttableRooms.Add(top);
                _cuttableRooms.Add(bottom);
            }
        }

        

        
    }

    private void AssignRoomTypes()
    {
        Room startRoom = _rooms
            .OrderByDescending(r => r.Center.x)
            .ThenBy(r => r.Center.y)
            .First();

        Room bossRoom = _rooms
            .OrderBy(r => r.Center.x)
            .ThenByDescending(r => r.Center.y)
            .First();

        startRoom.Type = RoomType.Start;
        bossRoom.Type = RoomType.Boss;

        Debug.Log($"Start Room : {startRoom.Center}");
        Debug.Log($"Boss Room : {bossRoom.Center}");

        foreach (Room r in _rooms)
        {
            if (r != startRoom && r != bossRoom)
            {
                r.Type = RoomType.Normal;
            }
        }

        var sideRooms = _rooms
            .Where(r => r.Type == RoomType.Normal)
            .OrderByDescending(r => Mathf.Abs(r.Center.x))
            .Take(2);

        foreach (var special in sideRooms)
        {
            special.Type = RoomType.Special;
            Debug.Log($"Salle spéciale placée : {special.Center}");
        }
    }

    private List<Room> FindPathFromStartToBoss()
    {
        Room start = _rooms.First(r => r.Type == RoomType.Start);
        Room boss = _rooms.First(r => r.Type == RoomType.Boss);

        List<Room> path = new List<Room>();
        Room current = start;

        path.Add(current);

        while (current != boss)
        {
            Room next = _rooms
                .Where(r => !path.Contains(r))
                .OrderBy(r => Vector2.Distance(r.Center, boss.Center))
                .First();

            path.Add(next);
            current = next;
        }
        
        foreach (var room in path)
        {
            Debug.Log($" -> {room.Type} ({room.Center})");
        }

        return path;
    }

    private GameObject PickChunkForRoom(Room room)
    {
        if (room.Type == RoomType.Start)
            return _startRoomPrefab;

        if (room.Type == RoomType.Boss)
            return _bossRoomPrefab;

        if (room.Type == RoomType.Special)
            return _specialRoomPrefab;

        var list = new List<GameObject>();

        if (room.Size.x >= 20 && room.Size.y >= 20)
            list.AddRange(_chunkPrefab20X20);

        if (room.Size.x >= 10)
        {
            if (room.Size.y >= 10)
                list.AddRange(_chunkPrefab10X10);
            else if (room.Size.y >= 5)
                list.AddRange(_chunkPrefab10X5);
        }

        if (room.Size.x >= 5 && room.Size.y >= 5)
            list.AddRange(_chunkPrefab5X5);

        return list[Random.Range(0, list.Count)];
    }

    private void SpawnRoom(Room room)
    {
        if (room.IsInstantiated) return;

        room.ChunkPrefab = PickChunkForRoom(room);

        room.InstantiatedChunk = Instantiate(
            room.ChunkPrefab,
            new Vector3(room.Center.x, 0, room.Center.y),
            Quaternion.identity,
            _generationContainer.transform
        );

        room.IsInstantiated = true;

    
    }

    private void DespawnRoom(Room room)
    {
        if (!room.IsInstantiated) return;

        Destroy(room.InstantiatedChunk);
        room.IsInstantiated = false;
        
    }

    private void UpdateVisibleRooms()
    {
        for (int i = 0; i < _mainPath.Count; i++)
        {
            Room room = _mainPath[i];

            if (i == _currentRoomIndex || i == _currentRoomIndex + 1)
            {
                if (!room.IsInstantiated)
                    SpawnRoom(room);
            }
            else
            {
                if (room.IsInstantiated)
                    DespawnRoom(room);
            }
        }
    }

   

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_sizeX, 0, _sizeY));

        if (_rooms != null)
        {
            foreach (Room room in _rooms)
            {
                Color c = Color.white;

                if (room.Type == RoomType.Start) c = Color.green;
                if (room.Type == RoomType.Boss) c = Color.red;
                if (room.Type == RoomType.Special) c = Color.yellow;

                Gizmos.color = c;

                Gizmos.DrawWireCube(
                    new Vector3(room.Center.x, 0, room.Center.y),
                    new Vector3(room.Size.x, 0, room.Size.y)
                );
            }
        }
    }
}
