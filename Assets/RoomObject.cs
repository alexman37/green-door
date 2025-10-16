using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A RoomObject is any object that characters can interact with or at least potentially acknowledge.
 * At a technical level, it's a "thing" in the room that is not the walls or floor.
 * The RoomObject itself represents the physical object in game...all its data is in the easily manipulable RoomObjectProperties
*/
public class RoomObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject physicalObjectRef; // Needed bc of main thread BS

    public RoomObjectProperties properties;
    private static DialogueManager dialogueManager;

    public void Initialize()
    {
        dialogueManager = FindObjectsByType<DialogueManager>(FindObjectsSortMode.None)[0];
        physicalObjectRef = this.gameObject;
        
        if(!properties.positionHardcoded)
        {
            // Everything is offset by 0.5 because...that's just the way it is
            properties.absoluteCoords = new Coords(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
        
        properties.roomObjectRef = this;
    }

    // If you are looking at an object and press E, you see its dialogue if there is any
    public void deployDialogue()
    {
        if (properties.dialogueFile != null) dialogueManager.processConversation(properties.dialogueFile);
    }
}

/* RoomObjectProperties stores all the data of the RoomObject
 * When making a "new" of this RoomObject from a template, the first thing we wanna do is actually copy and modify its properties
 * This was necessary once we realized templates were using the old one's position if multiple were being created at once,
 * And they were all clobbering / fighting with each other...yadda yadda...
 */
[System.Serializable]
public class RoomObjectProperties
{
    //Properties:
    public string objectName;
    public TextAsset dialogueFile;
    public Coords absoluteCoords; // Where in the world is this?
    public Coords[] relativePositions; // RELATIVE to absolute coords, the object also extends to these places
    public bool positionHardcoded;

    //Physical Game Object Reference
    [HideInInspector]
    public GameObject physicalObjectRef;
    public RoomObject roomObjectRef;

    // Default const.
    public RoomObjectProperties()
    {

    }

    // All room objects should already have their properties specified in the prefab, we'll just copy them over from there.
    public RoomObjectProperties(RoomObjectProperties template)
    {
        objectName = template.objectName;
        dialogueFile = template.dialogueFile;
        absoluteCoords = new Coords(template.absoluteCoords.x, template.absoluteCoords.y);
        relativePositions = template.relativePositions.Clone() as Coords[];
    }
}