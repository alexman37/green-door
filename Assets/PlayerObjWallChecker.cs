using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Unfortunately there is an edge case with movement, where if you rotate the player's "aheadCollider" box
/// into a wall that you are about to move into, it doesn't register the collision in time. Thus we have to use
/// this stupid class...ugh.
/// </summary>
public class PlayerObjWallChecker : MonoBehaviour
{
    public Direction direction;

    public static Action<Direction, Collision2D> checkerCollisionEnter;
    public static Action<Direction, Collision2D> checkerCollisionExit;

    public void Start()
    {
        if(checkerCollisionEnter == null)
        {
            checkerCollisionEnter += (_, __) => { };
        }
        if (checkerCollisionExit == null)
        {
            checkerCollisionExit += (_, __) => { };
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LogicCollider possibleLC = collision.gameObject.GetComponent<LogicCollider>();
        // Normal wall.
        if (possibleLC == null)
        {
            checkerCollisionEnter.Invoke(direction, collision);
        }
        // If not, do something special when entering this collider
        else
        {
            possibleLC.onEntered();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LogicCollider possibleLC = collision.gameObject.GetComponent<LogicCollider>();
        // Normal wall.
        if (possibleLC == null)
        {
            checkerCollisionExit.Invoke(direction, collision);
        }
        // If not, do something special when entering this collider
        else
        {
            possibleLC.onExited();
        }
    }
}
