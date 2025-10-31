using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public static bool staticSetup = false;

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
        staticSetup = true;
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
public class ScriptedEventInputs
{
    public ScriptedEventType eventType;
    public float totalTime;

    public ScriptedEventInputs(ScriptedEventType eventType)
    {
        this.eventType = eventType;
    }

    public virtual IEnumerator getCoroutine()
    {
        Debug.LogError("Please override the scripted event.");
        return null;
    }
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
/// For objects with a sprite renderer, change the sprite
/// </summary>
[System.Serializable]
public class SEInputs_SpriteSwap : ScriptedEventInputs
{
    public GameObject focus;
    public Sprite newSprite;

    public SEInputs_SpriteSwap(GameObject f, Sprite s) : base(ScriptedEventType.SPRITE_SWAP)
    {
        focus = f;
        newSprite = s;
        totalTime = 0;
    }

    public override IEnumerator getCoroutine()
    {
        focus.GetComponent<SpriteRenderer>().sprite = newSprite;
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
        yield return new WaitUntil(() => ScriptedTimedEvent.staticSetup);
        Debug.Log("Completed static setup.");
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
/// Play music
/// </summary>
[System.Serializable]
public class SEInputs_Music : ScriptedEventInputs
{
    public string musicTrackId;
    public float volume;
    public bool mainMusic; // as opposed to atmospheric

    public SEInputs_Music(string trackId, float vol, bool main) : base(ScriptedEventType.MUSIC)
    {
        musicTrackId = trackId;
        volume = vol;
        mainMusic = main;
    }

    public override IEnumerator getCoroutine()
    {
        yield return null;
        MusicManager whereToPlay = mainMusic ? MusicManager.mainInstance : MusicManager.ambientInstance;
        if (musicTrackId == "stop")
        {
            whereToPlay.stopMusic();
        }
        else if (musicTrackId == "fade")
        {
            whereToPlay.fadeOutCurrent();
        }
        else
        {
            whereToPlay.playTrackByName(musicTrackId, volume);
        }
        EventManager.instance.finishEventExecution();
    }
}

/// <summary>
/// Play music
/// </summary>
[System.Serializable]
public class SEInputs_SFX : ScriptedEventInputs
{
    public string sfxTrackId;

    public SEInputs_SFX(string trackId) : base(ScriptedEventType.SFX)
    {
        sfxTrackId = trackId;
    }

    public override IEnumerator getCoroutine()
    {
        yield return null;
        SfxManager.instance.playSFXbyName(sfxTrackId, 1);
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

/// <summary>
/// Scene transition
/// </summary>
[System.Serializable]
public class SEInputs_SceneTransition : ScriptedEventInputs
{
    public string sceneName;

    public SEInputs_SceneTransition(string s) : base(ScriptedEventType.SCENE_TRANSITION)
    {
        sceneName = s;
    }

    // TODO
    public override IEnumerator getCoroutine()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;
        EventManager.instance.finishEventExecution();
    }
}


public enum ScriptedEventType
{
    MOVEMENT,
    MOVEMENT_X,
    ENABLE,
    SPRITE_SWAP,
    FADE,
    SHOW,
    HIDE,
    ANIMATE,
    MUSIC,
    SFX,
    SCENE_TRANSITION
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