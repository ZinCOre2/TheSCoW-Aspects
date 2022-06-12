using UnityEngine;

[CreateAssetMenu(fileName = "AudioClip", menuName = "AudioClip", order = 0)]
public class AudioClipData : ScriptableObject
{
    public AudioClip Clip = default;
    public float Volume = 1;
    public float Pitch = 1;
    public int Priority = 128;
    public bool Loop = false;
    public bool PlayOnAwake = false;

    public bool IsMusic = false;
}
