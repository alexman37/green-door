using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryDriver : MonoBehaviour
{
    public static StoryDriver instance;
    public TextAsset initialFile;
    public DialogueItem dialogueItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(setupPhase());
    }

    // Wait until all managers and important stuff are created. then begin with the story
    IEnumerator setupPhase()
    {
        Debug.Log("Setting up story driver...");
        yield return new WaitUntil(() =>
        {
            return DialogueManager.greenlight;
        });
        Debug.Log("Story Driver DONE");

        if(dialogueItem)
        {
            DialogueManager.instance.processConversation(dialogueItem);
        } else
        {
            DialogueManager.instance.processConversation(initialFile);
        }
    }
}

public enum StartupService
{
    Dialogue,
    Events,
    Rooms
}
