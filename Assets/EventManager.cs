using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Executes ScriptedTimedEvent objects in a particular order
 */
public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    private static bool activeEvent;
    private static Queue<IEnumerator> futureEvents;

    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        futureEvents = new Queue<IEnumerator>();
    }


    public void QueueEvent(IEnumerator ev) {
        futureEvents.Enqueue(ev);
        
        if (!activeEvent)
        {
            activeEvent = true;
            IEnumerator next = futureEvents.Dequeue();
            StartCoroutine(next);
        }
        
    }

    public void finishEventExecution()
    {
        activeEvent = false;
        if(futureEvents.Count > 0)
        {
            activeEvent = true;
            IEnumerator next = futureEvents.Dequeue();
            StartCoroutine(next);
        }
    }
}
