using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A WallObject is an object that represents a wall in the room
 * Primarily used for MovableRoomObjects to know if they are colliding with a wall when we don't want to actually *show* the wall to the player
*/
public class WallObject : RoomObject
{
    public GameObject wallDebugSprite; // For testing purposes only
    public List<GameObject> wallObjects; // Different wall sprites for different orientations
    public bool showWall;
    public void Start()
    {
        if (showWall) DisplayWalls();
        
    }

    //Lets you toggle seeing the wall in the inspector at runtime
    private void OnValidate()
    {
        
        if (showWall)
        {
            DisplayWalls();
        }
        else
        {
            DestroyWallObjects();
        }
    }

    public void DisplayWalls()
    {
        DestroyWallObjects();
        wallObjects = new List<GameObject>();
        GameObject start = Instantiate(wallDebugSprite, transform);
        start.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, 0);
        start.SetActive(true);
        wallObjects.Add(start);
        foreach (Coords tile in properties.relativePositions)
        {
            GameObject GO = Instantiate(wallDebugSprite, transform);
            GO.transform.position = new Vector3((int)transform.position.x + tile.x, (int)transform.position.y + tile.y, 0);
            GO.SetActive(true);
            wallObjects.Add(GO);
        }
    }

    public void DestroyWallObjects()
    {
        if (wallObjects.Count > 0)
        {
            foreach (GameObject go in wallObjects)
            {
                Destroy(go);
            }
        }
    }
}