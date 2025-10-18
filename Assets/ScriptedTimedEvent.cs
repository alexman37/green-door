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

    public static Image fadingScreen;
    public static Image pictureScreen;

    // optional - use if you wish to create this scripted event from the inspector
    public ScriptedEventInputs inputs;

    private void Start()
    {
        if(coroutine == null && inputs != null)
        {
            setupCoroutine(inputs);
        }
        if(fadingScreen == null)
        {
            fadingScreen = GameObject.FindGameObjectWithTag("FadingCanvas").GetComponent<Image>();
        }
        if (pictureScreen == null)
        {
            pictureScreen = GameObject.FindGameObjectWithTag("PictureCanvas").GetComponent<Image>();
        }
    }

    // If you need to reuse the coroutine, just call this method again
    public void setupCoroutine(ScriptedEventInputs inputs, bool t)
    {
        tethered = t;
        coroutine = inputs.getCoroutine();
    }

    public void setupCoroutine(ScriptedEventInputs inputs)
    {
        coroutine = inputs.getCoroutine();
    }

    public static IEnumerator getCoroutine(ScriptedEventInputs inputs)
    {
        return inputs.getCoroutine();
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
}

/// <summary>
/// Generic Scripted event type
/// </summary>
[System.Serializable]
public abstract class ScriptedEventInputs
{
    public ScriptedEventType eventType;
    public float totalTime;

    public ScriptedEventInputs(ScriptedEventType eventType)
    {
        this.eventType = eventType;
    }

    public abstract IEnumerator getCoroutine();
}



/// <summary>
/// Movement event. Move something from one location to another
/// </summary>
[System.Serializable]
public class SEInputs_Movement : ScriptedEventInputs
{
    public GameObject focus;
    public Vector3 delta;
    public float time;
    public SEMovementType moveType;

    public SEInputs_Movement(GameObject f, Vector3 d, float t, SEMovementType m) : base((m == SEMovementType.LINEAR) ? ScriptedEventType.MOVEMENT : ScriptedEventType.MOVEMENT_X)
    {
        focus = f;
        delta = d;
        time = t;
        totalTime = t;
    }

    public override IEnumerator getCoroutine()
    {
        Vector3 startPos = focus.transform.position;
        if(time > 0)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                switch (moveType)
                {
                    case SEMovementType.LINEAR: focus.transform.position = Vector3.Lerp(startPos, startPos + delta, Mathf.Clamp(t / time, 0, 1)); break;
                    case SEMovementType.EXPONENTIAL: focus.transform.position = Utils.XerpStandard(startPos, startPos + delta, Mathf.Clamp(t / time, 0, 1)); break;
                }

                yield return null;
            }
        }
        
        focus.transform.position = startPos + delta;
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Enabling event. Enable or disable a game object
/// </summary>
[System.Serializable]
public class SEInputs_Enable : ScriptedEventInputs
{
    public GameObject focus;
    public bool enable;

    public SEInputs_Enable(GameObject f, bool s) : base(ScriptedEventType.ENABLE)
    {
        focus = f;
        enable = s;
        totalTime = 0;
    }

    public override IEnumerator getCoroutine()
    {
        focus.SetActive(enable);
        yield return null;
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Fade event. Fade the screen into, or out of, black
/// </summary>
[System.Serializable]
public class SEInputs_Fade : ScriptedEventInputs
{
    public bool inOrOut;
    public float time;
    private static Image fadingScreen;

    public SEInputs_Fade(bool f, float t) : base(ScriptedEventType.FADE)
    {
        inOrOut = f;
        time = t;
        totalTime = t;
        if (fadingScreen == null) fadingScreen = ScriptedTimedEvent.fadingScreen;
    }

    public override IEnumerator getCoroutine()
    {
        Color col = fadingScreen.color;
        if(time > 0)
        {
            for (float t = 0; t <= time; t += Time.deltaTime)
            {
                fadingScreen.color = new Color(col.r, col.g, col.b, (inOrOut ? (t / time) : 1 - (t / time)));
                yield return null;
            }
        }
        fadingScreen.color = new Color(col.r, col.g, col.b, (inOrOut ? 1 : 0));
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Show event. Display an image on screen
/// </summary>
[System.Serializable]
public class SEInputs_Show : ScriptedEventInputs
{
    public Sprite content;
    public PictureTransitionType trans;
    public float time;
    private static Image pictureScreen;

    public SEInputs_Show(Sprite c, PictureTransitionType tr, float t) : base(ScriptedEventType.SHOW)
    {
        content = c;
        trans = tr;
        time = t;
        totalTime = t;
        if (pictureScreen == null) pictureScreen = ScriptedTimedEvent.pictureScreen;
    }
    
    public override IEnumerator getCoroutine()
    {
        Color col = pictureScreen.color;
        pictureScreen.sprite = content;
        if (trans == PictureTransitionType.FADE && time > 0)
        {
            pictureScreen.color = new Color(col.r, col.g, col.b, 0);
            for (float t = 0; t <= time; t += Time.deltaTime)
            {
                pictureScreen.color = new Color(col.r, col.g, col.b, t / time);
                yield return null;
            }
        }
        pictureScreen.color = new Color(col.r, col.g, col.b, 1);
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Hide event. Fade out or immediately remove the shown image
/// </summary>
[System.Serializable]
public class SEInputs_Hide : ScriptedEventInputs
{
    public PictureTransitionType trans;
    public float time;
    private static Image pictureScreen;

    public SEInputs_Hide(PictureTransitionType tr, float t) : base(ScriptedEventType.HIDE)
    {
        trans = tr;
        time = t;
        totalTime = t;
        if (pictureScreen == null) pictureScreen = ScriptedTimedEvent.pictureScreen;
    }

    public override IEnumerator getCoroutine()
    {
        Color col = pictureScreen.color;
        if (trans == PictureTransitionType.FADE && time > 0)
        {
            for (float t = 0; t <= time; t += Time.deltaTime)
            {
                pictureScreen.color = new Color(col.r, col.g, col.b, col.a - (col.a * t / time));
                yield return null;
            }
        }
        pictureScreen.color = new Color(col.r, col.g, col.b, 0);
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Animation event. Play a new animation on an animated object
/// </summary>
[System.Serializable]
public class SEInputs_Animate : ScriptedEventInputs
{
    public SEInputs_Animate() : base(ScriptedEventType.ANIMATE)
    {

    }

    // TODO
    public override IEnumerator getCoroutine()
    {
        yield return null;
        EventManager.instance.finishEventExecution();
    }
}


public enum ScriptedEventType
{
    MOVEMENT,
    MOVEMENT_X,
    ENABLE,
    FADE,
    SHOW,
    HIDE,
    ANIMATE,
}

public enum SEMovementType
{
    LINEAR,
    EXPONENTIAL
}

public enum PictureTransitionType
{
    FADE,
    SNAP
}