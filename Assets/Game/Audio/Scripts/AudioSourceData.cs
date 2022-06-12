using UnityEngine;

public class AudioSourceData
{
    public AudioSource AudioSource = default;
    public AudioClipData AudioClipData = default;

    public AudioSourceData(AudioSource source, AudioClipData data = default)
    {
        AudioSource = source;
        AudioClipData = data;
    }
}
