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

    //pour la matrice
    private Room[,] _roomMatrix;
    private int _gridWidth;
    private int _gridHeight;

    // Salles spéciales
    private Room _startRoom;
    private Room _bossRoom;
    private Room _specialRoom;
    
    [Header("Special Rooms Size")]
    [SerializeField] private Vector2 _startRoomSize = new Vector2(20, 20);
    [SerializeField] private Vector2 _bossRoomSize = new Vector2(20, 20);


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

// List<Room>
    private void GeneratePattern()
    {
        // Clear previously created rooms
        foreach (Transform child in _generationContainer.transform)
        {
            Destroy(child.gameObject);
        }
    
        //Create first Room
        _rooms = new List<Room>();
        _cuttableRooms = new List<Room>();
        
        _startRoom = new Room
        {
            Size = _startRoomSize,
            Center = new Vector2(
                -_sizeX / 2f + _startRoomSize.x / 2f,
                _sizeY / 2f - _startRoomSize.y / 2f
            )
        };

        _rooms.Add(_startRoom);
        
        _bossRoom = new Room
        {
            Size = _bossRoomSize,
            Center = new Vector2(
                _sizeX / 2f - _bossRoomSize.x / 2f,
                -_sizeY / 2f + _bossRoomSize.y / 2f
            )
        };

        _rooms.Add(_bossRoom);


        Room rootRoom = new Room
        {
            Size = new Vector2(_sizeX, _sizeY),
            Center = Vector2.zero
        };

        _cuttableRooms.Add(rootRoom);


        _rooms = new List<Room> { rootRoom };

        _cuttableRooms = new List<Room> { rootRoom };

        // Binary Space Partitioning (BSP)
        while (_cuttableRooms.Count > 0)
        {
            //While can cut a room in 2, do it
            //If cut in 2, create new rooms
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

            // If code reach this point => Cut the room
            if (doCutVertical)
            {
                // Cut Vertical
                Room roomLeft = new Room();
                Room roomRight = new Room();

                // Size
                float newWidth = Random.Range(_minSizeX, roomToCut.Size.x - _minSizeX);

                roomLeft.Size = new Vector2(newWidth, roomToCut.Size.y);
                roomRight.Size = new Vector2(roomToCut.Size.x - newWidth, roomToCut.Size.y);

                // center
                float offset = (roomToCut.Size.x / 2.0f) - (roomLeft.Size.x / 2.0f);
                roomLeft.Center = new Vector2(roomToCut.Center.x - offset, roomToCut.Center.y);

                offset = (roomToCut.Size.x / 2.0f) - (roomRight.Size.x / 2.0f);
                roomRight.Center = new Vector2(roomToCut.Center.x + offset, roomToCut.Center.y);

                // Add room to cuttable rooms
                _cuttableRooms.Add(roomLeft);
                _cuttableRooms.Add(roomRight);
            }
            else
            {
                // Cut Horizontal
                Room roomTop = new Room();
                Room roomBottom = new Room();

                float newHeight = Random.Range(_minSizeY, roomToCut.Size.y - _minSizeY);

                roomTop.Size = new Vector2(roomToCut.Size.x, newHeight);
                roomBottom.Size = new Vector2(roomToCut.Size.x, roomToCut.Size.y - newHeight);

                float offset = (roomToCut.Size.y / 2.0f) - (roomTop.Size.y / 2.0f);
                roomTop.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y - offset);

                offset = (roomToCut.Size.y / 2.0f) - (roomBottom.Size.y / 2.0f);
                roomBottom.Center = new Vector2(roomToCut.Center.x, roomToCut.Center.y + offset);

                // Add room to cuttable rooms
                _cuttableRooms.Add(roomTop);
                _cuttableRooms.Add(roomBottom);
            }
        }

        // Spawn chunks
        foreach (Room room in _rooms)
        {
            var possibleChunkToInstantiate = new List<GameObject>();

            if (room.Size is { x: >= 20, y: >= 20 })
            {
                possibleChunkToInstantiate.AddRange(_chunkPrefab20X20);
            }

            if (room.Size.x >= 10)
            {
                if (room.Size.y >= 10)
                {
                    possibleChunkToInstantiate.AddRange(_chunkPrefab10X10);
                } else if (room.Size.y >= 5)
                {
                    possibleChunkToInstantiate.AddRange(_chunkPrefab10X5);
                }
            }

            if (room.Size is { x: >= 5, y: >= 5 })
            {
                possibleChunkToInstantiate.AddRange(_chunkPrefab5X5);
            }
     
            GameObject chunkPrefab = possibleChunkToInstantiate[Random.Range(0, possibleChunkToInstantiate.Count)];
     
            room.InstantiatedChunk = Instantiate(chunkPrefab, new Vector3(room.Center.x, 0, room.Center.y), Quaternion.identity, _generationContainer.transform);
        }
        // Connect teleporters
            foreach (Room room in _rooms)
            {
                    // 2. Chercher les 3 plus proches
                 List<Room> neighborRooms = new List<Room>();
                 foreach (Room otherRoom in _rooms)
                 {
                        // Ignore self
                         if (otherRoom == room) continue;

                        // Distance to otherRoom
                         float distance = Vector2.Distance(room.Center, otherRoom.Center);

                         if (neighborRooms.Count < 3)
                        {
                            neighborRooms.Add(otherRoom);  
                        } 
                         else
                         {
                             for (int i = 0; i < 3; i++)
                             {
                                 Room neighborRoom = neighborRooms[i];
                                 // Check if closer than existing neighbor room
                                 if (distance < Vector2.Distance(room.Center, neighborRoom.Center))
                                 {
                                     neighborRooms.Remove(neighborRoom);
                                     neighborRooms.Add(otherRoom);
                                     break;
                                 }
                             }
                        } 
                 }
    
                // Connect teleporters
                List<Teleporter> myTeleporters = new List<Teleporter>();
                myTeleporters = room.InstantiatedChunk.GetComponentsInChildren<Teleporter>().ToList();

                foreach (Room neighborRoom in neighborRooms)
                {
                    List<Teleporter> neighborTeleporters = new List<Teleporter>();
                    neighborTeleporters = neighborRoom.InstantiatedChunk.GetComponentsInChildren<Teleporter>().ToList();

                    bool connected = false;
                    foreach (Teleporter neighborTeleporter in neighborTeleporters)
                    {
                        if (neighborTeleporter.GetDestinationTeleporter() == null)
                        {
                            foreach (Teleporter myTeleporter in myTeleporters)
                            {
                                if (myTeleporter.GetDestinationTeleporter() == null)
                                {
                                    myTeleporter.SetDestinationTeleporter(neighborTeleporter);
                                    neighborTeleporter.SetDestinationTeleporter(myTeleporter);
                                    connected = true;
                                    break;
                                }
                            }
                        }

                        if (connected)
                        {
                            break;
                        }
                    }
                }
            }
        BuildRoomMatrix();
        // Spawn joueur dans la start room
        if (_player != null && _startRoom != null)
        {
            _player.position = new Vector3(_startRoom.Center.x, 0, _startRoom.Center.y);
        }
    }
    

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
        List<float> _specialRooms = new List<float>();
        _specialRoom = _roomMatrix[_gridWidth - 5, _gridHeight - 5];
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;// voir toute les salles
        Gizmos.DrawWireCube(transform.position, new Vector3(_sizeX, 0, _sizeY));

        if (_rooms != null)
        {
            foreach (Room room in _rooms)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(
                    new Vector3(room.Center.x, 0, room.Center.y),
                    new Vector3(room.Size.x, 0, room.Size.y)
                );
            }
        }

        if (_startRoom != null)
        {
            Gizmos.color = Color.green;// voir la première salle
            Gizmos.DrawCube(
                new Vector3(_startRoom.Center.x, 0, _startRoom.Center.y),
                new Vector3(_startRoom.Size.x, 0, _startRoom.Size.y)
                );
        }

        if (_bossRoom != null)
        {
            Gizmos.color = Color.red;// voir la boss room
            Gizmos.DrawCube(
              new Vector3(_bossRoom.Center.x, 0, _bossRoom.Center.y),
              new Vector3(_bossRoom.Size.x, 0, _bossRoom.Size.y)
              
            );
            //Gizmos.DrawSphere(new Vector3(_bossRoom.Center.x, 0, _bossRoom.Center.y), 4.0f);
        }
        if (_specialRoom != null)
        {
            Gizmos.color = Color.yellow;// voir la boss room
            Gizmos.DrawCube(
                new Vector3(_specialRoom.Center.x, 0, _specialRoom.Center.y),
                new Vector3(_specialRoom.Size.x, 0, _specialRoom.Size.y)
              
            );
            
            //Gizmos.DrawSphere(new Vector3(_bossRoom.Center.x, 0, _bossRoom.Center.y), 4.0f);
        }
        else
        {
            _specialRoom = _roomMatrix[2,4];
            Gizmos.color = Color.white;Gizmos.DrawCube(
                new Vector3(_specialRoom.Center.x, 0, _specialRoom.Center.y),
                new Vector3(_specialRoom.Size.x, 0, _specialRoom.Size.y)
              
            );
        }
    }
}
