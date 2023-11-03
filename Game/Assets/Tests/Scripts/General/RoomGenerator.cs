using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room info")] 
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor, wallColor;

    [Header("Location control")] 
    public Transform generatorPoint;
    public Vector2 numPos;
    public float xOffset;
    public float yOffset;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };
    public Direction direction;
    public List<Room> rooms = new List<Room>();
    public List<Vector2> posList = new List<Vector2>();
    public List<GameObject> roomSetList;
    public GameObject lastRoom;

    public DecorationGenerator decoration;
    
    public WallType wallType;

    // Start is called before the first frame update
    void Start()
    {
        //System.Random rand = new System.Random(3);
        posList.Add(new Vector2(0, 0));
        
        for (int i = 0; i < roomNumber; i++)
        {
            Vector3 currentPos = generatorPoint.position;
            Room newRoom = Instantiate(roomPrefab, currentPos, Quaternion.identity).GetComponent<Room>();
            newRoom.pointPos = currentPos;
            rooms.Add(newRoom);
            
            decoration.GenerateDecorations(newRoom.pointPos);

            if (i > 0 && i < roomNumber - 1)
            {
                Instantiate(roomSetList[Random.Range(0, roomSetList.Count)], currentPos, Quaternion.identity);
            }
            else if (i == roomNumber - 1)
            {
                Instantiate(lastRoom, currentPos, Quaternion.identity);
            }
            
            ChangePos();
        }

        // remove the unused pos
        posList.RemoveAt(roomNumber);
        
        // temporarily remove endRoom from posList to create gateways between any neighbouring pair of the rest of the rooms
        Vector2 endPos = posList[roomNumber - 1];
        posList.RemoveAt(roomNumber - 1);

        // create all gateways except the one connecting endRoom
        for (int i = 0; i < roomNumber - 1; i++)
        {
            SetUpRoom(rooms[i], posList[i]);
        }
        
        // create gateway connecting endRoom to ensure endRoom only connects the very room before it
        if (roomNumber > 1)
        {
            Vector2 endVec = endPos - posList[roomNumber - 2];
            int x = (int)endVec.x;
            int y = (int)endVec.y;
            int dst = roomNumber - 1;
            int src = roomNumber - 2;

            for (int i = 0; i < 2; i++)
            {
                // first loop for normal case, second for endRoom neighbouring startRoom
                if (x == 0)
                {
                    if (y == 1)
                    {
                        rooms[dst].down = true;
                        rooms[src].up = true;
                        break;
                    }
                    if (y == -1)
                    {
                        rooms[dst].up = true;
                        rooms[src].down = true;
                        break;
                    }
                }
                else if (y == 0)
                {
                    if (x == 1)
                    {
                        rooms[dst].left = true;
                        rooms[src].right = true;
                        break;
                    }
                    if (x == -1)
                    {
                        rooms[dst].right = true;
                        rooms[src].left = true;
                        break;
                    }
                }

                // if endRoom is next to startRoom
                src = 0;
                endVec = endPos - posList[0];
                x = (int)endVec.x;
                y = (int)endVec.y;
            }
            
        }
        
        // set up walls according to doors
        foreach (Room room in rooms)
        {
            room.UpdateDoorNum();
            SetUpWall(room);
        }
        

        rooms[0].GetComponent<SpriteRenderer>().color = startColor;
        rooms[roomNumber - 1].GetComponent<SpriteRenderer>().color = endColor;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.anyKeyDown)
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }
    }

    public void ChangePos()
    {
        Direction newDirection;
        Vector2 newPos;
        int count = 0;

        do
        {
            newDirection = (Direction)Random.Range(0, 4);
            switch (newDirection)
            {
                case Direction.Up:
                    newPos = numPos + new Vector2(0, 1);
                    break;

                case Direction.Down:
                    newPos = numPos + new Vector2(0, -1);
                    break;

                case Direction.Left:
                    newPos = numPos + new Vector2(-1, 0);
                    break;

                case Direction.Right:
                    newPos = numPos + new Vector2(1, 0);
                    break;

                default:
                    numPos = posList[0];
                    newPos = numPos;
                    break;
            }
            
            // deal with endless loops: set pos to starting point
            if (count++ == 16)
            {
                numPos = posList[0];
                newPos = numPos;
            }
            
        } while (posList.Contains(newPos));

        posList.Add(newPos);
        numPos = newPos;
        generatorPoint.position = new Vector3(numPos.x * xOffset, numPos.y * yOffset, 0);
    }

    public void SetUpRoom(Room newRoom, Vector2 pos)
    {
        Vector2 upPos = pos + new Vector2(0, 1);
        Vector2 downPos = pos + new Vector2(0, -1);
        Vector2 leftPos = pos + new Vector2(-1, 0);
        Vector2 rightPos = pos + new Vector2(1, 0);

        newRoom.up = posList.Contains(upPos);
        newRoom.down = posList.Contains(downPos);
        newRoom.left = posList.Contains(leftPos);
        newRoom.right = posList.Contains(rightPos);
    }

    public void SetUpWall(Room newRoom)
    {
        bool u = newRoom.up, d = newRoom.down, l = newRoom.left, r = newRoom.right;
        int n = newRoom.doorNum;
        var roomPosition = newRoom.pointPos;
        var id = Quaternion.identity;

        //GameObject newWall = Instantiate(wallType.WallUDLR, roomPosition, id);

        switch (n)
        {
            case 1:
                if (u)
                    Instantiate(wallType.WallU, roomPosition, id);
                else if (d)
                    Instantiate(wallType.WallD, roomPosition, id);
                else if (l)
                    Instantiate(wallType.WallL, roomPosition, id);
                else if (r)
                    Instantiate(wallType.WallR, roomPosition, id);
                break;
            
            case 2:
                if (u && d)
                    Instantiate(wallType.WallUD, roomPosition, id);
                else if (u && l)
                    Instantiate(wallType.WallUL, roomPosition, id);
                else if (u && r)
                    Instantiate(wallType.WallUR, roomPosition, id);
                else if (d && l)
                    Instantiate(wallType.WallDL, roomPosition, id);
                else if (d && r)
                    Instantiate(wallType.WallDR, roomPosition, id);
                else if (l && r)
                    Instantiate(wallType.WallLR, roomPosition, id);
                break;
            
            case 3:
                if (u && l && r)
                    Instantiate(wallType.WallULR, roomPosition, id);
                else if (u && d && l)
                    Instantiate(wallType.WallUDL, roomPosition, id);
                else if (u && d && r)
                    Instantiate(wallType.WallUDR, roomPosition, id);
                else if (d && l && r)
                    Instantiate(wallType.WallDLR, roomPosition, id);
                break;
            
            case 4:
                Instantiate(wallType.WallUDLR, roomPosition, id);
                break;
            
            default:
                Instantiate(wallType.WallUDLR, roomPosition, id);
                break;
        }
        
        //newWall.GetComponent<SpriteRenderer>().color = wallColor;
    }
}


[System.Serializable]
public class WallType
{
    public GameObject WallU, WallD, WallL, WallR,
        WallUD, WallUL, WallUR, WallDL, WallDR, WallLR,
        WallUDL, WallUDR, WallULR, WallDLR,
        WallUDLR;
}

// [System.Serializable]
// public class SetType
// {
//     
// }