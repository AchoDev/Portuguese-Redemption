using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueSignalManager : MonoBehaviour, INotificationReceiver
{
    [SerializeField] SignalAsset signalAsset;

    public DialogueCutsceneManager manager;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if(notification is DialogueSignalEmitter dialogueSignal) {
            manager.StartDialogue(dialogueSignal.value);
        }
    }
}
