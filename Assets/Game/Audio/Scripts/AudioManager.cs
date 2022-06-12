using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioVolumeData AudioVolumeData;

    public List<AudioClipData> InternalClipDatas = new List<AudioClipData>();
    public List<AudioSourceData> AudioSourcesData = new List<AudioSourceData>();
    public int InitialSourcesCapacity = 10;

    private void Start()
    {
        for (var i = 0; i < InitialSourcesCapacity; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            AudioSourcesData.Add(new AudioSourceData(source));
        }

        foreach (var clipData in InternalClipDatas)
        {
            if (clipData.PlayOnAwake)
            {
                UseAudioSourceData(clipData);
            }
        }
    }
    private void Update()
    {
        transform.position = Camera.main.transform.position;
    }

    public AudioSourceData UseAudioSourceData(AudioClipData clipData)
    {
        var audioSourceData = ProvideAudioSourceData();
        
        audioSourceData.AudioSource.clip = clipData.Clip;
        audioSourceData.AudioSource.volume = clipData.Volume * (clipData.IsMusic ? AudioVolumeData.MusicVolume : AudioVolumeData.SoundVolume);
        audioSourceData.AudioSource.pitch = clipData.Pitch;
        audioSourceData.AudioSource.priority = clipData.Priority;
        audioSourceData.AudioSource.loop = clipData.Loop;

        audioSourceData.AudioClipData = clipData;
        
        audioSourceData.AudioSource.Play();

        return audioSourceData;
    }

    public void UpdateMusicSources(float value)
    {
        AudioVolumeData.MusicVolume = value;
        foreach (var asData in AudioSourcesData)
        {
            if (asData.AudioClipData && asData.AudioClipData.IsMusic)
            {
                asData.AudioSource.volume = asData.AudioClipData.Volume * AudioVolumeData.MusicVolume;
            }
        }
    }
    public void UpdateSoundSources(float value)
    {
        AudioVolumeData.SoundVolume = value;
        foreach (var asData in AudioSourcesData)
        {
            if (asData.AudioClipData && !asData.AudioClipData.IsMusic)
            {
                asData.AudioSource.volume = asData.AudioClipData.Volume * AudioVolumeData.SoundVolume;
            }
        }
    }
    
    private AudioSourceData ProvideAudioSourceData()
    {
        AudioSourceData audioSourceData = null;
        
        foreach (var source in AudioSourcesData)
        {
            if (!source.AudioSource.isPlaying)
            {
                audioSourceData = source;
                break;
            }    
        }

        if (audioSourceData == null)
        {
            audioSourceData = new AudioSourceData(gameObject.AddComponent<AudioSource>());
        }

        return audioSourceData;
    }
}
