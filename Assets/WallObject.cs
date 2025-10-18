using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A RoomObject is any object that characters can interact with or at least potentially acknowledge.
 * At a technical level, it's a "thing" in the room that is not the walls or floor.
 * The RoomObject itself represents the physical object in game...all its data is in the easily manipulable RoomObjectProperties
*/
public class WallObject : RoomObject
{
    public GameObject wallDebugSprite; // For testing purposes only
    public List<GameObject> wallObjects; // Different wall sprites for different orientations

    public void Start()
    {
        wallObjects = new List<GameObject>();
        GameObject start = Instantiate(wallDebugSprite, transform);
        start.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, 0);
        start.SetActive(true);
        wallObjects.Add(start);
        foreach(Coords tile in properties.relativePositions)
        {
            GameObject GO = Instantiate(wallDebugSprite, transform);
            GO.transform.position = new Vector3((int)transform.position.x + tile.x, (int)transform.position.y + tile.y, 0);
            GO.SetActive(true);
            wallObjects.Add(GO);
        }
    }
}