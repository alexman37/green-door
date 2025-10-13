using UnityEngine;

/* A ScriptedEvent is something that happens in the game world at a specific time
 * For instance - an object moving when we reach a certain line of dialogue.
 * Or - a light turning on when we hit a certain collider.
 * 
 * In general, ScriptedEvents take in GameObject(s) as parameters, and callback functions that describe what to do with them
 * */
public class ScriptedEvent : MonoBehaviour
{
    public delegate void EventCallback(GameObject focus);

    public GameObject focus;
    public EventCallback callback;

    public void trigger()
    {
        callback(focus);
    }

    /// <summary>
    /// Teleport the focus object to another point relative to its current position
    /// </summary>
    public static EventCallback movementCallback(Vector3 positionTransform)
    {
        return (focus) =>
        {
            focus.transform.position += positionTransform;
        };
    }
}
