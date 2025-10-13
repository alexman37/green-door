using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A door object is like a RoomObject, but a door.
/// It is either the lone "correct" door of the room, or an incorrect door.
/// Opening the correct door carries you over to the next room.
/// Opening the wrong door means the character who did must suffer consequences.
public class DoorObject : RoomObject
{
    private Animator animator;
    public int nextRoom;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void openDoor()
    {
        animator.SetTrigger("Opened");

        // RoomManager handles the fade transition
        RoomManager.instance.changeCurrentRoom(nextRoom);
    }
}