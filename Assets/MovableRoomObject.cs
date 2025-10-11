using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MovableRoomObject extends RoomObject to add traffic jam-style movement capabilities.
 * Objects can move horizontally or vertically along open spaces in the room.
 * Any object can be made to be movable, which can be nice for various puzzles.
 */
public class MovableRoomObject : RoomObject
{
    [Header("Movement Settings")]
    public MovementDirection allowedDirection = MovementDirection.Horizontal;
    public float moveSpeed = 0.3f;
    public bool isSelected = false;

    private bool isMoving = false;

    //Here we can add a new enum for the type of movement. For example, right now we have it using the mouse,
    // but we could do something like strength puzzles from pokemon, where you can push an object by walking into it.
    //For puzzles like those, you need to be careful not to push the object into a corner for example, where it can't move anymore.

    // Check if this object can move to a specific position
    public bool CanMoveTo(Coords targetPosition)
    {
        if (isMoving) return false;

        Room currentRoom = RoomManager.instance.activeRoom;

        // Check all tiles this object would occupy at the target position
        List<Coords> occupiedTiles = GetOccupiedTiles(targetPosition);

        foreach (Coords tile in occupiedTiles)
        {
            // Check if tile is occupied by another object (excluding this one)
            RoomObject objectAtTile = currentRoom.getRoomObjectAt(tile);
            if (objectAtTile != null && objectAtTile != this)
            {
                return false;
            }
        }

        return true;
    }

    // Get all tiles this object would occupy at a given position
    private List<Coords> GetOccupiedTiles(Coords position)
    {
        List<Coords> tiles = new List<Coords>();
        tiles.Add(position);

        if (properties.relativePositions != null)
        {
            foreach (Coords relativePos in properties.relativePositions)
            {
                tiles.Add(position.offset(relativePos.x, relativePos.y));
            }
        }

        return tiles;
    }

    public void MoveTo(Coords targetPosition)
    {
        if (!CanMoveTo(targetPosition) || isMoving) return;

        StartCoroutine(MoveToCoroutine(targetPosition));
    }

    private IEnumerator MoveToCoroutine(Coords targetPosition)
    {
        isMoving = true;
        Room currentRoom = RoomManager.instance.activeRoom;

        // Remove from old position in room map
        RemoveFromRoomMap(currentRoom);

        // Update logical position
        Coords oldPosition = new Coords(properties.absoluteCoords.x, properties.absoluteCoords.y);
        properties.absoluteCoords = new Coords(targetPosition.x, targetPosition.y);

        // Add to new position in room map
        AddToRoomMap(currentRoom);

        // Animate physical movement
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(targetPosition.x + 0.5f, targetPosition.y + 0.5f, transform.position.z);

        float elapsed = 0f;
        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }

    // Remove this object from the room's object map
    private void RemoveFromRoomMap(Room room)
    {
        room.roomObjectsMap.Remove(properties.absoluteCoords.asVector2Int());

        if (properties.relativePositions != null)
        {
            foreach (Coords relativePos in properties.relativePositions)
            {
                Coords tilePos = properties.absoluteCoords.offset(relativePos.x, relativePos.y);
                room.roomObjectsMap.Remove(tilePos.asVector2Int());
            }
        }
    }

    // Add this object to the room's object map
    private void AddToRoomMap(Room room)
    {
        room.roomObjectsMap.Add(properties.absoluteCoords.asVector2Int(), properties);

        if (properties.relativePositions != null)
        {
            foreach (Coords relativePos in properties.relativePositions)
            {
                Coords tilePos = properties.absoluteCoords.offset(relativePos.x, relativePos.y);
                room.roomObjectsMap.Add(tilePos.asVector2Int(), properties);
            }
        }
    }

    // Try to move in a specific direction by one tile
    public bool TryMoveInDirection(Direction direction)
    {
        if (isMoving) return false;

        // Check if movement is allowed in this direction
        if (!IsDirectionAllowed(direction)) return false;

        Coords targetPosition = properties.absoluteCoords;

        switch (direction)
        {
            case Direction.NORTH:
                targetPosition = properties.absoluteCoords.offset(0, 1);
                break;
            case Direction.SOUTH:
                targetPosition = properties.absoluteCoords.offset(0, -1);
                break;
            case Direction.EAST:
                targetPosition = properties.absoluteCoords.offset(1, 0);
                break;
            case Direction.WEST:
                targetPosition = properties.absoluteCoords.offset(-1, 0);
                break;
        }

        if (CanMoveTo(targetPosition))
        {
            MoveTo(targetPosition);
            return true;
        }

        return false;
    }

    private bool IsDirectionAllowed(Direction direction)
    {
        switch (allowedDirection)
        {
            case MovementDirection.Horizontal:
                return direction == Direction.EAST || direction == Direction.WEST;
            case MovementDirection.Vertical:
                return direction == Direction.NORTH || direction == Direction.SOUTH;
            case MovementDirection.Both:
                return true;
            case MovementDirection.None:
                return false;
            default:
                return false;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = selected ? Color.yellow : Color.white;
        }
    }
}

public enum MovementDirection
{
    None,
    Horizontal,
    Vertical,
    Both
}