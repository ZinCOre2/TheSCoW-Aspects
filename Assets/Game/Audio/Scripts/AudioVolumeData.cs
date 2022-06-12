using UnityEngine;

[CreateAssetMenu(fileName = "AudioVolume", menuName = "AudioVolume", order = 0)]
public class AudioVolumeData : ScriptableObject
{
    public float MusicVolume = 1f;
    public float SoundVolume = 1f;
}
