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
    
    
    [SerializeField] private GameObject _generationContainer;
    
    [SerializeField][Range(0, 500)] private float _sizeX;
    [SerializeField][Range(0, 500)] private float _sizeY;
   
    //Minimum size
    [SerializeField][Range(1, 50)] private float _minSizeX;
    [SerializeField][Range(1, 50)] private float _minSizeY;
    
    // Chances to cut rooms
    //[SerializeField][Range(0, 1)] private float _verticalCutChance;
    //[SerializeField][Range(0, 1)] private float _horizontalCutChance;
    
    // Chunks
    [SerializeField] private List<GameObject> _chunkPrefab5X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X5;
    [SerializeField] private List<GameObject> _chunkPrefab10X10;
    [SerializeField] private List<GameObject> _chunkPrefab20X20;
    
    // Room
    private List<Room> _rooms;
    private List<Room> _cuttableRooms;
    [SerializeField] public Transform _player;
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
        Room rootRoom = new Room
        {
            //Initialisation
            Size = new Vector2(_sizeX, _sizeY),
            Center = new Vector2(0, 0)
        };

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
            List<Room> neighborRooms = new List<Room>();
            
            foreach (Room otherRoom in _rooms)
            {
                if (otherRoom == room) continue;

                // Distance to other room
                float distance = Vector2.Distance(room.Center, otherRoom.Center);

                // Add room if less than 3 in list
                if (neighborRooms.Count < 3)
                {
                    neighborRooms.Add(otherRoom);
                }
                else
                {
                    // Check distance with each room and add to list if closer
                    for (int i = 0; i < 3; i++)
                    {
                        Room neighborRoom = neighborRooms[i];
                        if (distance < Vector2.Distance(room.Center, neighborRoom.Center))
                        {
                            neighborRooms.Remove(neighborRoom);
                            neighborRooms.Add(otherRoom);
                            break;
                        }
                    }
                } 
            }
            
            //List<Teleporter> myTeleporters = new List<Teleporter>();

           // myTeleporters = room.InstantiatedChunk.GetComponentsInChildren<Teleporter>().ToList();

            foreach (var neighborRoom in neighborRooms)
            {
                //List<Teleporter> neighborTeleporters = new List<Teleporter>();
                
                //neighborTeleporters = neighborRoom.InstantiatedChunk.GetComponentsInChildren<Teleporter>().ToList();
                
                bool connected = false;
               // foreach (var neighborTeleporter in neighborTeleporters)
                {
                  //  if (neighborTeleporter.SetDestinationTeleporter() == null)
                    {
                       // foreach (var myTeleporter in myTeleporters)
                        {
                          //  if (myTeleporter.SetDestinationTeleporter() == null)
                            {
                             //   myTeleporter.SetDestinationTeleporter(neighborTeleporter);
                              //  neighborTeleporter.SetDestinationTeleporter(myTeleporter);
                                connected = true;
                                break;
                            }
                        }
                    }

                    if (connected) break;
                }
            }
        }
        if (_player != null)
        {
            _player.position = GetRoomContainingTopLeftPosition();
        }
      
    }
    private Room GetRoomContainingTopLeftCorner()
    {
        Vector2 corner = new Vector2(-_sizeX / 2f, _sizeY / 2f);

        foreach (Room room in _rooms)
        {
            Rect roomRect = new Rect(room.Center - room.Size / 2f, room.Size);

            if (roomRect.Contains(corner))
                return room;
        }

        return null;
    }

    private Vector3 GetRoomContainingTopLeftPosition()
    {
        Room room = GetRoomContainingTopLeftCorner();

        if (room == null)
            return transform.position;

        return new Vector3(room.Center.x, 0, room.Center.y);
    }


    private void OnDrawGizmos()
    {
        //this function allows to draw a wire cube on the screen
        Gizmos.DrawWireCube(transform.position, new Vector3(_sizeX, 0, _sizeY));

        if (_rooms != null)
        {
            foreach (Room room in _rooms)
            {
                Gizmos.DrawWireCube(new Vector3(room.Center.x, 0, room.Center.y), new Vector3(room.Size.x, 0, room.Size.y));
            } 
        }
        Room topLeft = GetRoomContainingTopLeftCorner();

        if (topLeft != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3(topLeft.Center.x, 0, topLeft.Center.y), 1f);
        }

    }
}
