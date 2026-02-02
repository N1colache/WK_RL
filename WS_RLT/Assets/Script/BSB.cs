using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Bsp : MonoBehaviour
{
    public class Room
    {
        public Vector2 Size;
        public Vector2 Center;
        public GameObject InstantiatedChunk;
    }
    
    [Header("Container")]
    [SerializeField] private GameObject _generationContainer;

    [Header(" Size")]
    [SerializeField][Range(0, 500)] private float _sizeX;
    [SerializeField][Range(0, 500)] private float _sizeY;

    [Header("Minimum Room Size")]
    [SerializeField][Range(1, 50)] private float _minSizeX;
    [SerializeField][Range(1, 50)] private float _minSizeY;

    [Header("Chunks")]
    [SerializeField] private List<GameObject> _chunkPrefab5X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X10;
    [SerializeField] private List<GameObject> _chunkPrefab20X20;

    [Header("Player")]
    [SerializeField] private Transform _player;

    private List<Room> _rooms;
    private List<Room> _cuttableRooms;

    // MATRICE
    private Room[,] _roomMatrix;
    private int _gridWidth;
    private int _gridHeight;

    // Salles spéciales
    private Room _startRoom;
    private Room _bossRoom;

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
    }

    private void GeneratePattern()
    {
        // Clear
        foreach (Transform child in _generationContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Create first room
        Room rootRoom = new Room
        {
            Size = new Vector2(_sizeX, _sizeY),
            Center = new Vector2(0, 0)
        };

        _rooms = new List<Room> { rootRoom };
        _cuttableRooms = new List<Room> { rootRoom };

        // BSP LOOP
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

            bool doCutVertical;

            if (canCutHorizontally && canCutVertically)
            {
                doCutVertical = Random.Range(0, 2) == 0;
            }
            else
            {
                doCutVertical = canCutVertically;
            }

            _rooms.Remove(roomToCut);

            if (doCutVertical)
            {
                Room roomLeft = new Room();
                Room roomRight = new Room();

                float newWidth = Random.Range(_minSizeX, roomToCut.Size.x - _minSizeX);

                roomLeft.Size = new Vector2(newWidth, roomToCut.Size.y);
                roomRight.Size = new Vector2(roomToCut.Size.x - newWidth, roomToCut.Size.y);

                float offset = (roomToCut.Size.x / 2.0f) - (roomLeft.Size.x / 2.0f);
                roomLeft.Center = new Vector2(roomToCut.Center.x - offset, roomToCut.Center.y);

                offset = (roomToCut.Size.x / 2.0f) - (roomRight.Size.x / 2.0f);
                roomRight.Center = new Vector2(roomToCut.Center.x + offset, roomToCut.Center.y);

                _cuttableRooms.Add(roomLeft);
                _cuttableRooms.Add(roomRight);
            }
            else
            {
                Room roomTop = new Room();
                Room roomBottom = new Room();

                float newHeight = Random.Range(_minSizeY, roomToCut.Size.y - _minSizeY);

                roomTop.Size = new Vector2(roomToCut.Size.x, newHeight);
                roomBottom.Size = new Vector2(roomToCut.Size.x, roomToCut.Size.y - newHeight);

                float offset = (roomToCut.Size.y / 2.0f) - (roomTop.Size.y / 2.0f);
                roomTop.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y - offset);

                offset = (roomToCut.Size.y / 2.0f) - (roomBottom.Size.y / 2.0f);
                roomBottom.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y + offset);

                _cuttableRooms.Add(roomTop);
                _cuttableRooms.Add(roomBottom);
            }
        }

        // Spawn chunks
        foreach (Room room in _rooms)
        {
            var possibleChunkToInstantiate = new List<GameObject>();

            if (room.Size.x >= 20 && room.Size.y >= 20)
                possibleChunkToInstantiate.AddRange(_chunkPrefab20X20);

            if (room.Size.x >= 10)
            {
                if (room.Size.y >= 10)
                    possibleChunkToInstantiate.AddRange(_chunkPrefab10X10);
                else if (room.Size.y >= 5)
                    possibleChunkToInstantiate.AddRange(_chunkPrefab10X5);
            }

            if (room.Size.x >= 5 && room.Size.y >= 5)
                possibleChunkToInstantiate.AddRange(_chunkPrefab5X5);

            GameObject chunkPrefab =
                possibleChunkToInstantiate[Random.Range(0, possibleChunkToInstantiate.Count)];

            room.InstantiatedChunk = Instantiate(
                chunkPrefab,
                new Vector3(room.Center.x, 0, room.Center.y),
                Quaternion.identity,
                _generationContainer.transform
            );
        }

        // 🔥 ICI : ON CONSTRUIT LA MATRICE
        BuildRoomMatrix();

        // Spawn joueur dans la start room
        if (_player != null && _startRoom != null)
        {
            _player.position = new Vector3(
                _startRoom.Center.x,
                0,
                _startRoom.Center.y
            );
        }
    }

    // ================= MATRICE ===================

    private void BuildRoomMatrix()
    {
        float minX = _rooms.Min(r => r.Center.x);
        float maxX = _rooms.Max(r => r.Center.x);

        float minY = _rooms.Min(r => r.Center.y);
        float maxY = _rooms.Max(r => r.Center.y);

        _gridWidth = Mathf.RoundToInt((maxX - minX) / _minSizeX) + 1;
        _gridHeight = Mathf.RoundToInt((maxY - minY) / _minSizeY) + 1;

        _roomMatrix = new Room[_gridWidth, _gridHeight];

        foreach (Room room in _rooms)
        {
            int x = Mathf.RoundToInt((room.Center.x - minX) / _minSizeX);
            int y = Mathf.RoundToInt((room.Center.y - minY) / _minSizeY);

            x = Mathf.Clamp(x, 0, _gridWidth - 1);
            y = Mathf.Clamp(y, 0, _gridHeight - 1);

            _roomMatrix[x, y] = room;
        }

        // Définition des salles spéciales
        _startRoom = _roomMatrix[0, _gridHeight - 1];
        _bossRoom = _roomMatrix[_gridWidth - 1, 0];
    }

    // ============ ACCÈS PUBLIC ==============

    public Room GetStartRoom()
    {
        return _startRoom;
    }

    public Room GetBossRoom()
    {
        return _bossRoom;
    }

    public Vector3 GetStartRoomPosition()
    {
        return new Vector3(_startRoom.Center.x, 0, _startRoom.Center.y);
    }

    public Vector3 GetBossRoomPosition()
    {
        return new Vector3(_bossRoom.Center.x, 0, _bossRoom.Center.y);
    }

    public Room GetRoomAtGridPosition(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _gridWidth || y >= _gridHeight)
            return null;

        return _roomMatrix[x, y];
    }

    // ============ DEBUG GIZMOS ==============

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(_sizeX, 0, _sizeY));

        if (_rooms != null)
        {
            foreach (Room room in _rooms)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(
                    new Vector3(room.Center.x, 0, room.Center.y),
                    new Vector3(room.Size.x, 0, room.Size.y)
                );
            }
        }

        if (_startRoom != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(_startRoom.Center.x, 0, _startRoom.Center.y), 1f);
        }

        if (_bossRoom != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(_bossRoom.Center.x, 0, _bossRoom.Center.y), 1f);
        }
    }
}
