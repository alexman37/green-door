using UnityEngine;

public class LC_Doorway : LogicCollider
{
    public int goToRoom;

    public override void onEntered()
    {
        Debug.Log("You have entered a doorway...");
        RoomManager.instance.changeCurrentRoom(goToRoom);
    }

    public override void onExited()
    {
        //actually, it does happen when you complete the transition to the next room. doh.
        //Debug.Log("This should never happen");
    }
}
