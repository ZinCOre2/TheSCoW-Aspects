using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClipData> InternalClipDatas = new List<AudioClipData>();
    public List<AudioSource> AudioSources = new List<AudioSource>();
    public int InitialSourcesCapacity = 10;

    private void Start()
    {
        for (var i = 0; i < InitialSourcesCapacity; i++)
        {
            AudioSources.Add(gameObject.AddComponent<AudioSource>());
        }

        foreach (var clipData in InternalClipDatas)
        {
            if (clipData.PlayOnAwake)
            {
                UseAudioSource(clipData);
            }
        }
    }
    
    private void Update()
    {
        transform.position = GameController.Instance.
            CameraController.Camera.transform.position;
    }

    public AudioSource UseAudioSource(AudioClipData clipData)
    {
        var audioSource = ProvideAudioSource();
        
        audioSource.clip = clipData.Clip;
        audioSource.volume = clipData.Volume;
        audioSource.pitch = clipData.Pitch;
        audioSource.priority = clipData.Priority;
        audioSource.loop = clipData.Loop;
        
        audioSource.Play();

        return audioSource;
    }
    private AudioSource ProvideAudioSource()
    {
        AudioSource audioSource = null;
        
        foreach (var source in AudioSources)
        {
            if (!source.isPlaying)
            {
                audioSource = source;
                break;
            }    
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        return audioSource;
    }
}
