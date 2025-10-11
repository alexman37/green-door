using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MovableObjectManager handles player interaction with movable objects in traffic jam style gameplay.
 * Manages object selection, dragging, and movement validation.
 */
public class MovableObjectManager : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode selectKey = KeyCode.Mouse0; // Left mouse button
    public KeyCode dragKey = KeyCode.Mouse0; // Left mouse button held
    
    private MovableRoomObject selectedObject = null;
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None)[0];
        }
    }
    
    private void Update()
    {
        HandleMouseInput();
        HandleKeyboardMovement();
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetKeyDown(selectKey))
        {
            SelectObjectUnderMouse();
        }
        
        if (Input.GetKey(dragKey) && selectedObject != null)
        {
            if (!isDragging)
            {
                StartDragging();
            }
            else
            {
                UpdateDragging();
            }
        }
        
        if (Input.GetKeyUp(dragKey) && isDragging)
        {
            EndDragging();
        }
    }
    
    private void HandleKeyboardMovement()
    {
        if (selectedObject == null) return;
        
        // Arrow key movement for selected object
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedObject.TryMoveInDirection(Direction.NORTH);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedObject.TryMoveInDirection(Direction.SOUTH);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedObject.TryMoveInDirection(Direction.WEST);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedObject.TryMoveInDirection(Direction.EAST);
        }
    }
    
    private void SelectObjectUnderMouse()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Coords mouseGridPos = new Coords(
            Mathf.FloorToInt(mouseWorldPos.x),
            Mathf.FloorToInt(mouseWorldPos.y)
        );
        
        RoomObject roomObject = RoomManager.instance.activeRoom.getRoomObjectAt(mouseGridPos);
        MovableRoomObject movableObject = roomObject as MovableRoomObject;
        
        // Deselect previous object
        if (selectedObject != null)
        {
            selectedObject.SetSelected(false);
        }
        
        // Select new object
        selectedObject = movableObject;
        if (selectedObject != null)
        {
            selectedObject.SetSelected(true);
        }
    }
    
    private void StartDragging()
    {
        if (selectedObject == null) return;
        
        isDragging = true;
        dragStartPosition = Input.mousePosition;
    }
    
    private void UpdateDragging()
    {
        if (selectedObject == null) return;
        
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 dragDelta = currentMousePos - dragStartPosition;
        
        // Convert screen space drag to world space direction
        float dragThreshold = 50f; // Pixels needed to trigger movement
        
        if (Mathf.Abs(dragDelta.x) > dragThreshold || Mathf.Abs(dragDelta.y) > dragThreshold)
        {
            Direction dragDirection;
            
            if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
            {
                dragDirection = dragDelta.x > 0 ? Direction.EAST : Direction.WEST;
            }
            else
            {
                dragDirection = dragDelta.y > 0 ? Direction.NORTH : Direction.SOUTH;
            }
            
            if (selectedObject.TryMoveInDirection(dragDirection))
            {
                dragStartPosition = currentMousePos; // Reset drag start for continuous movement
            }
        }
    }
    
    private void EndDragging()
    {
        isDragging = false;
        // Deselect previous object
        if (selectedObject != null)
        {
            selectedObject.SetSelected(false);
        }
    }
   
}