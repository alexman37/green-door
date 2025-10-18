using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(IceRoomManager))]
public class IceRoomManagerEditor : Editor
{
    private IceRoomManager iceRoomManager;

    private void OnEnable()
    {
        iceRoomManager = (IceRoomManager)target;
    }

    private void OnSceneGUI()
    {
        if (!iceRoomManager.startAddingTiles) return;

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            // Convert mouse position to world position
            Vector3 mousePosition = currentEvent.mousePosition;
            mousePosition.y = Camera.current.pixelHeight - mousePosition.y;

            Ray ray = Camera.current.ScreenPointToRay(mousePosition);
            Vector3 worldPos = ray.origin;

            // Convert to grid coordinates
            Coords gridPos = new Coords(
                Mathf.FloorToInt(worldPos.x),
                Mathf.FloorToInt(worldPos.y)
            );

            // Add tile if it doesn't already exist
            if (!iceRoomManager.iceTiles.Any(tile => tile.x == gridPos.x && tile.y == gridPos.y))
            {
                Undo.RecordObject(iceRoomManager, "Add Ice Tile");
                iceRoomManager.iceTiles.Add(gridPos);
                EditorUtility.SetDirty(iceRoomManager);
            }

            currentEvent.Use(); // Consume the event to prevent other handlers
        }

        // Draw visual feedback for existing tiles
        DrawIceTiles();
    }

    private void DrawIceTiles()
    {
        Handles.color = Color.cyan;

        foreach (Coords tile in iceRoomManager.iceTiles)
        {
            Vector3 center = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);
            Vector3[] corners = new Vector3[]
            {
                new Vector3(tile.x, tile.y, 0),
                new Vector3(tile.x + 1, tile.y, 0),
                new Vector3(tile.x + 1, tile.y + 1, 0),
                new Vector3(tile.x, tile.y + 1, 0),
                new Vector3(tile.x, tile.y, 0)
            };

            Handles.DrawPolyLine(corners);
        }
    }
}