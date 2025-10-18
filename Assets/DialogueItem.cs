using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* A Dialogue Item is used for bits of dialogue that are more advanced than just "people talking"
 * For instance, you may want to specify things happening in the environment: like:
 *  - An object moving
 *  - A character playing a new animation
 *  - Fade to black
 *  - Show me a picture
 * etc...
 * 
 * How it works is, these events are specified like commands in the text dialogue file
 * Commands start with a '$'. The code for parsing them is in Dialogue Parser
 * Some of those commands, like movement or animation, need to be done on a --> specific object <--
 * That's what DialogueItem is for. You specify all needed additional objects here.
 * 
 * In parsing dialogue at any point, you should also accept a simple raw text file, which is still
 * useful for dialogue with no events at all.
 */
public class DialogueItem : MonoBehaviour
{
    public TextAsset rawFile;

    public ObjectWithIDList<GameObject> objectRefs;
    public ObjectWithIDList<Sprite> pictureRefs;

    private void Start()
    {
        objectRefs.buildDictionaries();
        pictureRefs.buildDictionaries();
    }
}



[System.Serializable]
public class ObjectWithIDList<T>
{
    public List<ObjectWithID<T>> listOfObjects;
    Dictionary<string, T> underlyingDict;

    public void buildDictionaries()
    {
        if(listOfObjects != null)
        {
            Debug.Log("Initialized the list of objects with " + listOfObjects.Count + " entries");
            underlyingDict = new Dictionary<string, T>();

            foreach (ObjectWithID<T> obj in listOfObjects)
            {
                underlyingDict.Add(obj.id, obj.content);
            }
        } else
        {
            Debug.Log("Failed to initialize a list of objects!");
            listOfObjects = new List<ObjectWithID<T>>();
            underlyingDict = new Dictionary<string, T>();
        }
    }

    public T getOfId(string id)
    {
        return underlyingDict[id];
    }
}

[System.Serializable]
public class ObjectWithID<T>
{
    public T content;
    public string id;
}