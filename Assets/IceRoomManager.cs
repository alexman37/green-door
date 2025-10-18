using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceRoomManager : MonoBehaviour
{
    public GameObject wallDebugSprite; // For testing purposes only
    public List<Coords> iceTiles; // Different wall sprites for different orientations
    [Header("Edit Mode Tools")]
    public bool startAddingTiles;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None)[0];
        }
    }

    // Helper method to check if a tile exists
    public bool HasTileAt(Coords coords)
    {
        return iceTiles.Any(tile => tile.x == coords.x && tile.y == coords.y);
    }

    // Helper method to remove a tile (useful for edit mode)
    public void RemoveTileAt(Coords coords)
    {
        iceTiles.RemoveAll(tile => tile.x == coords.x && tile.y == coords.y);
    }

    // Clear all tiles
    public void ClearAllTiles()
    {
        iceTiles.Clear();
    }
}
