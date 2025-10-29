using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/*RoomManager manages things to do with the different rooms in a scene
    For instance: what room are you in?
    It also handles transition between rooms (fade to black)
 */
public class RoomManager : MonoBehaviour
{
    public Room activeRoom;
    public Room[] loadedRooms;

    public static event Action<Room> currentRoomChanged;

    public static RoomManager instance;
    private PlayerManager playerManager;
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private Image fadeBlock;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None)[0];

        checkForIce(activeRoom);

        // Get actions started
        currentRoomChanged += (Room r) => { };
    }



    // Generate a new room at a designated location.
    // We should make this an async process, and we have to be smart about when we do it.

    public void changeCurrentRoom(int nextRoomIndex)
    {
        playerManager.stopMovement();
        Room changeToThis = loadedRooms[nextRoomIndex];

        activeRoom = changeToThis;

        checkForIce(changeToThis);

        //Fade transition
        instance.StartCoroutine(fadeTransition(1.5f, changeToThis));
    }

    public void checkForIce(Room room)
    {
        if (room.containsIce)
        {
            playerManager.currentRoomHasIce(room.gameObject.GetComponent<IceRoomManager>());
            Debug.Log("The current room has ice.");
        }
        else playerManager.noIce();
    }


    private IEnumerator fadeTransition(float timeToFade, Room changeToThis)
    {
        int steps = 20;
        fadeCanvas.gameObject.SetActive(true);
        for(int i = 0; i <= steps; i++)
        {
            fadeBlock.color = new Color(fadeBlock.color.r, fadeBlock.color.g, fadeBlock.color.b, (float) i / (float)steps);
            yield return new WaitForSeconds(timeToFade / steps);
        }

        //TODO: WAIT FOR NEXT ROOM TO BE LOADED
        // Call action when next room is ready- player position will change, among other things
        currentRoomChanged.Invoke(changeToThis);

        for (int i = 0; i <= steps; i++)
        {
            fadeBlock.color = new Color(fadeBlock.color.r, fadeBlock.color.g, fadeBlock.color.b, 1 - (float)i / (float)steps);
            yield return new WaitForSeconds(timeToFade / steps);
        }
        fadeCanvas.gameObject.SetActive(false);

        if(changeToThis.dialogueOnEntry != null)
        {
            DialogueManager.instance.processConversation(changeToThis.dialogueOnEntry);
        } else
        {
            playerManager.startMovement();
        }
    }
}