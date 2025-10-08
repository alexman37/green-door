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

    public static Action<Direction> checkerCollisionEnter;
    public static Action<Direction> checkerCollisionExit;

    public void Start()
    {
        if(checkerCollisionEnter == null)
        {
            checkerCollisionEnter += (_) => { };
        }
        if (checkerCollisionExit == null)
        {
            checkerCollisionExit += (_) => { };
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        checkerCollisionEnter.Invoke(direction);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        checkerCollisionExit.Invoke(direction);
    }
}
