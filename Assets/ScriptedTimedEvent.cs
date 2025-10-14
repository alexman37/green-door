using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/* See ScriptedEvent. It's the same idea, but these are Coroutines instead of methods.
 * All events run in order and take a certain amount of time to complete
 * */
public class ScriptedTimedEvent : MonoBehaviour
{
    public IEnumerator coroutine;
    public bool tethered;

    // optional - use if you wish to create this scripted event from the inspector
    public ScriptedEventInputs inputs;

    public ScriptedTimedEvent(ScriptedEventInputs inputs)
    {
        setupCoroutine(inputs);
    }

    private void Start()
    {
        if(coroutine == null)
        {
            setupCoroutine(inputs);
        }
    }

    // TODO the coroutine only can be used once. what if we have to reuse it?
    private void setupCoroutine(ScriptedEventInputs inputs)
    {
        switch (inputs.eventType)
        {
            case ScriptedEventType.MOVEMENT:
                coroutine = movementLinearCoroutine(inputs.focusObject, inputs.vector, inputs.time);
                break;
            case ScriptedEventType.MOVEMENT_X:
                coroutine = movementLinearCoroutine(inputs.focusObject, inputs.vector, inputs.time);
                break;
            case ScriptedEventType.ENABLE:
                coroutine = enableCoroutine(inputs.focusObject, inputs.flag);
                break;
            case ScriptedEventType.FADE:
                coroutine = fadeImageCoroutine(inputs.focusObject, inputs.flag, inputs.time);
                break;
        }
    }

    public void trigger()
    {
        if(!tethered)
        {
            StartCoroutine(coroutine);
        } else
        {
            EventManager.instance.QueueEvent(coroutine);
        }
        Destroy(this);
    }

    // Remake the coroutine from scratch if you might need to use this multiple times
    public void triggerReusable()
    {
        setupCoroutine(inputs);
        if (!tethered)
        {
            StartCoroutine(coroutine);
        }
        else
        {
            EventManager.instance.QueueEvent(coroutine);
        }
    }

    public static IEnumerator movementLinearCoroutine(GameObject focus, Vector3 position, float time)
    {
        Vector3 startPos = focus.transform.position;
        for(float t = 0; t < time; t += Time.deltaTime)
        {
            focus.transform.position = Vector3.Lerp(startPos, startPos + position, Mathf.Clamp(t / time, 0, 1));
            yield return null;
        }
        focus.transform.position = startPos + position;
        EventManager.instance.finishEventExecution();
    }

    public static IEnumerator movementExponentialCoroutine(GameObject focus, Vector3 position, float time)
    {
        Vector3 startPos = focus.transform.position;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            focus.transform.position = Utils.XerpStandard(startPos, startPos + position, Mathf.Clamp(t / time, 0, 1));
            yield return null;
        }
        focus.transform.position = startPos + position;
        EventManager.instance.finishEventExecution();
    }

    public static IEnumerator enableCoroutine(GameObject focus, bool enabled)
    {
        focus.SetActive(enabled);
        yield return null;
        EventManager.instance.finishEventExecution();
    }

    public static IEnumerator fadeImageCoroutine(GameObject focus, bool fadingIn, float time)
    {
        Image img = focus.GetComponent<Image>();
        Color col = img.color;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            img.color = new Color(col.r, col.g, col.b, (fadingIn ? (t / time) : 1 - (t / time)));
            Debug.Log("New fade col " + img.color);
            yield return null;
        }
        img.color = new Color(col.r, col.g, col.b, (fadingIn ? 1 : 0));
        EventManager.instance.finishEventExecution();
    }
}

[System.Serializable]
public class ScriptedEventInputs
{
    public ScriptedEventType eventType;
    public GameObject focusObject;
    public Vector3 vector;
    public float time;
    public bool flag;
    public Color col;
}

public enum ScriptedEventType
{
    MOVEMENT,
    MOVEMENT_X,
    ENABLE,
    FADE,
    ANIMATE,
}