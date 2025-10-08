using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



///
/// ROOMS, ROOM TILES, AND COORDINATE SYSTEM
/// 




// A Room is, well, a room. Its "tileArray" is a 2D array representing all "relevant" tiles, AKA walkable ones
// Some objects in this room can be outside of the tileArray bounds
public class Room : MonoBehaviour
{
    public int roomNumber;
    public Coords entryPoint; // Where your characters spawn when entering this room

    public List<RoomObject> roomObjects;

    // Track what objects are in this room, and where
    public Dictionary<Vector2Int, RoomObjectProperties> roomObjectsMap = new Dictionary<Vector2Int, RoomObjectProperties>();



    private void Start()
    {
        foreach(RoomObject ro in roomObjects)
        {
            addRoomObject(ro.properties);
        }
    }

    // Add this room object to the roomTile (if eligible). That means it can no longer be walked on
    public void addRoomObject(RoomObjectProperties ro)
    {
        Debug.Log("Adding object " + ro.objectName + " to " + ro.absoluteCoords.x + "," + ro.absoluteCoords.y + " in room " + roomNumber);
        roomObjectsMap.Add(ro.absoluteCoords.asVector2Int(), ro);

        foreach (Coords c in ro.relativePositions)
        {
            roomObjectsMap.Add(ro.absoluteCoords.offset(c.x, c.y).asVector2Int(), ro);
            Debug.Log("Adding object " + ro.objectName + " to " + ro.absoluteCoords.offset(c.x, c.y) + " in room " + roomNumber);
        }
    }




    // Return room object at this position, if there is one
    public RoomObject getRoomObjectAt(Coords position)
    {
        Debug.Log("Is there anything at " + position);
        if (roomObjectsMap.ContainsKey(position.asVector2Int()))
        {
            Debug.Log("Yes: ");
            return roomObjectsMap[position.asVector2Int()].roomObjectRef;
        }
        else return null;
    }

}


/* Typical coordinate class with some helpful additions */
[System.Serializable]
public class Coords
{
    public int x;
    public int y;
    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // When the tilemap is ready to be built you need to reset the grid to a (0,0) scale
    public Coords offset(int amountX, int amountY)
    {
        return new Coords(this.x + amountX, this.y + amountY);
    }

    public void offsetThis(int amountX, int amountY)
    {
        this.x += amountX;
        this.y += amountY;
    }

    public Vector3Int asVector3Int()
    {
        return new Vector3Int(x, y, 0);
    }

    public Vector2Int asVector2Int()
    {
        return new Vector2Int(x, y);
    }

    public Vector2 asVector2()
    {
        return new Vector2(x, y);
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    public override bool Equals(object obj)
    {
        Coords other = obj as Coords;
        return other.x == this.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}