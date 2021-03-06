﻿using UnityEngine;
using System;
using System.Collections;

public class FMODEmitter : MonoBehaviour
{
    [Header("Custom FMOD Emitter")]
    [FMODUnity.EventRef]
    public string Event = "";
    public bool playOnStart = false;

    [Header("3D Settings")]
    public bool is3D = true;
    /*
    public bool OverrideAttenuation = false;
    [Range(0f, 1000f)]
    public float OverrideMinDistance = 1.0f;
    [Range(0f, 1000f)]
    public float OverrideMaxDistance = 20.0f;
    */

    [Header("Initial Parameters")]
    public FMODUnity.ParamRef[] parameters = new FMODUnity.ParamRef[0];

    private FMOD.Studio.EventInstance instance;

    private FMOD.Studio.EventDescription eventDescription;

    public void Awake()
    {
        if (playOnStart)
        {
            if (Event != "")
            {
                SetEvent(Event);
                Play();
            }
        }
    }

    public void SetEvent(string Event)
    {
        //Debug.Log("Changed FMOD Emitter event to:" + Event);
        this.Event = Event;
    }

    public void Play()
    {
        if (!instance.isValid())
        {
            //Debug.Log("Play FMOD Emitter. Event: " + Event);
            instance = FMODUnity.RuntimeManager.CreateInstance(Event);

            // Only want to update if we need to set 3D attributes
            if (is3D)
            {
                Rigidbody2D rigidBody2D = GetComponent<Rigidbody2D>();
                Transform transform = GetComponent<Transform>();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
            }

            /*
            if (is3D && OverrideAttenuation)
            {
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
            }
            */

            instance.start();
        }
    }

    public void Stop()
    {
        //Debug.Log("Stop FMOD Emitter. Event: " + Event);
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void Kill()
    {
        //Debug.Log("Kill FMOD Emitter. Event: " + Event);
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }

    public void SetParameter(string name, float value)
    {
        instance.setParameterValue(name, value);
    }

    public bool IsPlaying()
    {
        if (instance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }

    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position, "FMODEmitter.tiff", true);

        if (is3D && OverrideAttenuation)
        {
            Gizmos.color = new Color(0.6f, 0.6f, 1f, 0.8f);
            if (OverrideAttenuation)
            {
                Gizmos.DrawWireSphere(transform.position, OverrideMinDistance);
                Gizmos.DrawWireSphere(transform.position, OverrideMaxDistance);
            }
        }
    }
    */

    public float GetLength(string eventPath)
    {
        if (instance.isValid())
        {
            eventDescription = FMODUnity.RuntimeManager.GetEventDescription(eventPath);
            int length;
            eventDescription.getLength(out length);
            return length / 1000f;
        }
        else
            return -1f;
    }

    public void ChangeAfterTime(float time, string eventPath)
    {
        StartCoroutine(LengthTimer(time, eventPath));
    }

    private IEnumerator LengthTimer(float time, string eventPath)
    {
        yield return new WaitForSeconds(time);
        Stop();
        SetEvent(eventPath);
        Play();
    }
}
