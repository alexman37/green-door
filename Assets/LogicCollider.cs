using UnityEngine;

public class LogicCollider : MonoBehaviour
{
    // Whether or not the player will be allowed to move somewhere else once entering this
    // Useful for Doorways
    public bool stopMovement;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered collider..." + collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exited collider..." + collision.gameObject.name);
    }

    public virtual void onEntered()
    {

    }

    public virtual void onExited()
    {

    }
}
